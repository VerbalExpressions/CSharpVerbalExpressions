using CSharpVerbalExpressions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    class MaybeTests
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
