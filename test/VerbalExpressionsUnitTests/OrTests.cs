using System;
using CSharpVerbalExpressions;
using Xunit;

namespace VerbalExpressionsUnitTests
{
	public class OrTests
	{
		[Fact]
		public void Or_AddComOrOrg_DoesMatchComAndOrg()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.Add("com").Or("org");

			Console.WriteLine(verbEx);
			Assert.True(verbEx.IsMatch("org"), "Should match 'org'");
			Assert.True(verbEx.IsMatch("com"), "Should match 'com'");
		}

		[Fact]
		public void Or_AddComOrOrg_RegexIsAsExpecteds()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.Add("com").Or("org");

			Assert.Equal("(com)|(org)", verbEx.ToString());
		}

		[Fact]
		public void Or_VerbalExpressionsUrlOrVerbalExpressionEmail_DoesMatchEmailAndUrl()
		{
			var verbEx = VerbalExpressions.DefaultExpression;
			verbEx.Add(CommonRegex.Url)
				.Or(CommonRegex.Email);

			Assert.True(verbEx.IsMatch("test@github.com"), "Should match email address");
			Assert.True(verbEx.IsMatch("http://www.google.com"), "Should match url address");
		}
	}
}