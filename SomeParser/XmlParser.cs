using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony;
using Irony.Parsing;

namespace SomeParser
{
    public class XmlParser
    {
        private readonly Parser parser;

        public XmlParser()
        {
            var queryGrammar = new XmlGrammar();
            var languageData = new LanguageData(queryGrammar);
            if (languageData.Errors.Count > 0)
            {
                var b = new StringBuilder();
                foreach (var error in languageData.Errors)
                    b.Append(error);
                throw new InvalidOperationException($"invalid grammar\n{b}");
            }
            parser = new Parser(languageData);
        }

        public AspNetXmlDocument Parse(string source)
        {
            var parseTree = parser.Parse(source);
            if (parseTree.Status != ParseTreeStatus.Parsed)
                throw new InvalidOperationException(FormatMessage(parseTree.ParserMessages, source));
            var result = (AspNetXmlDocument) parseTree.Root.AstNode;
            return result;
        }

        private static string FormatMessage(IEnumerable<LogMessage> errors, string source)
        {
            var b = new StringBuilder();
            foreach (var message in errors)
            {
                b.AppendLine(
                    $"{message.Level}: {message.Message} at {message.Location} in state {message.ParserState}");

                var theMessage = message;
                var lines = source
                    .Split(new[] {"\r\n", "\n"}, StringSplitOptions.None)
                    .Select((sourceLine, index) =>
                        index == theMessage.Location.Line
                            ? $"{sourceLine}\r\n{new string('_', theMessage.Location.Column)}|<-Here"
                            : sourceLine);
                foreach (var line in lines)
                    b.AppendLine(line);
            }
            return $"parse errors\r\n:{b}";
        }
    }
}