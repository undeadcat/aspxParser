using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeParser
{
    public static class Utils
    {
        public static string[] SplitLines(this string s)
        {
            return s.Split(new[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool EqualsIgnoringCase(this string s1, string s2)
        {
            return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoringCase(this string s1, string s2)
        {
            return s1.IndexOf(s2, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string ExcludeSuffix(this string s1, string suffix)
        {
            return s1 != null && s1.EndsWith(suffix)
                ? s1.Substring(0, s1.Length - suffix.Length)
                : s1;
        }

        public static string JoinStrings<T>(this IEnumerable<T> thisEnumerable, string separator = "")
        {
            return string.Join(separator, thisEnumerable);
        }

        public static bool IsSimpleType(this Type type)
        {
            if (SimpleTypes.Contains(type) || type.IsEnum)
                return true;
            var nullableWrapped = Nullable.GetUnderlyingType(type);
            return nullableWrapped != null && nullableWrapped.IsSimpleType();
        }

        private static readonly ISet<Type> SimpleTypes = new HashSet<Type>
        {
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(double),
            typeof(float),
            typeof(string),
            typeof(Guid),
            typeof(bool),
            typeof(DateTime),
            typeof(TimeSpan)
        };

        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !source.Any();
        }

        public static bool Contains<T>(this IEnumerable<T> source, Func<T, bool> condition)
        {
            return source.Where(condition).Any();
        }
    }
}