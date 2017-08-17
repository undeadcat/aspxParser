using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;

namespace SomeParser.Tests
{
    [TestFixture]
    public class ParserTests
    {
        private static string GetBaseDirectory() => AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;

        private const int IndentSize = 4;
        private const string TestDataDirectory = "TestData";

        [Test]
        public void PaymentsList()
        {
            var document = new XmlParser().Parse(File.ReadAllText(Path.Combine(GetTestDataDirectory(), "PaymentsList.aspx")));
            Console.WriteLine(DumpToString(document));
    
        }
        
        [Test]
        public void SimpleTag()
        {
            DoTestFromText("<div></div>", @"<AspNetXmlDocument>
Elements: 
    <Tag>
    Name: 
        <CompositeIdentifier>
        Components: 
            div
    Attributes: 

    Content: 
");
        }

        [Test]
        public void TagWithAttributes()
        {
            DoTestFromText("<div foo=\"fooValue\" bar = \"some\nmultiline\nvalue\"></div>",
                @"<AspNetXmlDocument>
Elements: 
    <Tag>
    Name: 
        <CompositeIdentifier>
        Components: 
            div
    Attributes: 
        <Attribute>
        Name: 
            <CompositeIdentifier>
            Components: 
                foo
        Value: 
            fooValue
        <Attribute>
        Name: 
            <CompositeIdentifier>
            Components: 
                bar
        Value: 
            some
multiline
value
    Content: ");
        }

        [Test]
        public void SelfClosingTagWithAttributes()
        {
            DoTestFromText("<div foo=\"fooValue\" bar = \"barValue\"/>",
                @"<AspNetXmlDocument>
Elements: 
    <Tag>
    Name: 
        <CompositeIdentifier>
        Components: 
            div
    Attributes: 
        <Attribute>
        Name: 
            <CompositeIdentifier>
            Components: 
                foo
        Value: 
            fooValue
        <Attribute>
        Name: 
            <CompositeIdentifier>
            Components: 
                bar
        Value: 
            barValue
    Content: ");
        }

        [Test]
        public void NamespacedTagAndAttributeNames()
        {
            DoTestFromText("<ib:AccountingWizardMessageControl " +
                           "meta:resourcekey=\'fff\' " +
                           "ID=\"AccountingWizardMessage\" " +
                           "runat=\"server\"/>",
                @"<AspNetXmlDocument>
Elements: 
    <Tag>
    Name: 
        <CompositeIdentifier>
        Components: 
            ib
            AccountingWizardMessageControl
    Attributes: 
        <Attribute>
        Name: 
            <CompositeIdentifier>
            Components: 
                meta
                resourcekey
        Value: 
            fff
        <Attribute>
        Name: 
            <CompositeIdentifier>
            Components: 
                ID
        Value: 
            AccountingWizardMessage
        <Attribute>
        Name: 
            <CompositeIdentifier>
            Components: 
                runat
        Value: 
            server
    Content: 
");
        }

        [Test]
        public void NestedTags()
        {
            DoTestFromText("<foo><bar><baz/><baz/></bar></foo>", @"<AspNetXmlDocument>
Elements: 
    <Tag>
    Name: 
        <CompositeIdentifier>
        Components: 
            foo
    Attributes: 

    Content: 
        <Tag>
        Name: 
            <CompositeIdentifier>
            Components: 
                bar
        Attributes: 

        Content: 
            <Tag>
            Name: 
                <CompositeIdentifier>
                Components: 
                    baz
            Attributes: 

            Content: 

            <Tag>
            Name: 
                <CompositeIdentifier>
                Components: 
                    baz
            Attributes: 

            Content: ");
        }

        [Test]
        public void XmlText()
        {
            DoTestFromText("<div> some xml текст &nbsp; </div>", @"<AspNetXmlDocument>
Elements: 
    <Tag>
    Name: 
        <CompositeIdentifier>
        Components: 
            div
    Attributes: 

    Content: 
        <XmlText>
        Content: 
            some xml текст &nbsp; ");
        }

        [Test]
        public void TopLevelDirectives()
        {
            DoTestFromFile();
        }

        [Test]
        public void CodeFragments()
        {
            DoTestFromText(@"<div>
	<%# DataBoundProp %>
	<% SomePartial(); %>
	<%= 1 
        + 
        1 %>
	<%: 1 + 1 %>
</div>", @"<AspNetXmlDocument>
Elements: 
    <Tag>
    Name: 
        <CompositeIdentifier>
        Components: 
            div
    Attributes: 

    Content: 
        <CodeFragment>
        Type: 
            DataBinding
        Value: 
             DataBoundProp 
        <CodeFragment>
        Type: 
            Code
        Value: 
             SomePartial(); 
        <CodeFragment>
        Type: 
            Expression
        Value: 
             1 
        + 
        1 
        <CodeFragment>
        Type: 
            EscapedExpression
        Value: 
             1 + 1 ");
        }

        [Test]
        public void Comments()
        {
            DoTestFromText(@"<%--<span>--%>
<%--$1$	text#1#--%>
<%--</span>--%>
<div>
<%--	<%= some commented expression %>--%>
</div>", @"<AspNetXmlDocument>
Elements: 
    <Tag>
    Name: 
        <CompositeIdentifier>
        Components: 
            div
    Attributes: 

    Content: ");
        }

        [Test]
        public void OptionalEndTags()
        {
            DoTestFromText(@"<div>
well<br>
hello<br>
there<br>
", @"");
        }

        private static void DoTestFromFile()
        {
            var testName = TestContext.CurrentContext.Test.MethodName;
            var srcFilePath = Path.Combine(GetTestDataDirectory(), testName + ".txt");
            var expectedFilePath = Path.Combine(GetTestDataDirectory(), testName + ".expected");
            var document = new XmlParser().Parse(File.ReadAllText(srcFilePath));
            var actual = DumpToString(document);
            if (!File.Exists(expectedFilePath))
            {
                File.WriteAllText(expectedFilePath, actual);
                Assert.Fail("Expected file not found at path {0}. Written file", expectedFilePath);
            }

            AssertEquals(actual, File.ReadAllText(expectedFilePath));
        }

        private static string GetTestDataDirectory()
        {
            return Path.Combine(GetBaseDirectory(), "..", "..", TestDataDirectory);
        }

        private static void DoTestFromText(string source, string expected)
        {
            var document = new XmlParser().Parse(source);
            var actual = DumpToString(document);
            AssertEquals(actual, expected);
        }

        private static void AssertEquals(string actual, string expected)
        {
            if (string.Equals(actual.Trim(), expected.Trim())) return;
            Console.WriteLine("Actual: \n" + actual);
            Console.WriteLine("Expected: \n" + expected);
            Assert.That(actual, Is.EqualTo(expected).NoClip);
        }

        private static string DumpToString(object obj)
        {
            var stringBuilder = new StringBuilder();
            DumpToString(stringBuilder, 0, obj);
            return stringBuilder.ToString();
        }

        private static void DumpToString(StringBuilder result, int indentDepth, object obj)
        {
            if (obj == null)
            {
                result.Append("null");
                return;
            }
            var type = obj.GetType();
            if (type.IsSimpleType())
            {
                result.Append(Enumerable.Repeat(" ", indentDepth * IndentSize).JoinStrings());
                result.Append(obj);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type) || type.IsArray)
            {
                var hasPrevious = false;
                foreach (var el in (IEnumerable) obj)
                {
                    if (hasPrevious)
                        result.Append("\n");
                    DumpToString(result, indentDepth, el);
                    hasPrevious = true;
                }
            }
            else
            {
                var allInstance = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
                var members = type.GetFields(allInstance).Where(c => !c.Name.Contains("__BackingField"))
                    .Union<MemberInfo>(type.GetProperties(allInstance))
                    .ToArray();
                result.Append(Enumerable.Repeat(" ", indentDepth * IndentSize).JoinStrings());
                result.AppendFormat("<{0}>", type.Name);
                foreach (var memberInfo in members)
                {
                    var fieldInfo = memberInfo as FieldInfo;
                    object value = null;
                    if (fieldInfo != null)
                        value = fieldInfo.GetValue(obj);
                    var propInfo = memberInfo as PropertyInfo;
                    if (propInfo != null)
                        value = propInfo.GetValue(obj);

                    result.Append("\n");
                    result.Append(Enumerable.Repeat(" ", indentDepth * IndentSize).JoinStrings());
                    result.Append(String.Format("{0}: ", memberInfo.Name));
                    result.Append("\n");
                    DumpToString(result, indentDepth + 1, value);
                }
            }
        }

    }
}