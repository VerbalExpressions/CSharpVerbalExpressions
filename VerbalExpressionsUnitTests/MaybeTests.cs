using CSharpVerbalExpressions;
using Xunit;

namespace VerbalExpressionsUnitTests
{
	public class MaybeTests
	{
		[Fact]
		public void Maybe_WhenCalled_UsesCommonRegexUrl()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.Maybe(CommonRegex.Url);

			Assert.True(verbEx.IsMatch("http://www.google.com"), "Should match url address");
		}

		[Fact]
		public void Maybe_WhenCalled_UsesCommonRegexEmail()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.Maybe(CommonRegex.Email);

			Assert.True(verbEx.IsMatch("test@github.com"), "Should match email address");
		}
	}
}