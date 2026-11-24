using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{
    /// <summary>
    /// Every code example in README.md is mirrored here, so the documentation
    /// stays correct. If you change an example in the README, change it here too.
    /// </summary>
    [TestFixture]
    public class ReadmeExamplesTests
    {
        [Test]
        public void QuickStart_ValidUrl()
        {
            var verbEx = new VerbalExpressions()
                .StartOfLine()
                .Then("http")
                .Maybe("s")
                .Then("://")
                .Maybe("www.")
                .AnythingBut(" ")
                .EndOfLine();

            Assert.IsTrue(verbEx.Test("https://www.google.com"));
            Assert.AreEqual(@"^(http)(s)?(://)(www\.)?([^\ ]*)$", verbEx.ToString());
        }

        [Test]
        public void ReplacingStrings()
        {
            string result = new VerbalExpressions()
                .Find("bird")
                .Replace("Replace bird with a duck", "duck");

            Assert.AreEqual("Replace duck with a duck", result);
        }

        [Test]
        public void NamedCaptureGroups()
        {
            var verbEx = new VerbalExpressions()
                .Find("Order #")
                .BeginCapture("id").Digit().OneOrMore().EndCapture();

            string id = verbEx.Capture("Thanks! Order #12345 is on its way.", "id");

            Assert.AreEqual("12345", id);
        }

        [Test]
        public void TestVersusTestExact()
        {
            var verbEx = new VerbalExpressions().Then("abc").Or("def");

            Assert.IsTrue(verbEx.Test("defzzz"));
            Assert.IsFalse(verbEx.TestExact("defzzz"));
            Assert.IsTrue(verbEx.TestExact("def"));
        }

        [Test]
        public void ReusingExpressions()
        {
            var number = new VerbalExpressions().Digit().OneOrMore();

            var version = new VerbalExpressions()
                .StartOfLine()
                .Add(number).Then(".").Add(number).Then(".").Add(number)
                .EndOfLine();

            Assert.IsTrue(version.Test("0.4.0"));
            Assert.IsFalse(version.Test("0.4"));
            Assert.AreEqual(@"^(?:\d+)(\.)(?:\d+)(\.)(?:\d+)$", version.ToString());
        }

        [Test]
        public void OneOfAndCaseInsensitive()
        {
            var verbEx = new VerbalExpressions()
                .StartOfLine()
                .OneOf("jpg", "jpeg", "png")
                .EndOfLine()
                .WithAnyCase();

            Assert.IsTrue(verbEx.Test("PNG"));
            Assert.IsFalse(verbEx.Test("gif"));
            Assert.AreEqual("^(?:jpg|jpeg|png)$", verbEx.ToString());
        }

        [Test]
        public void UsingTheRegex()
        {
            var verbEx = new VerbalExpressions().Word();

            System.Text.RegularExpressions.Regex regex = verbEx.ToRegex();

            Assert.AreEqual(3, regex.Matches("three words here").Count);
        }
    }
}
