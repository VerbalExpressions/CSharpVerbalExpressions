using System;
using System.Text.RegularExpressions;
using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    public class VerbalExpressionsTests
    {
        [Test]
        public void TestingIfWeHaveAValidURL()
        {
            var verbEx = VerbalExpressions.DefaultExpression
                        .StartOfLine()
                        .Then("http")
                        .Maybe("s")
                        .Then("://")
                        .Maybe("www.")
                        .AnythingBut(" ")
                        .EndOfLine();

            var testMe = "https://www.google.com";

            Assert.IsTrue(verbEx.Test(testMe), "The URL is incorrect");
        }

        [Test]
        public void Anything_StartOfLineAnythingEndOfline_DoesMatchAnyThing()
        {
            var verbEx = VerbalExpressions.DefaultExpression
                .StartOfLine()
                .Anything()
                .EndOfLine();


            var isMatch = verbEx.IsMatch("'!@#$%¨&*()__+{}'");
            Assert.IsTrue(isMatch, "Ooops, should match anything");
        }

        [Test]
        public void Replace_WhenCalledImmediatelyAfteInitialize_ShouldNotThrowNullReferenceException()
        {
            //Arrange
            var verbEx = VerbalExpressions.DefaultExpression;
            string value = "value";
            bool hasThrownNullReferenceEx = false;

            //Act
            try
            {
                verbEx.Replace(value);
            }
            catch (NullReferenceException)
            {
                hasThrownNullReferenceEx = true;
            }

            //Assert
            Assert.IsFalse(hasThrownNullReferenceEx);
        }

        [Test]
        public void AnyOf_WhenValueParameterIsNullOrEmpty_ShouldThrowArgumentException()
        {
            //Arrange
            var verbEx = VerbalExpressions.DefaultExpression;
            string value = null;

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => verbEx.AnyOf(value));
        }

        [Test]
        public void Any_WhenValueParameterIsNullOrEmpty_ShouldThrowArgumentException()
        {
            //Arrange
            var verbEx = VerbalExpressions.DefaultExpression;
            string value = null;

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => verbEx.Any(value));
        }

        [Test]
        public void Find_WhenNullParameterValueIsPassed_ThrowsArgumentException()
        {
            //Arrange
            var verbEx = VerbalExpressions.DefaultExpression;
            string value = null;

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => verbEx.Find(value));
        }

        [Test]
        public void LineBreak_WhenCalled_ReturnsExpectedExpression()
        {
            //Arrange
            var verbEx = VerbalExpressions.DefaultExpression;
            string text = string.Format("testin with {0} line break", Environment.NewLine);

            //Act
            verbEx.LineBreak();
            //Assert
            Assert.IsTrue(verbEx.Test(text));
        }

        [Test]
        public void Br_WhenCalled_ReturnsExpectedExpression()
        {
            //Arrange
            var verbEx = VerbalExpressions.DefaultExpression;
            string text = string.Format("testin with {0} line break", Environment.NewLine);

            //Act
            verbEx.Br();
            //Assert
            Assert.IsTrue(verbEx.Test(text));
        }

        [Test]
        public void Tab_WhenCalled_ReturnsExpectedExpression()
        {
            //Arrange
            var verbEx = VerbalExpressions.DefaultExpression;
            string text = string.Format("text that contains {0} a tab", @"\t");

            //Act
            verbEx.Tab();

            //Assert
            Assert.IsTrue(verbEx.Test(text));
        }

        [Test]
        public void Word_WhenCalled_ReturnsExpectedNumberOfWords()
        {
            //Arrange
            var verbEx = VerbalExpressions.DefaultExpression;
            string text = "three words here";
            int expectedCount = 3;

            //Act
            verbEx.Word();
            Regex currentExpression = verbEx.ToRegex();
            int result = currentExpression.Matches(text).Count;

            //Assert
            Assert.AreEqual(expectedCount, result);
        }

        [Test]
        public void UseOneLineSearchOption_WhenCalled_ShouldChangeMultilineModifier()
        {
            //Arrange
            var verbEx = VerbalExpressions.DefaultExpression;
            verbEx.UseOneLineSearchOption(false);
            var regex = verbEx.ToRegex();
            Assert.IsTrue(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should now be present");
            //Act
            verbEx.UseOneLineSearchOption(true);
            //Assert
            regex = verbEx.ToRegex();
            Assert.IsFalse(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should now have been removed");
        }
    }
}
