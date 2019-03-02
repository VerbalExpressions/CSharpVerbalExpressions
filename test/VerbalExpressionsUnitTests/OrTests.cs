using System;
using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    public class OrTests
    {
        [Test]
        public void Or_AddComOrOrg_DoesMatchComAndOrg()
        {
            var verbEx = VerbalExpressions.DefaultExpression;
            verbEx.Add("com").Or("org");

            Console.WriteLine(verbEx);
            Assert.IsTrue(verbEx.IsMatch("org"), "Should match 'org'");
            Assert.IsTrue(verbEx.IsMatch("com"), "Should match 'com'");
        }

        [Test]
        public void Or_AddComOrOrg_RegexIsAsExpecteds()
        {
            var verbEx = VerbalExpressions.DefaultExpression;
            verbEx.Add("com").Or("org");

            Assert.AreEqual("(com)|(org)", verbEx.ToString());
        }

        [Test]
        public void Or_VerbalExpressionsUrlOrVerbalExpressionEmail_DoesMatchEmailAndUrl()
        {
            var verbEx = VerbalExpressions.DefaultExpression;
            verbEx.Add(CommonRegex.Url)
                .Or(CommonRegex.Email);

            Assert.IsTrue(verbEx.IsMatch("test@github.com"), "Should match email address");
            Assert.IsTrue(verbEx.IsMatch("http://www.google.com"), "Should match url address");
        }
    }
}
