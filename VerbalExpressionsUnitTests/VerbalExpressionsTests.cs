using System;
using System.Text.RegularExpressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerbalExpression.Net;

namespace VerbalExpressionsUnitTests
{
	[TestClass]
	public class VerbalExpressionsTests
	{

		private VerbalExpressions verbEx;

		[TestInitialize]
		public void Initialize()
		{
			verbEx = new VerbalExpressions();
		}

		[TestMethod]
		public void TestingIfWeHaveAValidURL()
		{
			verbEx = new VerbalExpressions()
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
		public void EndOfLine_EndsWithDotCom_DoesMatchDotComInEnd()
		{
			verbEx.Add(".com")
				.EndOfLine();

			var isMatch = verbEx.IsMatch("www.google.com");
			Assert.IsTrue(isMatch, "Should match '.com' in end");
		}
	}
}
