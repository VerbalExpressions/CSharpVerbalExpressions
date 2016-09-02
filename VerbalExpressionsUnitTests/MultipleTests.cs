using System;
using CSharpVerbalExpressions;
using Xunit;

namespace VerbalExpressionsUnitTests
{
	public class MultipleTests
	{
		[Fact]
		public void Multiple_WhenNullOrEmptyValueParameterIsPassed_ShouldThrowArgumentException()
		{
			//Arrange
			var verbEx = VerbalExpressions.DefaultExpression;
			string value = null;

			//Act
			//Assert
			Assert.Throws<ArgumentNullException>(() => verbEx.Multiple(value));
		}

		[Fact]
		public void Multiple_WhenParamIsGiven_ShouldMatchOneOrMultipleValuesGiven()
		{
			//Arrange
			var verbEx = VerbalExpressions.DefaultExpression;
			string text = "testesting 123 yahoahoahou another test";
			string expectedExpression = "y(aho)+u";
			//Act
			verbEx.Add("y")
				.Multiple("aho")
				.Add("u");

			//Assert
			Assert.True(verbEx.Test(text));
			Assert.Equal(expectedExpression, verbEx.ToString());
		}

		[Fact]
		public void Multiple_WhenNullArgumentPassed_ThrowsArgumentNullException()
		{
			//Arrange
			var verbEx = VerbalExpressions.DefaultExpression;
			string argument = string.Empty;

			//Act
			//Assert
			Assert.Throws<ArgumentNullException>(() => verbEx.Multiple(argument));
		}
	}
}