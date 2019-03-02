using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    public class PerformanceTests
    {

        [Test]
        public void VerbalExpression_Is_Not_Slower_Than_Direct_Use_Of_Regex()
        {
            const string someUrl = "https://www.google.com";

            var verbEx = VerbalExpressions.DefaultExpression
                .StartOfLine()
                .Then("http")
                .Maybe("s")
                .Then("://")
                .Maybe("www.")
                .AnythingBut(" ")
                .EndOfLine();

            var regex = new Regex(@"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$");

            var timeVerbEx = MeasureCallDuration(() => verbEx.IsMatch(someUrl));
            var timeRegex = MeasureCallDuration(() => regex.IsMatch(someUrl));

            Assert.That(timeVerbEx - timeRegex, Is.LessThan(TimeSpan.FromSeconds(0.1)));
        }

        private static TimeSpan MeasureCallDuration(Action action)
        {
            var sw = Stopwatch.StartNew();

            for (var i = 0; i < 10000; i++)
            {
                action();
            }

            sw.Stop();
            return sw.Elapsed;
        }
    }
}
