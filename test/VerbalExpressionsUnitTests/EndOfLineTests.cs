using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    public class EndOfLineTests
    {
        [Test]
        public void EndOfLine_AddDotComtEndOfLine_DoesMatchDotComInEnd()
        {
            var verbEx = VerbalExpressions.DefaultExpression;
            verbEx.Add(".com").EndOfLine();

            var isMatch = verbEx.IsMatch("www.google.com");
            Assert.IsTrue(isMatch, "Should match '.com' in end");
        }

        [Test]
        public void EndOfLine_AddDotComEndOfLine_DoesNotMatchSlashInEnd()
        {
            var verbEx = VerbalExpressions.DefaultExpression;
            verbEx.Add(".com").EndOfLine();

            var isMatch = verbEx.IsMatch("http://www.google.com/");
            Assert.IsFalse(isMatch, "Should not match '/' in end");
        }
    }
}
