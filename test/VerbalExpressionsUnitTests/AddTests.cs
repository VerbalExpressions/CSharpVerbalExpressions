using System;
using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    public class AddTests
    {
        [Test]
        public void Add_WhenNullStringPassedAsParameter_ShouldThrowNullArgumentException()
        {
            //Arrange
            var verbEx = VerbalExpressions.DefaultExpression;
            string value = null;

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => verbEx.Add(value));
        }

        [Test]
        public void Add_AddDotCom_DoesNotMatchGoogleComWithoutDot()
        {
            var verbEx = VerbalExpressions.DefaultExpression;
            verbEx.Add(".com");

            var isMatch = verbEx.IsMatch("http://www.googlecom/");
            Assert.IsFalse(isMatch, "Should not match 'ecom'");
        }
    }
}
