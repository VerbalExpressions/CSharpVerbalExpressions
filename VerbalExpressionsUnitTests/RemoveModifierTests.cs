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
    class RemoveMofifierTests
    {
        private VerbalExpressions verbEx = null;

        [Test]
        public void RemoveModifier_RemoveModifierM_RemovesMulitilineAsDefault()
        {
            verbEx = VerbalExpressions.DefaultExpression;
            var regex = verbEx.ToRegex();
            Assert.IsTrue(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should have MultiLine as default");

            verbEx.RemoveModifier('m');
            regex = verbEx.ToRegex();

            Assert.IsFalse(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should now have been removed");
        }

        [Test]
        public void RemoveModifier_RemoveModifierI_RemovesCase()
        {
            verbEx.AddModifier('i');

            verbEx.RemoveModifier('i');
            var regex = verbEx.ToRegex();
            Assert.IsFalse(regex.Options.HasFlag(RegexOptions.IgnoreCase), "RegexOptions should now have been removed");
        }

        [Test]
        public void RemoveModifier_RemoveModifierX_RemovesCase()
        {
            verbEx.AddModifier('x');

            verbEx.RemoveModifier('x');
            var regex = verbEx.ToRegex();
            Assert.IsFalse(regex.Options.HasFlag(RegexOptions.IgnorePatternWhitespace), "RegexOptions should now have been removed");
        }
    }
}
