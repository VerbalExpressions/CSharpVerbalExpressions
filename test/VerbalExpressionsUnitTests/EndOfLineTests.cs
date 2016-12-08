using CSharpVerbalExpressions;
using Xunit;

namespace VerbalExpressionsUnitTests
{
	public class EndOfLineTests
	{
		[Fact]
		public void EndOfLine_AddDotComtEndOfLine_DoesMatchDotComInEnd()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.Add(".com").EndOfLine();

			var isMatch = verbEx.IsMatch("www.google.com");
			Assert.True(isMatch, "Should match '.com' in end");
		}

		[Fact]
		public void EndOfLine_AddDotComEndOfLine_DoesNotMatchSlashInEnd()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.Add(".com").EndOfLine();

			var isMatch = verbEx.IsMatch("http://www.google.com/");
			Assert.False(isMatch, "Should not match '/' in end");
		}
	}
}