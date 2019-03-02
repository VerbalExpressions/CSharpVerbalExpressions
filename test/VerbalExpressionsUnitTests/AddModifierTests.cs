using System;
using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{

    [TestFixture]
    public class AddModifierTests
    {
        [Test]
        public void AddModifier_AddModifierI_RemovesCase()
        {
            VerbalExpressions verbEx = VerbalExpressions.DefaultExpression;
            verbEx.Add("teststring").AddModifier('i');

            Assert.IsTrue(verbEx.IsMatch("TESTSTRING"));
        }

        [Test]
        public void AddModifier_AddModifierM_Multiline()
        {
            //Arrange
            VerbalExpressions verbEx = VerbalExpressions.DefaultExpression;
            string text = string.Format("testin with {0} line break", Environment.NewLine);

            //Act
            verbEx.AddModifier('m');

            //Assert
            Assert.IsTrue(verbEx.Test(text));
        }

        [Test]
        public void AddModifier_AddModifierS_SingleLine()
        {
            //Arrange
            VerbalExpressions verbEx = VerbalExpressions.DefaultExpression;
            string testString = "First string" + Environment.NewLine + "Second string";

            //Act
            verbEx.Add("First string").Anything().Then("Second string");

            //Assert
            Assert.IsFalse(
                           verbEx.IsMatch(testString),
                "The dot matches a single character, except line break characters.");

            verbEx.AddModifier('s');
            Assert.IsTrue(
                          verbEx.IsMatch(testString),
                "The dot matches a single character and line break characters.");
        }

        [Test]
        public void AddModifier_AddModifierX_IgnoreWhitspace()
        {
            VerbalExpressions verbEx = VerbalExpressions.DefaultExpression;
            verbEx.Add("test string").AddModifier('x');

            Assert.IsTrue(verbEx.IsMatch("test string #comment"));
        }
    }

}