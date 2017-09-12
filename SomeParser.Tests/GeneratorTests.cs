using System.IO;
using NUnit.Framework;

namespace SomeParser.Tests
{
    public class GeneratorTests : UnitTestBase
    {
        //TODO. incomplete code everywhere
        public const string GeneratorTestDir = "Generator";

        [Test]
        public void UserControlRegisteredWithPageRelative()
        {
            DoTestFromFolder(Path.Combine("Pages", "Test.aspx"));
        }

        [Test]
        public void UserControlRegisteredWithRootRelative()
        {
            DoTestFromFolder(Path.Combine("Pages", "Test.aspx"));
        }

        [Test]
        public void ServerHtmlTag()
        {
            DoTestFromFolder("Test.aspx");
        }

        [Test]
        public void NotServerHtmlTag()
        {
            DoTestFromFolder("Test.aspx");
        }

        [Test]
        public void UserControlRegisteredInWebConfig()
        {
            DoTestFromFolder("Test.aspx");
        }

        private static void DoTestFromFolder(string aspxFileName)
        {
            var baseDirectory = Path.Combine(GetTestDataDirectory(), GeneratorTestDir,
                TestContext.CurrentContext.Test.MethodName);
            var generated = new DesignerGenerator().Generate(baseDirectory, aspxFileName);
            var expectedFileName = Path.ChangeExtension(aspxFileName, ".expected.aspx.designer.cs");
            var expected = File.ReadAllText(Path.Combine(baseDirectory, expectedFileName));
            AssertSameText(generated, expected);
        }
    }
}