using System;
using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{

    [TestFixture]
    public class SomethingTests
    {
        [Test]
        public void Something_EmptyStringAsParameter_DoesNotMatch()
        {
            // Arange
            VerbalExpressions verbEx = VerbalExpressions.DefaultExpression.Something();
            string testString = string.Empty;

            // Act
            bool isMatch = verbEx.IsMatch(testString);

            // Assert
            Assert.IsFalse(isMatch, "Test string should be empty.");
        }

        [Test]
        public void Something_NullAsParameter_Throws()
        {
            // Arange
            VerbalExpressions verbEx = VerbalExpressions.DefaultExpression.Something();
            string testString = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => verbEx.IsMatch(testString));
        }

        [Test]
        public void Something_SomeStringAsParameter_DoesMatch()
        {
            // Arange
            VerbalExpressions verbEx = VerbalExpressions.DefaultExpression.Something();
            const string TEST_STRING = "Test string";

            // Act
            bool isMatch = verbEx.IsMatch(TEST_STRING);

            // Assert
            Assert.IsTrue(isMatch, "Test string should not be empty.");
        }
    }

}