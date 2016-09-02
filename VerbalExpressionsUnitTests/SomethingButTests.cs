using System;
using CSharpVerbalExpressions;
using Xunit;

namespace VerbalExpressionsUnitTests
{
	public class SomethingButTests
	{
		[Fact]
		public void SomethingBut_EmptyStringAsParameter_DoesNotMatch()
		{
			// Arange
			VerbalExpressions verbEx = VerbalExpressions.DefaultExpression.SomethingBut("Test");
			string testString = string.Empty;

			// Act
			bool isMatch = verbEx.IsMatch(testString);

			// Assert
			Assert.False(isMatch, "Test string should be empty.");
		}

		[Fact]
		public void SomethingBut_NullAsParameter_Throws()
		{
			// Arange
			VerbalExpressions verbEx = VerbalExpressions.DefaultExpression.SomethingBut("Test");
			string testString = null;

			// Act and Assert
			Assert.Throws<ArgumentNullException>(() => verbEx.IsMatch(testString));
		}

		[Fact]
		public void SomethingBut_TestStringStartsCorrect_DoesMatch()
		{
			// Arange
			const string START_STRING = "Test";
			VerbalExpressions verbEx = VerbalExpressions.DefaultExpression.SomethingBut(START_STRING);
			const string TEST_STRING = "Test string";

			// Act
			bool isMatch = verbEx.IsMatch(TEST_STRING);

			// Assert
			Assert.True(isMatch, "Test string should not be empty and starts with \"" + START_STRING + "\".");
		}

		[Fact]
		public void SomethingBut_TestStringStartsIncorrect_DoesNotMatch()
		{
			// Arange
			const string START_STRING = "Test";
			VerbalExpressions verbEx = VerbalExpressions.DefaultExpression.SomethingBut(START_STRING);
			const string TEST_STRING = "string";

			// Act
			bool isMatch = verbEx.IsMatch(TEST_STRING);

			// Assert
			Assert.True(isMatch, "Test string starts with \"" + START_STRING + "\".");
		}
	}
}