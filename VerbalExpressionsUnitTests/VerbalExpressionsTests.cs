using System;
using System.Text.RegularExpressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerbalExpression.Net;

namespace VerbalExpressionsUnitTests
{
	[TestClass]
	public class VerbalExpressionsTests
	{

		private VerbalExpressions verbEx = VerbalExpressions.NewExpression;

		[TestInitialize]
		public void Initialize()
		{
			verbEx = VerbalExpressions.NewExpression;
		}

		[TestMethod]
		public void TestingIfWeHaveAValidURL()
		{
			verbEx = VerbalExpressions.NewExpression
						.StartOfLine()
						.Then( "http" )
						.Maybe( "s" )
						.Then( "://" )
						.Maybe( "www." )
						.AnythingBut( " " )
						.EndOfLine();

			var testMe = "https://www.google.com";

			Assert.IsTrue(verbEx.Test( testMe ), "The URL is incorrect");

			Console.WriteLine("We have a correct URL ");
		}
	
		[TestMethod]
		public void StartOfLine_CreatesCorrectRegex()
		{
			verbEx.StartOfLine();
			Assert.AreEqual("^", verbEx.ToString(), "missing start of line regex");
		}
	
		[TestMethod]
		public void StartOfLine_ThenHttpMaybeWww_DoesMatchHttpInStart()
		{
			verbEx.StartOfLine()
				.Then("http")
				.Maybe("www");

			var isMatch = Regex.IsMatch("http", verbEx.ToString());
			Assert.IsTrue(isMatch, "Should match http in start");
		}
	
		[TestMethod]
		public void StartOfLine_ThenHttpMaybeWww_DoesNotMatchWwwInStart()
		{
			verbEx.StartOfLine()
				.Then("http")
				.Maybe("www");

			var isMatch = verbEx.IsMatch("www");
			Assert.IsFalse(isMatch, "Should not match www in start");
		}
	
		[TestMethod]
		public void EndOfLine_AddDotComtEndOfLine_DoesMatchDotComInEnd()
		{
			verbEx.Add(".com")
				.EndOfLine();

			var isMatch = verbEx.IsMatch("www.google.com");
			Assert.IsTrue(isMatch, "Should match '.com' in end");
		}
	
		[TestMethod]
		public void EndOfLine_AddDotComEndOfLine_DoesNotMatchSlashInEnd()
		{
			verbEx.Add(".com")
				.EndOfLine();

			var isMatch = verbEx.IsMatch("http://www.google.com/");
			Assert.IsFalse(isMatch, "Should not match '/' in end");
		}
	
		[TestMethod]
		public void Anything_StartOfLineAnythingEndOfline_DoesMatchAnyThing()
		{
			verbEx
				.StartOfLine()
				.Anything()
				.EndOfLine();
			
			var isMatch = verbEx.IsMatch("'!@#$%¨&*()__+{}'");
			Assert.IsTrue(isMatch, "Ooops, should match anything");
		}
	
		[TestMethod]
		public void WithAnyCase_AddwwwWithAnyCase_DoesMatchwWw()
		{
			verbEx.Add("www")
				.WithAnyCase();
			
			var isMatch = verbEx.IsMatch("wWw");
			Assert.IsTrue(isMatch, "Should match any case");
		}
	
		[TestMethod]
		public void WithAnyCase_SetsCorrectIgnoreCaseRegexOptionAndHasMultiLineRegexOptionAsDefault()
		{
			verbEx.WithAnyCase();
			
			var regex = verbEx.ToRegex();
			Assert.IsTrue(regex.Options.HasFlag(RegexOptions.IgnoreCase), "RegexOptions should have ignoreCase");
			Assert.IsTrue(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should have MultiLine as default");
		}
	
		[TestMethod]
		public void RemoveModifier_RemoveModifierM_RemovesMulitilineAsDefault()
		{
			var regex = verbEx.ToRegex();
			Assert.IsTrue(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should have MultiLine as default");

			verbEx.RemoveModifier('m');
			regex = verbEx.ToRegex();

			Assert.IsFalse(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should now have been removed");
		}
	
		[TestMethod]
		public void WithAnyCase_AddwwwWithAnyCaseFalse_DoesNotMatchwWw()
		{
			verbEx.Add("www")
				.WithAnyCase(false);
			
			var isMatch = verbEx.IsMatch("wWw");
			Assert.IsFalse(isMatch, "Should not match any case");
		}

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Sanitize_Handles_Null_String()
    {
      //Arrange
      string value = null;

      //Act
      //Assert
      value = verbEx.Sanitize(value);
    }

    [TestMethod]
    public void Sanitize_AddCharactersThatShouldBeEscaped_ReturnsEscapedString()
    {
      //Arrange
      string value = "*+?";
      string result = string.Empty;
      string expected = @"\*\+\?";

      //Act
      result = verbEx.Sanitize(value);

      //Assert
      Assert.AreEqual<string>(expected, result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Add_WhenNullStringPassedAsParameter_ShouldThrowNullArgumentException()
    {
      //Arrange
      string value = null;

      //Act
      //Assert
      verbEx.Add(value);      
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Range_WhenNullParameterPassed_ShouldThrowArgumentNullException()
    {
      //Arrange
      object[] value = null;

      //Act
      //Assert
      verbEx.Range(value);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Range_WhenArrayParameterHasOnlyOneValue_ShouldThrowArgumentOutOfRangeException()
    {
      //Arrange
      object[] value = new object[1] { 0 };

      //Act
      //Assert
      verbEx.Range(value);
    }

    [TestMethod]
    public void Range_WhenArrayParameterHasValuesInReverseOrder_ReturnsCorrectResultForCorrectOrder()
    {
      object[] inversedOrderArray = new object[2] { 9, 2 };
      verbEx.Range(inversedOrderArray);
      string lookupString = "testing 8 another test";

      bool isMatch = verbEx.IsMatch(lookupString);
      Assert.IsTrue(isMatch);
    }
	}
}
