using System.Text.RegularExpressions;
using CSharpVerbalExpressions;
using Xunit;

namespace VerbalExpressionsUnitTests
{
	public class StartOfLineTests
	{
		[Fact]
		public void StartOfLine_CreatesCorrectRegex()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.StartOfLine();
			Assert.Equal("^", verbEx.ToString());
		}

		[Fact]
		public void StartOfLine_WhenPlacedInRandomCallOrder_ShouldAppendAtTheBeginningOfTheExpression()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.Add("test")
				.Add("ing")
				.StartOfLine();

			string text = "testing1234";
			Assert.True(verbEx.IsMatch(text), "Should match that the text starts with test");
		}

		[Fact]
		public void StartOfLine_ThenHttpMaybeWww_DoesMatchHttpInStart()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.StartOfLine()
				.Then("http")
				.Maybe("www");

			var isMatch = Regex.IsMatch("http", verbEx.ToString());
			Assert.True(isMatch, "Should match http in start");
		}

		[Fact]
		public void StartOfLine_ThenHttpMaybeWww_DoesNotMatchWwwInStart()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.StartOfLine()
				.Then("http")
				.Maybe("www");

			var isMatch = verbEx.IsMatch("www");
			Assert.False(isMatch, "Should not match www in start");
		}
	}
}