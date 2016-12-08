using System;
using CSharpVerbalExpressions;
using Xunit;

namespace VerbalExpressionsUnitTests
{
	public class SomethingTests
	{
		[Fact]
		public void Something_EmptyStringAsParameter_DoesNotMatch()
		{
			// Arange
			VerbalExpressions verbEx = VerbalExpressions.DefaultExpression.Something();
			string testString = string.Empty;

			// Act
			bool isMatch = verbEx.IsMatch(testString);

			// Assert
			Assert.False(isMatch, "Test string should be empty.");
		}

		[Fact]
		public void Something_NullAsParameter_Throws()
		{
			// Arange
			VerbalExpressions verbEx = VerbalExpressions.DefaultExpression.Something();
			string testString = null;

			// Act and Assert
			Assert.Throws<ArgumentNullException>(() => verbEx.IsMatch(testString));
		}

		[Fact]
		public void Something_SomeStringAsParameter_DoesMatch()
		{
			// Arange
			VerbalExpressions verbEx = VerbalExpressions.DefaultExpression.Something();
			const string TEST_STRING = "Test string";

			// Act
			bool isMatch = verbEx.IsMatch(TEST_STRING);

			// Assert
			Assert.True(isMatch, "Test string should not be empty.");
		}
	}
}