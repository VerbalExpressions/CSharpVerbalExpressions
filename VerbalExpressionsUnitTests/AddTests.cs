using System;
using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{
	[TestFixture]
	public class AddTests
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Add_WhenNullStringPassedAsParameter_ShouldThrowNullArgumentException()
		{
			//Arrange
			var verbEx = VerbalExpressions.DefaultExpression;
			string value = null;

			//Act
			//Assert
			verbEx.Add(value);
		}

		[Test]
		public void Add_AddDotCom_DoesNotMatchGoogleComWithoutDot()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.Add(".com");

			var isMatch = verbEx.IsMatch("http://www.googlecom/");
			Assert.IsFalse(isMatch, "Should not match 'ecom'");
		}

		[Test]
		public void AddModifier_AddModifierI_RemovesCase()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.Add("teststring").AddModifier('i');

			Assert.IsTrue(verbEx.IsMatch("TESTSTRING"));
		}

		[Test]
		public void AddModifier_AddModifierX_IgnoreWhitspace()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.Add("test string").AddModifier('x');

			Assert.IsTrue(verbEx.IsMatch("test string #comment"));
		}

		[Test]
		public void AddModifier_AddModifierM_Multiline()
		{
			//Arrange
			var verbEx = VerbalExpressions.DefaultExpression;
			string text = string.Format("testin with {0} line break", Environment.NewLine);

			//Act
			verbEx.AddModifier('m');

			//Assert
			Assert.IsTrue(verbEx.Test(text));
		}
	}
}
