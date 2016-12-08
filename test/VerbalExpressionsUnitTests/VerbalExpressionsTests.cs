using System;
using System.Text.RegularExpressions;
using CSharpVerbalExpressions;
using Xunit;

namespace VerbalExpressionsUnitTests
{
	public class VerbalExpressionsTests
	{
		[Fact]
		public void TestingIfWeHaveAValidURL()
		{
			var verbEx = VerbalExpressions.DefaultExpression
						.StartOfLine()
						.Then("http")
						.Maybe("s")
						.Then("://")
						.Maybe("www.")
						.AnythingBut(" ")
						.EndOfLine();

			var testMe = "https://www.google.com";

			Assert.True(verbEx.Test(testMe), "The URL is incorrect");
		}

		[Fact]
		public void Anything_StartOfLineAnythingEndOfline_DoesMatchAnyThing()
		{
			var verbEx = VerbalExpressions.DefaultExpression
				.StartOfLine()
				.Anything()
				.EndOfLine();

			var isMatch = verbEx.IsMatch("'!@#$%¨&*()__+{}'");
			Assert.True(isMatch, "Ooops, should match anything");
		}

		[Fact]
		public void Replace_WhenCalledImmediatelyAfteInitialize_ShouldNotThrowNullReferenceException()
		{
			//Arrange
			var verbEx = VerbalExpressions.DefaultExpression;
			string value = "value";
			bool hasThrownNullReferenceEx = false;

			//Act
			try
			{
				verbEx.Replace(value);
			}
			catch (NullReferenceException)
			{
				hasThrownNullReferenceEx = true;
			}

			//Assert
			Assert.False(hasThrownNullReferenceEx);
		}

		[Fact]
		public void AnyOf_WhenValueParameterIsNullOrEmpty_ShouldThrowArgumentException()
		{
			//Arrange
			var verbEx = VerbalExpressions.DefaultExpression;
			string value = null;

			//Act
			//Assert
			Assert.Throws<ArgumentNullException>(() => verbEx.AnyOf(value));
		}

		[Fact]
		public void Any_WhenValueParameterIsNullOrEmpty_ShouldThrowArgumentException()
		{
			//Arrange
			var verbEx = VerbalExpressions.DefaultExpression;
			string value = null;

			//Act
			//Assert
			Assert.Throws<ArgumentNullException>(() => verbEx.Any(value));
		}

		[Fact]
		public void Find_WhenNullParameterValueIsPassed_ThrowsArgumentException()
		{
			//Arrange
			var verbEx = VerbalExpressions.DefaultExpression;
			string value = null;

			//Act
			//Assert
			Assert.Throws<ArgumentNullException>(() => verbEx.Find(value));
		}

		[Fact]
		public void LineBreak_WhenCalled_ReturnsExpectedExpression()
		{
			//Arrange
			var verbEx = VerbalExpressions.DefaultExpression;
			string text = string.Format("testin with {0} line break", Environment.NewLine);

			//Act
			verbEx.LineBreak();
			//Assert
			Assert.True(verbEx.Test(text));
		}

		[Fact]
		public void Br_WhenCalled_ReturnsExpectedExpression()
		{
			//Arrange
			var verbEx = VerbalExpressions.DefaultExpression;
			string text = string.Format("testin with {0} line break", Environment.NewLine);

			//Act
			verbEx.Br();
			//Assert
			Assert.True(verbEx.Test(text));
		}

		[Fact]
		public void Tab_WhenCalled_ReturnsExpectedExpression()
		{
			//Arrange
			var verbEx = VerbalExpressions.DefaultExpression;
			string text = string.Format("text that contains {0} a tab", @"\t");

			//Act
			verbEx.Tab();

			//Assert
			Assert.True(verbEx.Test(text));
		}

		[Fact]
		public void Word_WhenCalled_ReturnsExpectedNumberOfWords()
		{
			//Arrange
			var verbEx = VerbalExpressions.DefaultExpression;
			string text = "three words here";
			int expectedCount = 3;

			//Act
			verbEx.Word();
			Regex currentExpression = verbEx.ToRegex();
			int result = currentExpression.Matches(text).Count;

			//Assert
			Assert.Equal(expectedCount, result);
		}

		[Fact]
		public void UseOneLineSearchOption_WhenCalled_ShouldChangeMultilineModifier()
		{
			//Arrange
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.UseOneLineSearchOption(false);
			var regex = verbEx.ToRegex();
			Assert.True(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should now be present");
			//Act
			verbEx.UseOneLineSearchOption(true);
			//Assert
			regex = verbEx.ToRegex();
			Assert.False(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should now have been removed");
		}
	}
}