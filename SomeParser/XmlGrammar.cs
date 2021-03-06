﻿using System;
using System.Collections.Generic;
using System.Linq;
using Irony.Ast;
using Irony.Parsing;

namespace SomeParser
{
    public class XmlGrammar : Grammar
    {
        private readonly IdentifierTerminal simpleIdentifier = new IdentifierTerminal("identifier")
        {
            Flags = TermFlags.NoAstNode
        };

        public XmlGrammar()
            : base(false)
        {
            LanguageFlags = LanguageFlags.CreateAst;
            NonGrammarTerminals.Add(new CommentTerminal("comment", "<%--", "--%>"));

            var element = Transient("element", null);
            var identifier = Identifier();
            var attributesList = AttributesList(identifier);
            var xmlText = new XmlTextTerminal(new List<Terminal> {ToTerm(">"), ToTerm("/>")},
                (context, node) => node.AstNode = new XmlText {Content = node.FindTokenAndGetText()});
            var tag = Tag(identifier, attributesList, element);
            var directive = Directive(identifier, attributesList);
            var codeFragment = CodeFragment();
            var document = NonTerminal("document", null, node => new AspNetXmlDocument
            {
                Elements = node.ChildNodes.Select(c => c.AstNode).Cast<ITagContent>().ToList()
            });

            element.Rule = tag | directive | codeFragment | xmlText;
            document.Rule = MakeStarRule(document, element);
            Root = document;
        }

        private NonTerminal Identifier()
        {
            var identifier = NonTerminal("identifier", null, node => new CompositeIdentifier
            {
                Components = node.ChildNodes.Select(n => n.FindTokenAndGetText()).ToList()
            });
            identifier.Rule = MakePlusRule(identifier, ToTerm(":"), simpleIdentifier);
            return identifier;
        }

        private NonTerminal AttributesList(BnfTerm identifier)
        {
            var options = StringOptions.AllowsAllEscapes | StringOptions.AllowsLineBreak;
            var attributeValue = new StringLiteral("attributeValue", "'", options,
                (context, node) => node.AstNode = node.Token.ValueString);
            attributeValue.AddStartEnd("\"", options);
            var attribute = NonTerminal("attribute", null, node => new Attribute
            {
                Name = (CompositeIdentifier) node.ChildNodes[0].AstNode,
                Value = (string) node.ChildNodes[2].AstNode
            });
            var attributesList = NonTerminal("attributesList", null,
                node => node.ChildNodes.Select(c => (Attribute) c.AstNode).ToList());

            attribute.Rule = identifier + "=" + attributeValue;
            attributesList.Rule = MakeStarRule(attributesList, attribute);
            return attributesList;
        }

        private NonTerminal Tag(BnfTerm identifier, BnfTerm attributesList, BnfTerm element)
        {
            var tagStart = NonTerminal("tagStart", null, node => new TagStart
            {
                Name = (CompositeIdentifier) node.ChildNodes[1].AstNode,
                Attributes = (List<Attribute>) node.ChildNodes[2].AstNode
            });
            var contentTagEnd = NonTerminal("contentTagEnd", null,
                node => node.ChildNodes[1].AstNode);
            var tagEnd = Transient("tagEnd", null);
            var tagContent = NonTerminal("tagContent", null,
                node => node.ChildNodes.Select(c => c.AstNode).Cast<ITagContent>().ToList());
            var tag = NonTerminal("tag", null, node =>
            {
                var start = (TagStart) node.ChildNodes[0].AstNode;
                var content = (List<ITagContent>) node.ChildNodes[1]
                                  .ChildNodes.ElementAtOrDefault(1)?
                                  .AstNode ?? new List<ITagContent>();
                return new Tag
                {
                    Name = start.Name,
                    Attributes = start.Attributes,
                    Content = content
                };
            });

            tagStart.Rule = "<" + identifier + attributesList;
            tagContent.Rule = MakeStarRule(tagContent, element);
            contentTagEnd.Rule = ToTerm(">") + tagContent + "</" + identifier + ToTerm(">");
            tagEnd.Rule = "/>" | contentTagEnd;
            tag.Rule = tagStart + tagEnd;
            return tag;
        }

        private static BnfTerm CodeFragment()
        {
            return new CodeFragmentTerm
            {
                AstConfig =
                {
                    NodeCreator = (context, node) =>
                    {
                        node.AstNode = new CodeFragment
                        {
                            Type = GetCodeFragmentType(node.Token.Text),
                            Value = node.Token.ValueString
                        };
                    }
                }
            };
        }

