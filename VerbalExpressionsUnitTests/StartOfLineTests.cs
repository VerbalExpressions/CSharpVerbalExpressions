using CSharpVerbalExpressions;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    public class StartOfLineTests
    {
        [Test]
        public void StartOfLine_CreatesCorrectRegex()
        {
            var verbEx = VerbalExpressions.DefaultExpression;
            verbEx.StartOfLine();
            Assert.AreEqual("^", verbEx.ToString(), "missing start of line regex");
        }

        [Test]
        public void StartOfLine_WhenPlacedInRandomCallOrder_ShouldAppendAtTheBeginningOfTheExpression()
        {
            var verbEx = VerbalExpressions.DefaultExpression;
            verbEx.Add("test")
                .Add("ing")
                .StartOfLine();

            string text = "testing1234";
            Assert.IsTrue(verbEx.IsMatch(text), "Should match that the text starts with test");
        }

        [Test]
        public void StartOfLine_ThenHttpMaybeWww_DoesMatchHttpInStart()
        {
            var verbEx = VerbalExpressions.DefaultExpression;
            verbEx.StartOfLine()
                .Then("http")
                .Maybe("www");

            var isMatch = Regex.IsMatch("http", verbEx.ToString());
            Assert.IsTrue(isMatch, "Should match http in start");
        }

        [Test]
        public void StartOfLine_ThenHttpMaybeWww_DoesNotMatchWwwInStart()
        {
            var verbEx = VerbalExpressions.DefaultExpression;
            verbEx.StartOfLine()
                .Then("http")
                .Maybe("www");

            var isMatch = verbEx.IsMatch("www");
            Assert.IsFalse(isMatch, "Should not match www in start");
        }
    }
}
