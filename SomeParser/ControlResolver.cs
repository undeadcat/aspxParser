using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SomeParser
{
    public class ControlResolver
    {
        private readonly CSharpClassParser cSharpClassParser = new CSharpClassParser();

        public string Resolve(CompositeIdentifier tagId, AspNetXmlDocument document, string baseDirectory,
            string aspxRelativePath)
        {
            if (tagId.Components.Count == 1)
                return "System.Web.UI.HtmlControls.HtmlGenericControl";
            var registerDirective = document.CollectElements<Directive>()
                .Where(d => d.Name.GetName().EqualsIgnoringCase("Register"))
                .FirstOrDefault(d =>
                {
                    var tagPrefix = d.GetAttributeValueOrNull("TagPrefix");
                    var tagName = d.GetAttributeValueOrNull("TagName");
                    return tagId.GetName().EqualsIgnoringCase(tagPrefix + ":" + tagName);
                });
            if (registerDirective == null)
                throw new InvalidOperationException(
                    $"Could not find 'Register' directive for tag {tagId.GetName()} in file {aspxRelativePath}");
            var srcValue = registerDirective.GetAttributeValueOrNull("Src");
            var controlPath = srcValue.StartsWith("~")
                ? srcValue.Replace("~", baseDirectory)
                : Path.Combine(baseDirectory, Path.GetDirectoryName(aspxRelativePath), srcValue);
            var codeBehindPath = controlPath + ".cs";
            return cSharpClassParser.ParseQualifiedName(File.ReadAllText(codeBehindPath));
        }

        public class CSharpClassParser
        {
            private static readonly RegexOptions RegexOptions =
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline;

            //LOL
            private readonly Regex namespaceRegex =
                new Regex("namespace\\s*(?<namespaceName>\\w*(\\.\\w)*)", RegexOptions);

            private readonly Regex classRegex = new Regex("class\\s*(?<className>(\\w*))", RegexOptions);

            public string ParseQualifiedName(string content)
            {
                var namespaceGroup = namespaceRegex.Match(content).Groups["namespaceName"];
                if (!namespaceGroup.Success)
                    throw new InvalidOperationException($"Could not extract namespace from file content {content}");
                var classGroup = classRegex.Match(content).Groups["className"];
                if (!classGroup.Success)
                    throw new InvalidOperationException($"Could not extract class from file content {content}");
                return namespaceGroup.Value + "." + classGroup.Value;
            }
        }
    }
}