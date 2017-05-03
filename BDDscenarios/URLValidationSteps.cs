using System;
using TechTalk.SpecFlow;
using CSharpVerbalExpressions;
using NUnit.Framework;

namespace BDDscenarios
{
    [Binding]
    public class URLValidationSteps
    {
        private VerbalExpressions verbEx = new VerbalExpressions();
        private string paramString;
        [Given(@"I have a valid URL ""(.*)""")]
        public void GivenIHaveAValidURL(string url)
        {
            paramString = url;
        }
        
        [When(@"I test it with the appropriate methods")]
        public void WhenITestItWithTheAppropriateMethods()
        {
            verbEx = VerbalExpressions.DefaultExpression
                        .StartOfLine()
                        .Then("http")
                        .Maybe("s")
                        .Then("://")
                        .Maybe("www.")
                        .AnythingBut(" ")
                        .EndOfLine();

           
        }
        
        [Then(@"I get a true assertion")]
        public void ThenIGetATrueAssertion()
        {
            Assert.IsTrue(verbEx.Test(paramString), "The URL is incorrect");
        }
    }
}
