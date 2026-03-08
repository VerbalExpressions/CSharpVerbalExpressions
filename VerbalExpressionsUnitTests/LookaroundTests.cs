using System.Text.RegularExpressions;
using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    public class LookaroundTests
    {
        [Test]
        public void LookAhead_WhenCalled_MatchesWithoutConsuming()
        {
            // Match digits followed by "px" without consuming "px"
            var verbEx = VerbalExpressions.DefaultExpression
                .Add(@"\d+", false)
                .LookAhead("px");

            var regex = verbEx.ToRegex();
            var match = regex.Match("width: 100px");

            Assert.IsTrue(match.Success);
            Assert.AreEqual("100", match.Value);
        }

        [Test]
        public void NegativeLookAhead_WhenCalled_MatchesWhenNotFollowedBy()
        {
            // Match "foo" NOT followed by "bar"
            var verbEx = VerbalExpressions.DefaultExpression
                .Then("foo")
                .NegativeLookAhead("bar");

            Assert.IsTrue(verbEx.IsMatch("foobaz"));
            Assert.IsFalse(verbEx.IsMatch("foobar"));
        }

        [Test]
        public void LookBehind_WhenCalled_MatchesPrecededBy()
        {
            // Match digits preceded by "$"
            var verbEx = VerbalExpressions.DefaultExpression
                .LookBehind(@"\$", false)
                .Add(@"\d+", false);

            var regex = verbEx.ToRegex();
            var match = regex.Match("price: $100");

            Assert.IsTrue(match.Success);
            Assert.AreEqual("100", match.Value);
        }

        [Test]
        public void NegativeLookBehind_WhenCalled_MatchesNotPrecededBy()
        {
            // Match digits NOT preceded by "$"
            var verbEx = VerbalExpressions.DefaultExpression
                .NegativeLookBehind(@"\$", false)
                .Add(@"\d+", false);

            Assert.IsTrue(verbEx.IsMatch("count: 100"));
        }

        [Test]
        public void LookAhead_WithSanitize_EscapesSpecialCharacters()
        {
            var verbEx = VerbalExpressions.DefaultExpression
                .Add(@"\w+", false)
                .LookAhead(".");

            Assert.AreEqual(@"\w+(?=\.)", verbEx.ToString());
        }

        [Test]
        public void LookBehind_WithSanitize_EscapesSpecialCharacters()
        {
            var verbEx = VerbalExpressions.DefaultExpression
                .LookBehind(".")
                .Add(@"\w+", false);

            Assert.AreEqual(@"(?<=\.)\w+", verbEx.ToString());
        }

        [Test]
        public void LookAhead_CombinedWithLookBehind_WorksTogether()
        {
            // Match content between parentheses without including them
            var verbEx = VerbalExpressions.DefaultExpression
                .LookBehind(@"\(", false)
                .Add(@"\w+", false)
                .LookAhead(@"\)", false);

            var regex = verbEx.ToRegex();
            var match = regex.Match("say (hello) world");

            Assert.IsTrue(match.Success);
            Assert.AreEqual("hello", match.Value);
        }
    }
}
