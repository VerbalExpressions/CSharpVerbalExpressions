using System;
using CSharpVerbalExpressions;
using Xunit;

namespace VerbalExpressionsUnitTests
{
	public class AddTests
	{
		[Fact]
		public void Add_WhenNullStringPassedAsParameter_ShouldThrowNullArgumentException()
		{
			//Arrange
			var verbEx = VerbalExpressions.DefaultExpression;
			string value = null;

			//Act
			//Assert
			Assert.Throws<ArgumentNullException>(() => verbEx.Add(value));
		}

		[Fact]
		public void Add_AddDotCom_DoesNotMatchGoogleComWithoutDot()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.Add(".com");

			var isMatch = verbEx.IsMatch("http://www.googlecom/");
			Assert.False(isMatch, "Should not match 'ecom'");
		}
	}
}