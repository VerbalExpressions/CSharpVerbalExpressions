using System;
using CSharpVerbalExpressions;
using Xunit;

namespace VerbalExpressionsUnitTests
{
	public class SanatizeTests
	{
		[Fact]
		public void Sanitize_Handles_Null_String()
		{
			//Arrange
			var verbEx = VerbalExpressions.DefaultExpression;
			string value = null;

			//Act
			//Assert
			Assert.Throws<ArgumentNullException>(() => verbEx.Sanitize(value));
		}

		[Fact]
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
			Assert.Equal(expected, result);
		}
	}
}