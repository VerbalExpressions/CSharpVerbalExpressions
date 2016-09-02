using CSharpVerbalExpressions;
using Xunit;

namespace VerbalExpressionsUnitTests
{
	public class RepeatPreviousTests
	{
		[Fact]
		public void RepeatPrevious_WhenThreeA_RegesIsAsExpected()
		{
			// Arrange
			VerbalExpressions verbEx = VerbalExpressions.DefaultExpression;

			// Act
			verbEx.BeginCapture()
				  .Add("A")
				  .RepeatPrevious(3)
				  .EndCapture();

			// Assert
			Assert.Equal("(A{3})", verbEx.ToString());
		}

		[Fact]
		public void RepeatPrevious_WhenBetweenTwoAndFourA_RegesIsAsExpected()
		{
			// Arrange
			VerbalExpressions verbEx = VerbalExpressions.DefaultExpression;

			// Act
			verbEx.BeginCapture()
				  .Add("A")
				  .RepeatPrevious(2, 4)
				  .EndCapture();

			// Assert
			Assert.Equal("(A{2,4})", verbEx.ToString());
		}
	}
}