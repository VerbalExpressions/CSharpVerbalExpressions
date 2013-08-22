using CSharpVerbalExpressions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    class EndOfLineTests
    {
        VerbalExpressions verbEx = null;

        [Test]
        public void EndOfLine_AddDotComtEndOfLine_DoesMatchDotComInEnd()
        {
            verbEx = VerbalExpressions.DefaultExpression;
            verbEx.Add(".com")
                .EndOfLine();

            var isMatch = verbEx.IsMatch("www.google.com");
            Assert.IsTrue(isMatch, "Should match '.com' in end");
        }

        [Test]
        public void EndOfLine_AddDotComEndOfLine_DoesNotMatchSlashInEnd()
        {
            verbEx = VerbalExpressions.DefaultExpression;
            verbEx.Add(".com")
                .EndOfLine();

            var isMatch = verbEx.IsMatch("http://www.google.com/");
            Assert.IsFalse(isMatch, "Should not match '/' in end");
        }
    }
}
