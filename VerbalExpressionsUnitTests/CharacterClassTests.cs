using System.Text.RegularExpressions;
using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    public class CharacterClassTests
    {
        [Test]
        public void Letter_WhenCalled_MatchesUpperAndLowerCaseLetters()
        {
            var verbEx = VerbalExpressions.DefaultExpression
                .StartOfLine()
                .Letter()
                .EndOfLine();

            Assert.IsTrue(verbEx.IsMatch("a"));
            Assert.IsTrue(verbEx.IsMatch("Z"));
            Assert.IsFalse(verbEx.IsMatch("5"));
            Assert.IsFalse(verbEx.IsMatch("!"));
        }

        [Test]
        public void Letter_WhenRepeated_MatchesMultipleLetters()
        {
            var verbEx = VerbalExpressions.DefaultExpression
                .StartOfLine()
                .Letter().RepeatPrevious(3)
                .EndOfLine();

            Assert.IsTrue(verbEx.IsMatch("abc"));
            Assert.IsTrue(verbEx.IsMatch("XYZ"));
            Assert.IsFalse(verbEx.IsMatch("ab"));
            Assert.IsFalse(verbEx.IsMatch("ab1"));
        }

        [Test]
        public void UpperCaseLetter_WhenCalled_MatchesOnlyUpperCase()
        {
            var verbEx = VerbalExpressions.DefaultExpression
                .StartOfLine()
                .UpperCaseLetter()
                .EndOfLine();

            Assert.IsTrue(verbEx.IsMatch("A"));
            Assert.IsTrue(verbEx.IsMatch("Z"));
            Assert.IsFalse(verbEx.IsMatch("a"));
            Assert.IsFalse(verbEx.IsMatch("5"));
        }

        [Test]
        public void LowerCaseLetter_WhenCalled_MatchesOnlyLowerCase()
        {
            var verbEx = VerbalExpressions.DefaultExpression
                .StartOfLine()
                .LowerCaseLetter()
                .EndOfLine();

            Assert.IsTrue(verbEx.IsMatch("a"));
            Assert.IsTrue(verbEx.IsMatch("z"));
            Assert.IsFalse(verbEx.IsMatch("A"));
            Assert.IsFalse(verbEx.IsMatch("5"));
        }

        [Test]
        public void Digit_WhenCalled_MatchesSingleDigit()
        {
            var verbEx = VerbalExpressions.DefaultExpression
                .StartOfLine()
                .Digit()
                .EndOfLine();

            Assert.IsTrue(verbEx.IsMatch("0"));
            Assert.IsTrue(verbEx.IsMatch("9"));
            Assert.IsFalse(verbEx.IsMatch("a"));
            Assert.IsFalse(verbEx.IsMatch("12"));
        }

        [Test]
        public void Digit_WhenRepeated_MatchesMultipleDigits()
        {
            var verbEx = VerbalExpressions.DefaultExpression
                .StartOfLine()
                .Digit().RepeatPrevious(3)
                .EndOfLine();

            Assert.IsTrue(verbEx.IsMatch("123"));
            Assert.IsTrue(verbEx.IsMatch("007"));
            Assert.IsFalse(verbEx.IsMatch("12"));
            Assert.IsFalse(verbEx.IsMatch("12a"));
        }

        [Test]
        public void WordBoundary_WhenCalled_MatchesAtWordBoundary()
        {
            var verbEx = VerbalExpressions.DefaultExpression
                .WordBoundary()
                .Then("cat")
                .WordBoundary();

            Assert.IsTrue(verbEx.IsMatch("the cat sat"));
            Assert.IsTrue(verbEx.IsMatch("cat"));
            Assert.IsFalse(verbEx.IsMatch("concatenate"));
        }
    }
}
