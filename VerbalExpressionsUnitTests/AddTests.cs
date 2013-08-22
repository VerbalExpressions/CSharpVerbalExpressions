using CSharpVerbalExpressions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    class AddTests
    {
        VerbalExpressions verbEx = null;

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_WhenNullStringPassedAsParameter_ShouldThrowNullArgumentException()
        {
            //Arrange
            verbEx = VerbalExpressions.DefaultExpression;
            string value = null;

            //Act
            //Assert
            verbEx.Add(value);
        }

        [Test]
        public void Add_AddDotCom_DoesNotMatchGoogleComWithoutDot()
        {
            verbEx = VerbalExpressions.DefaultExpression;
            verbEx.Add(".com");

            var isMatch = verbEx.IsMatch("http://www.googlecom/");
            Assert.IsFalse(isMatch, "Should not match 'ecom'");
        }
        
        [Test]
        public void AddModifier_AddModifierI_RemovesCase()
        {
            verbEx.Add("teststring")
                .AddModifier('i');

            Assert.IsTrue(verbEx.IsMatch("TESTSTRING"));

        }

        [Test]
        public void AddModifier_AddModifierX_IgnoreWhitspace()
        {
            verbEx.Add("test string")
                  .AddModifier('x');

            Assert.IsTrue(verbEx.IsMatch("test string #comment"));
        }

        [Test]
        public void AddModifier_AddModifierM_Multiline()
        {
            //Arrange
            string text = string.Format("testin with {0} line break", Environment.NewLine);

            //Act
            verbEx.AddModifier('m');

            //Assert
            Assert.IsTrue(verbEx.Test(text));
        }


    }
}
