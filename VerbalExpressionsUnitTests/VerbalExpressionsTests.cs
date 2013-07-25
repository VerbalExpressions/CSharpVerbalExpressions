using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerbalExpression.Net;

namespace VerbalExpressionsUnitTests
{
	[TestClass]
	public class VerbalExpressionsTests
	{
		[TestMethod]
		public void TestingIfWeHaveAValidURL()
		{
			var tester = new VerbalExpressions()
						.StartOfLine()
						.Then( "http" )
						.Maybe( "s" )
						.Then( "://" )
						.Maybe( "www." )
						.AnythingBut( " " )
						.EndOfLine();

			var testMe = "https://www.google.com";

			// Use RegExp object's native test() function
			Assert.IsTrue(tester.Test( testMe ), "The URL is incorrect");

			Console.WriteLine("We have a correct URL ");
		}
	}
}
