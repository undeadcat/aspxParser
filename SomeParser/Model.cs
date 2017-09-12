using System.Collections.Generic;

namespace SomeParser
{
    public class AspNetXmlDocument
    {
        public List<ITagContent> Elements { get; set; }
    }

    public class Directive : ITagContent, IAttributeOwner
    {
        public CompositeIdentifier Name { get; set; }
        public List<Attribute> Attributes { get; set; }

        public override string ToString()
        {
            return $"<%@{Name} ${Attributes.JoinStrings(" ")}%>";
        }
    }

    public class CompositeIdentifier
    {
        public List<string> Components { get; set; }

        public string GetName()
        {
            return Components.JoinStrings(":");
        }

        public override string ToString()
        {
            return GetName();
        }
    }

    public class Attribute
    {
        public CompositeIdentifier Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Name}='{Value}'";
        }
    }

    public class Tag : ITagContent, IAttributeOwner
    {
        public CompositeIdentifier Name { get; set; }
        public List<Attribute> Attributes { get; set; }
        public List<ITagContent> Content { get; set; }
    }

    public interface ITagContent
    {
    }

    public interface IAttributeOwner
    {
        List<Attribute> Attributes { get; set; }
    }

    public class XmlText : ITagContent
    {
        public string Content { get; set; }
    }

    public class CodeFragment : ITagContent
    {
        public CodeFragmentType Type { get; set; }
        public string Value { get; set; }
    }

    public enum CodeFragmentType
    {
        /// <code>&lt;# SomeCode(); %&gt;</code>
        DataBinding,

        /// <code>&lt;% SomeCode(); %&gt;</code>
        /// Code that is executed when the page is rendered.
        Code,

        /// <code>&lt;%=expression %&gt;</code>
        /// A shortcut for calling <code>&lt;% HttpResponse.Write(expression); %&gt;</code>
        Expression,

        /// <code>&lt;%:expression %&gt;</code>
        /// https://weblogs.asp.net/scottgu/new-lt-gt-syntax-for-html-encoding-output-in-asp-net-4-and-asp-net-mvc-2
        /// Seems to be equivalent to <code>&lt;%=Server.HtmlEncode(expression) %&gt;</code>
        EscapedExpression
    }
}