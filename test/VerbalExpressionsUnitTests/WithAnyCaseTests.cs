using System.Text.RegularExpressions;
using CSharpVerbalExpressions;
using Xunit;

namespace VerbalExpressionsUnitTests
{
	public class WithAnyCaseTests
	{
		[Fact]
		public void WithAnyCase_AddwwwWithAnyCase_DoesMatchwWw()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.Add("www")
				.WithAnyCase();

			var isMatch = verbEx.IsMatch("wWw");
			Assert.True(isMatch, "Should match any case");
		}

		[Fact]
		public void WithAnyCase_SetsCorrectIgnoreCaseRegexOptionAndHasMultiLineRegexOptionAsDefault()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.WithAnyCase();

			var regex = verbEx.ToRegex();
			Assert.True(regex.Options.HasFlag(RegexOptions.IgnoreCase), "RegexOptions should have ignoreCase");
			Assert.True(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should have MultiLine as default");
		}

		[Fact]
		public void WithAnyCase_AddwwwWithAnyCaseFalse_DoesNotMatchwWw()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.Add("www")
				.WithAnyCase(false);

			var isMatch = verbEx.IsMatch("wWw");
			Assert.False(isMatch, "Should not match any case");
		}
	}
}