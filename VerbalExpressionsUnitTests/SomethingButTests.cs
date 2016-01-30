using System;
using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{

    [TestFixture]
    public class SomethingButTests
    {
        [Test]
        public void SomethingBut_EmptyStringAsParameter_DoesNotMatch()
        {
            // Arange
            VerbalExpressions verbEx = VerbalExpressions.DefaultExpression.SomethingBut("Test");
            string testString = string.Empty;

            // Act
            bool isMatch = verbEx.IsMatch(testString);

            // Assert
            Assert.IsFalse(isMatch, "Test string should be empty.");
        }

        [Test]
        public void SomethingBut_NullAsParameter_Throws()
        {
            // Arange
            VerbalExpressions verbEx = VerbalExpressions.DefaultExpression.SomethingBut("Test");
            string testString = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => verbEx.IsMatch(testString));
        }

        [Test]
        public void SomethingBut_TestStringStartsCorrect_DoesMatch()
        {
            // Arange
            const string START_STRING = "Test";
            VerbalExpressions verbEx = VerbalExpressions.DefaultExpression.SomethingBut(START_STRING);
            const string TEST_STRING = "Test string";

            // Act
            bool isMatch = verbEx.IsMatch(TEST_STRING);

            // Assert
            Assert.IsTrue(isMatch, "Test string should not be empty and starts with \"" + START_STRING + "\".");
        }

        [Test]
        public void SomethingBut_TestStringStartsIncorrect_DoesNotMatch()
        {
            // Arange
            const string START_STRING = "Test";
            VerbalExpressions verbEx = VerbalExpressions.DefaultExpression.SomethingBut(START_STRING);
            const string TEST_STRING = "string";

            // Act
            bool isMatch = verbEx.IsMatch(TEST_STRING);

            // Assert
            Assert.IsTrue(isMatch, "Test string starts with \"" + START_STRING + "\".");
        }
    }

}