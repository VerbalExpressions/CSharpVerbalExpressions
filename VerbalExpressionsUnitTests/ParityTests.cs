using System.Text.RegularExpressions;
using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{
    /// <summary>
    /// Tests for the methods that bring the C# API in line with the
    /// Java and JavaScript implementations of VerbalExpressions.
    /// </summary>
    [TestFixture]
    public class ParityTests
    {
        private static VerbalExpressions Exact()
        {
            return VerbalExpressions.DefaultExpression.StartOfLine();
        }

        #region Character classes

        [Test]
        public void WordChar_MatchesSingleWordCharacter()
        {
            var verbEx = Exact().WordChar().EndOfLine();

            Assert.AreEqual(@"^\w$", verbEx.ToString());
            Assert.IsTrue(verbEx.IsMatch("a"));
            Assert.IsTrue(verbEx.IsMatch("_"));
            Assert.IsFalse(verbEx.IsMatch("-"));
            Assert.IsFalse(verbEx.IsMatch("ab"));
        }

        [Test]
        public void NonWordChar_MatchesSingleNonWordCharacter()
        {
            var verbEx = Exact().NonWordChar().EndOfLine();

            Assert.AreEqual(@"^\W$", verbEx.ToString());
            Assert.IsTrue(verbEx.IsMatch("-"));
            Assert.IsFalse(verbEx.IsMatch("a"));
        }

        [Test]
        public void NonDigit_MatchesSingleNonDigit()
        {
            var verbEx = Exact().NonDigit().EndOfLine();

            Assert.AreEqual(@"^\D$", verbEx.ToString());
            Assert.IsTrue(verbEx.IsMatch("x"));
            Assert.IsFalse(verbEx.IsMatch("7"));
        }

        [Test]
        public void Whitespace_MatchesSingleWhitespaceCharacter()
        {
            var verbEx = Exact().Whitespace().EndOfLine();

            Assert.AreEqual(@"^\s$", verbEx.ToString());
            Assert.IsTrue(verbEx.IsMatch(" "));
            Assert.IsTrue(verbEx.IsMatch("\t"));
            Assert.IsFalse(verbEx.IsMatch("x"));
        }

        [Test]
        public void NonWhitespace_MatchesSingleNonWhitespaceCharacter()
        {
            var verbEx = Exact().NonWhitespace().EndOfLine();

            Assert.AreEqual(@"^\S$", verbEx.ToString());
            Assert.IsTrue(verbEx.IsMatch("x"));
            Assert.IsFalse(verbEx.IsMatch(" "));
        }

        [Test]
        public void Space_And_NonSpace_AreAliasesForWhitespace()
        {
            Assert.AreEqual(@"\s", VerbalExpressions.DefaultExpression.Space().ToString());
            Assert.AreEqual(@"\S", VerbalExpressions.DefaultExpression.NonSpace().ToString());
        }

        #endregion Character classes

        #region Quantifiers

        [Test]
        public void OneOrMore_AppendsPlusQuantifier()
        {
            var verbEx = Exact().Digit().OneOrMore().EndOfLine();

            Assert.AreEqual(@"^\d+$", verbEx.ToString());
            Assert.IsTrue(verbEx.IsMatch("2026"));
            Assert.IsFalse(verbEx.IsMatch(""));
        }

        [Test]
        public void ZeroOrMore_AppendsStarQuantifier()
        {
            var verbEx = Exact().Then("a").Digit().ZeroOrMore().EndOfLine();

            Assert.AreEqual(@"^(a)\d*$", verbEx.ToString());
            Assert.IsTrue(verbEx.IsMatch("a"));
            Assert.IsTrue(verbEx.IsMatch("a123"));
            Assert.IsFalse(verbEx.IsMatch("ab"));
        }

        [Test]
        public void AtLeast_AppendsOpenEndedQuantifier()
        {
            var verbEx = Exact().Digit().AtLeast(2).EndOfLine();

            Assert.AreEqual(@"^\d{2,}$", verbEx.ToString());
            Assert.IsFalse(verbEx.IsMatch("1"));
            Assert.IsTrue(verbEx.IsMatch("12"));
            Assert.IsTrue(verbEx.IsMatch("12345"));
        }

        [Test]
        public void Count_IsAliasForRepeatPrevious()
        {
            Assert.AreEqual(@"\d{3}", VerbalExpressions.DefaultExpression.Digit().Count(3).ToString());
            Assert.AreEqual(@"\d{2,4}", VerbalExpressions.DefaultExpression.Digit().Count(2, 4).ToString());
        }

        [Test]
        public void Multiple_WithMinimum_RepeatsAtLeastMinimumTimes()
        {
            var verbEx = Exact().Multiple("ab", 2).EndOfLine();

            Assert.AreEqual(@"^(ab){2,}$", verbEx.ToString());
            Assert.IsFalse(verbEx.IsMatch("ab"));
            Assert.IsTrue(verbEx.IsMatch("abab"));
            Assert.IsTrue(verbEx.IsMatch("ababab"));
        }

        [Test]
        public void Multiple_WithMinimumAndMaximum_RepeatsWithinRange()
        {
            var verbEx = Exact().Multiple("a", 2, 3).EndOfLine();

            Assert.AreEqual(@"^(a){2,3}$", verbEx.ToString());
            Assert.IsFalse(verbEx.IsMatch("a"));
            Assert.IsTrue(verbEx.IsMatch("aa"));
            Assert.IsTrue(verbEx.IsMatch("aaa"));
            Assert.IsFalse(verbEx.IsMatch("aaaa"));
        }

        [Test]
        public void Multiple_WithCounts_SanitizesValue()
        {
            Assert.AreEqual(@"(\.){1,2}", VerbalExpressions.DefaultExpression.Multiple(".", 1, 2).ToString());
        }

        [Test]
        public void AtLeast_NegativeValue_Throws()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => VerbalExpressions.DefaultExpression.AtLeast(-1));
        }

        [Test]
        public void Multiple_MaximumLessThanMinimum_Throws()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => VerbalExpressions.DefaultExpression.Multiple("a", 3, 2));
        }

        #endregion Quantifiers

        #region Grouping and alternation

        [Test]
        public void OneOf_MatchesAnyOfTheGivenValues()
        {
            var verbEx = Exact().OneOf("cat", "dog", "c++").EndOfLine();

            Assert.AreEqual(@"^(?:cat|dog|c\+\+)$", verbEx.ToString());
            Assert.IsTrue(verbEx.IsMatch("cat"));
            Assert.IsTrue(verbEx.IsMatch("dog"));
            Assert.IsTrue(verbEx.IsMatch("c++"));
            Assert.IsFalse(verbEx.IsMatch("cow"));
        }

        [Test]
        public void OneOf_DoesNotAddCaptureGroup()
        {
            var regex = VerbalExpressions.DefaultExpression.OneOf("a", "b").ToRegex();

            Assert.AreEqual(1, regex.Match("a").Groups.Count);
        }

        [Test]
        public void OneOf_NoValues_Throws()
        {
            Assert.Throws<System.ArgumentException>(() => VerbalExpressions.DefaultExpression.OneOf());
        }

        [Test]
        public void BeginGroup_EndGroup_CreatesNonCapturingGroup()
        {
            var verbEx = Exact()
                .BeginGroup().Then("ab").EndGroup().OneOrMore()
                .EndOfLine();

            Assert.AreEqual(@"^(?:(ab))+$", verbEx.ToString());
            Assert.IsTrue(verbEx.IsMatch("ababab"));
            Assert.IsFalse(verbEx.IsMatch("aba"));
        }

        [Test]
        public void Add_OtherExpression_IsWrappedInNonCapturingGroup()
        {
            var digits = VerbalExpressions.DefaultExpression.Digit().OneOrMore();
            var verbEx = Exact().Add(digits).Then("-").Add(digits).EndOfLine();

            Assert.AreEqual(@"^(?:\d+)(-)(?:\d+)$", verbEx.ToString());
            Assert.IsTrue(verbEx.IsMatch("123-456"));
            Assert.IsFalse(verbEx.IsMatch("123-"));
        }

        [Test]
        public void Add_OtherExpression_LeavesOtherUnchanged()
        {
            var digits = VerbalExpressions.DefaultExpression.Digit();
            VerbalExpressions.DefaultExpression.Add(digits).Then("x");

            Assert.AreEqual(@"\d", digits.ToString());
        }

        [Test]
        public void Add_NullExpression_Throws()
        {
            Assert.Throws<System.ArgumentNullException>(() => VerbalExpressions.DefaultExpression.Add((VerbalExpressions)null));
        }

        #endregion Grouping and alternation

        #region Lookaround and options

        [Test]
        public void Not_IsAliasForNegativeLookAhead()
        {
            var verbEx = Exact().Not("foo").Word().EndOfLine();

            Assert.AreEqual(@"^(?!foo)\w+$", verbEx.ToString());
            Assert.IsTrue(verbEx.IsMatch("bar"));
            Assert.IsFalse(verbEx.IsMatch("foobar"));
        }

        [Test]
        public void SearchOneLine_RemovesMultilineOption()
        {
            var verbEx = VerbalExpressions.DefaultExpression.SearchOneLine();

            Assert.AreEqual(RegexOptions.None, verbEx.ToRegex().Options & RegexOptions.Multiline);
        }

        [Test]
        public void SearchOneLine_False_KeepsMultilineOption()
        {
            var verbEx = VerbalExpressions.DefaultExpression.SearchOneLine(false);

            Assert.AreEqual(RegexOptions.Multiline, verbEx.ToRegex().Options & RegexOptions.Multiline);
        }

        #endregion Lookaround and options

        #region Matching helpers

        [Test]
        public void TestExact_RequiresWholeInputToMatch()
        {
            var verbEx = VerbalExpressions.DefaultExpression.Then("abc").Or("def");

            Assert.IsTrue(verbEx.Test("defzzz"));
            Assert.IsFalse(verbEx.TestExact("defzzz"));
            Assert.IsTrue(verbEx.TestExact("def"));
        }

        [Test]
        public void TestExact_AlternationWherePrefixMatchesFirst_StillMatchesWholeInput()
        {
            var verbEx = VerbalExpressions.DefaultExpression.Then("a").Or("ab");

            Assert.IsTrue(verbEx.TestExact("ab"));
        }

        [Test]
        public void TestExact_DoesNotMatchAcrossLines()
        {
            var verbEx = VerbalExpressions.DefaultExpression.Then("abc");

            Assert.IsFalse(verbEx.TestExact("abc\nabc"));
        }

        [Test]
        public void TestExact_Null_ReturnsFalse()
        {
            Assert.IsFalse(VerbalExpressions.DefaultExpression.Then("a").TestExact(null));
        }

        [Test]
        public void Replace_WithInputAndReplacement_ReplacesAllMatches()
        {
            var result = VerbalExpressions.DefaultExpression.Find("red").Replace("A red car and a red house", "blue");

            Assert.AreEqual("A blue car and a blue house", result);
        }

        #endregion Matching helpers
    }
}
