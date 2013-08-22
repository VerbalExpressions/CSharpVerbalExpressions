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
    class MultipleTests
    {
        private VerbalExpressions verbEx = null;

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Multiple_WhenNullOrEmptyValueParameterIsPassed_ShouldThrowArgumentException()
        {
            //Arrange
            verbEx = VerbalExpressions.DefaultExpression;
            string value = null;

            //Act
            //Assert
            verbEx.Multiple(value);
        }

        [Test]
        public void Multiple_WhenParamIsGiven_ShouldMatchOneOrMultipleValuesGiven()
        {
            //Arrange
            verbEx = VerbalExpressions.DefaultExpression;
            string text = "testesting 123 yahoahoahou another test";
            string expectedExpression = "y(aho)+u";
            //Act
            verbEx.Add("y")
                .Multiple("aho")
                .Add("u");

            //Assert
            Assert.IsTrue(verbEx.Test(text));
            Assert.AreEqual(expectedExpression, verbEx.ToString());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Multiple_WhenNullArgumentPassed_ThrowsArgumentNullException()
        {
            //Arrange
            verbEx = VerbalExpressions.DefaultExpression;
            string argument = string.Empty;

            //Act
            //Assert
            verbEx.Multiple(argument);
        }
    
    }
}
