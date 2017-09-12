using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SomeParser
{
    public class DesignerGenerator
    {
        private readonly XmlParser parser = new XmlParser();
        private readonly ControlResolver controlResolver = new ControlResolver();

        public string Generate(string baseDirectory, string aspxFileName)
        {
            var document = parser.Parse(File.ReadAllText(Path.Combine(baseDirectory, aspxFileName)));
            var pageDirective = document.CollectElements<Directive>()
                .SingleOrDefault(d => d.Name.GetName().EqualsIgnoringCase("Page"));
            if (pageDirective == null)
                throw new InvalidOperationException($"'Page' directive not found in aspx {aspxFileName}");
            var inheritsAttribute =
                pageDirective.Attributes.SingleOrDefault(a => a.Name.GetName().EqualsIgnoringCase("Inherits"));
            if (inheritsAttribute == null)
                throw new InvalidOperationException(
                    $"'Inherits' attribute not found in page directive {pageDirective}");
            var pageQualifiedName = inheritsAttribute.Value.Split(new[] {"."}, StringSplitOptions.RemoveEmptyEntries);
            var pageClassName = pageQualifiedName.Last();
            var pageNamespace = pageQualifiedName.Take(pageQualifiedName.Length - 1).JoinStrings(".");

            var result = new StringBuilder();
            result.Append(@"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated. 
// </auto-generated>
//------------------------------------------------------------------------------

");
            result.AppendFormat("namespace {0} {{\n\n", pageNamespace);
            result.AppendFormat("    public partial class {0} {{\n\n", pageClassName);
            var controls = document.CollectTags()
                .Where(t => t.Attributes.Contains(a => a.Name.GetName().EqualsIgnoringCase("ID")))
                .Where(t => t.GetAttributeValueOrNull("runat").EqualsIgnoringCase("server"));

            foreach (var control in controls)
            {
                var controlClassName = controlResolver.Resolve(control.Name, document, baseDirectory, aspxFileName);
                var id = control.GetAttributeValueOrDie("ID");
                result.AppendLine("        /// <summary>");
                result.AppendFormat("        /// {0} control.\n", id);
                result.AppendLine("        /// </summary>");
                result.AppendLine("        /// <remarks>");
                result.AppendLine("        /// Auto-generated field.");
                result.AppendLine(
                    "        /// To modify move field declaration from designer file to code-behind file.");
                result.AppendLine("        /// </remarks>");
                result.AppendFormat(
                    "        protected global::{0} {1};\n", controlClassName, id);
            }
            result.Append("\n    }");
            result.Append("\n}");
            return result.ToString();
        }
    }
}