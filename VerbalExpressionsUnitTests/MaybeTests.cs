using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    public class MaybeTests
    {
        [Test]
        public void Maybe_WhenCalled_UsesCommonRegexUrl()
        {
            var verbEx = VerbalExpressions.DefaultExpression;
            verbEx.Maybe(CommonRegex.Url);

            Assert.IsTrue(verbEx.IsMatch("http://www.google.com"), "Should match url address");
        }

        [Test]
        public void Maybe_WhenCalled_UsesCommonRegexEmail()
        {
            var verbEx = VerbalExpressions.DefaultExpression;
            verbEx.Maybe(CommonRegex.Email);

            Assert.IsTrue(verbEx.IsMatch("test@github.com"), "Should match email address");
        }
    }
}
