using System;
using CSharpVerbalExpressions;
using Xunit;

namespace VerbalExpressionsUnitTests
{
	public class AddModifierTests
	{
		[Fact]
		public void AddModifier_AddModifierI_RemovesCase()
		{
			VerbalExpressions verbEx = VerbalExpressions.DefaultExpression;
			verbEx.Add("teststring").AddModifier('i');

			Assert.True(verbEx.IsMatch("TESTSTRING"));
		}

		[Fact]
		public void AddModifier_AddModifierM_Multiline()
		{
			//Arrange
			VerbalExpressions verbEx = VerbalExpressions.DefaultExpression;
			string text = string.Format("testin with {0} line break", Environment.NewLine);

			//Act
			verbEx.AddModifier('m');

			//Assert
			Assert.True(verbEx.Test(text));
		}

		[Fact]
		public void AddModifier_AddModifierS_SingleLine()
		{
			//Arrange
			VerbalExpressions verbEx = VerbalExpressions.DefaultExpression;
			string testString = "First string" + Environment.NewLine + "Second string";

			//Act
			verbEx.Add("First string").Anything().Then("Second string");

			//Assert
			Assert.False(
						   verbEx.IsMatch(testString),
				"The dot matches a single character, except line break characters.");

			verbEx.AddModifier('s');
			Assert.True(
						  verbEx.IsMatch(testString),
				"The dot matches a single character and line break characters.");
		}

		[Fact]
		public void AddModifier_AddModifierX_IgnoreWhitspace()
		{
			VerbalExpressions verbEx = VerbalExpressions.DefaultExpression;
			verbEx.Add("test string").AddModifier('x');

			Assert.True(verbEx.IsMatch("test string #comment"));
		}
	}
}