using System.Text.RegularExpressions;
using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    public class WithAnyCaseTests
    {
        [Test]
        public void WithAnyCase_AddwwwWithAnyCase_DoesMatchwWw()
        {
            var verbEx = VerbalExpressions.DefaultExpression;
            verbEx.Add("www")
                .WithAnyCase();


            var isMatch = verbEx.IsMatch("wWw");
            Assert.IsTrue(isMatch, "Should match any case");
        }

        [Test]
        public void WithAnyCase_SetsCorrectIgnoreCaseRegexOptionAndHasMultiLineRegexOptionAsDefault()
        {
            var verbEx = VerbalExpressions.DefaultExpression;
            verbEx.WithAnyCase();

            var regex = verbEx.ToRegex();
            Assert.IsTrue(regex.Options.HasFlag(RegexOptions.IgnoreCase), "RegexOptions should have ignoreCase");
            Assert.IsTrue(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should have MultiLine as default");
        }

        [Test]
        public void WithAnyCase_AddwwwWithAnyCaseFalse_DoesNotMatchwWw()
        {
            var verbEx = VerbalExpressions.DefaultExpression;
            verbEx.Add("www")
                .WithAnyCase(false);

            var isMatch = verbEx.IsMatch("wWw");
            Assert.IsFalse(isMatch, "Should not match any case");
        }
    }
}
