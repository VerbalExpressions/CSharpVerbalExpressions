using System.Text.RegularExpressions;
using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    public class RemoveMofifierTests
    {
        [Test]
        public void RemoveModifier_RemoveModifierM_RemovesMulitilineAsDefault()
        {
            var verbEx = VerbalExpressions.DefaultExpression;
            var regex = verbEx.ToRegex();
            Assert.IsTrue(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should have MultiLine as default");

            verbEx.RemoveModifier('m');
            regex = verbEx.ToRegex();

            Assert.IsFalse(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should now have been removed");
        }

        [Test]
        public void RemoveModifier_RemoveModifierI_RemovesCase()
        {
            var verbEx = VerbalExpressions.DefaultExpression;
            verbEx.AddModifier('i');

            verbEx.RemoveModifier('i');
            var regex = verbEx.ToRegex();
            Assert.IsFalse(regex.Options.HasFlag(RegexOptions.IgnoreCase), "RegexOptions should now have been removed");
        }

        [Test]
        public void RemoveModifier_RemoveModifierX_RemovesCase()
        {
            var verbEx = VerbalExpressions.DefaultExpression;
            verbEx.AddModifier('x');

            verbEx.RemoveModifier('x');
            var regex = verbEx.ToRegex();
            Assert.IsFalse(regex.Options.HasFlag(RegexOptions.IgnorePatternWhitespace), "RegexOptions should now have been removed");
        }
    }
}
