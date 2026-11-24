using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    public class BugFixTests
    {
        [Test]
        public void Tab_MatchesRealTabCharacter()
        {
            var verbEx = VerbalExpressions.DefaultExpression.StartOfLine().Then("a").Tab().Then("b");

            Assert.IsTrue(verbEx.IsMatch("a\tb"));
            Assert.IsFalse(verbEx.IsMatch(@"a\tb"));
        }

        [TestCase("{")]
        [TestCase("}")]
        [TestCase("{}")]
        public void SomethingBut_WithCurlyBraces_DoesNotThrowAndExcludesThem(string excluded)
        {
            var verbEx = VerbalExpressions.DefaultExpression.StartOfLine().SomethingBut(excluded).EndOfLine();

            Assert.IsTrue(verbEx.IsMatch("abc"));
            Assert.IsFalse(verbEx.IsMatch("a" + excluded + "c"));
        }

        [Test]
        public void AnyOf_WithClosingBracket_MatchesEachCharacterSeparately()
        {
            var verbEx = VerbalExpressions.DefaultExpression.StartOfLine().AnyOf("a]").EndOfLine();

            Assert.IsTrue(verbEx.IsMatch("a"));
            Assert.IsTrue(verbEx.IsMatch("]"));
            Assert.IsFalse(verbEx.IsMatch("a]"));
        }

        [Test]
        public void AnythingBut_And_SomethingBut_WithClosingBracket_ExcludeIt()
        {
            var anythingBut = VerbalExpressions.DefaultExpression.StartOfLine().AnythingBut("a]").EndOfLine();
            var somethingBut = VerbalExpressions.DefaultExpression.StartOfLine().SomethingBut("a]").EndOfLine();

            Assert.IsTrue(anythingBut.IsMatch("xyz"));
            Assert.IsFalse(anythingBut.IsMatch("x]z"));
            Assert.IsTrue(somethingBut.IsMatch("xyz"));
            Assert.IsFalse(somethingBut.IsMatch("x]z"));
        }

        [Test]
        public void AnyOf_ClosingBracketOnly_StillMatches()
        {
            Assert.IsTrue(VerbalExpressions.DefaultExpression.AnyOf("]").IsMatch("]"));
        }

        [Test]
        public void TestExact_WithTrailingCommentInIgnorePatternWhitespaceMode_DoesNotThrow()
        {
            var verbEx = VerbalExpressions.DefaultExpression.Add("abc # note", false).AddModifier('x');

            Assert.IsTrue(verbEx.Test("abc"));
            Assert.IsTrue(verbEx.TestExact("abc"));
            Assert.IsFalse(verbEx.TestExact("abcd"));
        }

        [Test]
        public void TestExact_WithoutIgnorePatternWhitespaceMode_TreatsHashLiterally()
        {
            var verbEx = VerbalExpressions.DefaultExpression.Add("abc # note", false);

            Assert.IsTrue(verbEx.TestExact("abc # note"));
            Assert.IsFalse(verbEx.TestExact("abc"));
        }

        [Test]
        public void CommonRegexUrl_ContainsOnlyAsciiCharacters()
        {
            foreach (var c in CommonRegex.Url.Name)
            {
                Assert.Less((int)c, 128, "Unexpected non-ASCII character U+{0:X4} in CommonRegex.Url", (int)c);
            }
        }

        [Test]
        public void CommonRegexUrl_MatchesUrlWithQueryAndFragment()
        {
            var verbEx = VerbalExpressions.DefaultExpression.StartOfLine().Then(CommonRegex.Url).EndOfLine();

            Assert.IsTrue(verbEx.IsMatch("https://www.example.com/path?query=1#section"));
        }
    }
}