        private static CodeFragmentType GetCodeFragmentType(string tokenText)
        {
            if (tokenText.StartsWith("<%#"))
                return CodeFragmentType.DataBinding;
            if (tokenText.StartsWith("<%:"))
                return CodeFragmentType.EscapedExpression;
            if (tokenText.StartsWith("<%="))
                return CodeFragmentType.Expression;
            if (tokenText.StartsWith("<%"))
                return CodeFragmentType.Code;
            throw new ArgumentOutOfRangeException(nameof(tokenText),
                $"Could not determine code fragment type from token {tokenText}");
        }

        private static NonTerminal Directive(BnfTerm identifier, BnfTerm attributesList)
        {
            var directive = NonTerminal("directive", null, node => new Directive
            {
                Name = (CompositeIdentifier) node.ChildNodes[1].AstNode,
                Attributes = (List<Attribute>) node.ChildNodes[2].AstNode
            });
            directive.Rule = "<%@" + identifier + attributesList + "%>";
            return directive;
        }

        private class XmlTextTerminal : Terminal
        {
            private readonly List<Terminal> tagEndTokens;

            public XmlTextTerminal(List<Terminal> tagEndTokens, AstNodeCreator nodeCreator) : base("xmlText")
            {
                AstConfig.NodeCreator = nodeCreator;
                this.tagEndTokens = tagEndTokens;
            }

            public override Token TryMatch(ParsingContext context, ISourceStream source)
            {
                if (context.PreviousToken == null || !tagEndTokens.Contains(context.PreviousToken.Terminal))
                    return null;
                var stopIndex = source.Text.IndexOf('<', source.Location.Position);
                if (stopIndex < 0 || stopIndex == source.Location.Position)
                    return null;
                source.PreviewPosition = stopIndex;
                return source.CreateToken(OutputTerminal);
            }
        }

        private class CodeFragmentTerm : Terminal
        {
            private const string DirectivePrefix = "<%@";

            private readonly List<string> prefixes = new List<string>
            {
                "<%=",
                "<%:",
                "<%#",
                "<%" //this needs to be last to avoid matching longer prefixes
            };

            public CodeFragmentTerm()
                : base("codeFragment")
            {
            }

            public override IList<string> GetFirsts()
            {
                return prefixes.Select(c => c.Substring(0, 1)).ToList();
            }

            public override Token TryMatch(ParsingContext context, ISourceStream source)
            {
                foreach (var prefix in prefixes)
                {
                    if (!PrefixMatches(source, prefix))
                        continue;
                    if (DirectivePrefix.StartsWith(prefix) && PrefixMatches(source, DirectivePrefix))
                        continue;
                    var startIndex = source.Location.Position + prefix.Length;
                    const string suffix = "%>";
                    var endIndex = source.Text.IndexOf(suffix, startIndex, StringComparison.Ordinal);
                    if (endIndex < 0)
                        continue;
                    source.PreviewPosition = endIndex + suffix.Length;
                    var tokenValue = source.Text.Substring(startIndex, endIndex - startIndex);
                    return source.CreateToken(OutputTerminal, tokenValue);
                }
                return null;
            }

            private static bool PrefixMatches(ISourceStream source, string prefix)
            {
                return prefix.Where((t, i) => source.Text[source.Location.Position + i] != t).IsEmpty();
            }
        }

        private static NonTerminal Transient(string name, BnfExpression rule)
        {
            return new NonTerminal(name, rule) {Flags = TermFlags.IsTransient};
        }

        private static NonTerminal NonTerminal<T>(string name, BnfExpression rule, Func<ParseTreeNode, T> creator)
        {
            return new NonTerminal(name, (context, n) =>
            {
                try
                {
                    n.AstNode = creator(n);
                }
                catch (Exception e)
                {
                    var input = GetTokens(n).JoinStrings(" ");
                    const string messageFormat = "exception creating ast node from node {0} [{1}]";
                    throw new InvalidOperationException(string.Format(messageFormat, n, input), e);
                }
            })
            {
                Rule = rule
            };
        }

        private static IEnumerable<Token> GetTokens(ParseTreeNode node)
        {
            return node.Token != null ? new[] {node.Token} : node.ChildNodes.SelectMany(GetTokens);
        }

        private class TagStart
        {
            public CompositeIdentifier Name { get; set; }
            public List<Attribute> Attributes { get; set; }
        }
    }
}