using CSharpVerbalExpressions;
using Xunit;

namespace VerbalExpressionsUnitTests
{
	public class ThenTests
	{
		[Fact]
		public void Then_VerbalExpressionsEmail_DoesMatchEmail()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.StartOfLine().Then(CommonRegex.Email);

			var isMatch = verbEx.IsMatch("test@github.com");
			Assert.True(isMatch, "Should match email address");
		}

		[Fact]
		public void Then_VerbalExpressionsEmail_DoesNotMatchUrl()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.StartOfLine().Then(CommonRegex.Email);

			var isMatch = verbEx.IsMatch("http://www.google.com");
			Assert.False(isMatch, "Should not match url address");
		}

		[Fact]
		public void Then_VerbalExpressionsUrl_DoesMatchUrl()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.StartOfLine()
				  .Then(CommonRegex.Url);

			Assert.True(verbEx.IsMatch("http://www.google.com"), "Should match url address");
			Assert.True(verbEx.IsMatch("https://www.google.com"), "Should match url address");
			Assert.True(verbEx.IsMatch("http://google.com"), "Should match url address");
		}

		[Fact]
		public void Then_VerbalExpressionsUrl_DoesNotMatchEmail()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.StartOfLine().Then(CommonRegex.Url);

			Assert.False(verbEx.IsMatch("test@github.com"), "Should not match email address");
		}
	}
}