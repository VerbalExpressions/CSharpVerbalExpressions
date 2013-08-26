using CSharpVerbalExpressions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    class SanatizeTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Sanitize_Handles_Null_String()
        {
            //Arrange
            var verbEx = VerbalExpressions.DefaultExpression;
            string value = null;

            //Act
            //Assert
            value = verbEx.Sanitize(value);
        }

        [Test]
        public void Sanitize_AddCharactersThatShouldBeEscaped_ReturnsEscapedString()
        {
            //Arrange
            var verbEx = VerbalExpressions.DefaultExpression;
            string value = "*+?";
            string result = string.Empty;
            string expected = @"\*\+\?";

            //Act
            result = verbEx.Sanitize(value);

            //Assert
            Assert.AreEqual(expected, result);
        }
    }
}
