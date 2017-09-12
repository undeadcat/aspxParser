using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace SomeParser.Tests
{
    public class UnitTestBase
    {
        private const string TestDataDirectory = "TestData";

        protected static string GetBaseDirectory() =>
            AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;

        protected static void AssertSameText(string actual, string expected)
        {
            actual = actual.SplitLines().Select(c => c.TrimEnd()).JoinStrings("\n");
            expected = expected.SplitLines().Select(c => c.TrimEnd()).JoinStrings("\n");
            if (string.Equals(actual, expected)) return;
            Console.WriteLine("Actual: \n" + actual);
            Console.WriteLine("Expected: \n" + expected);
            Assert.That(actual, Is.EqualTo(expected).NoClip);
        }

        protected static string GetTestDataDirectory()
        {
            return Path.Combine(GetBaseDirectory(), "..", "..", TestDataDirectory);
        }
    }
}