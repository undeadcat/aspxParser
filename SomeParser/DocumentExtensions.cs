using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeParser
{
    public static class DocumentExtensions
    {
        public static IEnumerable<Tag> CollectTags(this AspNetXmlDocument document)
        {
            return CollectElements<Tag>(document);
        }

        private static IEnumerable<T> CollectElements<T>(this ITagContent arg)
        {
            var tag = arg as Tag;
            return tag?.Content.OfType<T>().Union(tag.Content.SelectMany(CollectElements<T>))
                   ?? Enumerable.Empty<T>();
        }

        public static IEnumerable<T> CollectElements<T>(this AspNetXmlDocument document)
        {
            return document.Elements.OfType<T>().Union(document.Elements.SelectMany(CollectElements<T>));
        }

        public static string GetAttributeValueOrDie(this IAttributeOwner tag, string name)
        {
            var attribute = GetAttributeValueOrNull(tag, name);
            if (attribute == null)
                throw new InvalidOperationException($"Could not find attribute by name {name} in tag {tag}");
            return attribute;
        }

        public static string GetAttributeValueOrNull(this IAttributeOwner tag, string name)
        {
            return tag.Attributes.SingleOrDefault(a => a.Name.GetName().EqualsIgnoringCase(name))?.Value;
        }
    }
}