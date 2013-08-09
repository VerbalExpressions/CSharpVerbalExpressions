using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpVerbalExpressions;

namespace VerbalExpressionsUnitTests
{
    [TestClass]
    public class VerbalExpressionsTests
    {
        private VerbalExpressions verbEx = null;

        [TestInitialize]
        public void Initialize()
        {
            verbEx = VerbalExpressions.NewExpression;
        }

        [TestCleanup]
        public void TearDown()
        {
            verbEx = null;
        }

        [TestMethod]
        public void TestingIfWeHaveAValidURL()
        {
            verbEx = VerbalExpressions.NewExpression
                        .StartOfLine()
                        .Then("http")
                        .Maybe("s")
                        .Then("://")
                        .Maybe("www.")
                        .AnythingBut(" ")
                        .EndOfLine();

            var testMe = "https://www.google.com";

            Assert.IsTrue(verbEx.Test(testMe), "The URL is incorrect");
        }    

        [TestMethod]
        public void StartOfLine_CreatesCorrectRegex()
        {
            verbEx.StartOfLine();
            Assert.AreEqual("^", verbEx.ToString(), "missing start of line regex");
        }
        
        [TestMethod]
        public void StartOfLine_ThenHttpMaybeWww_DoesMatchHttpInStart()
        {
            verbEx.StartOfLine()
                .Then("http")
                .Maybe("www");

            var isMatch = Regex.IsMatch("http", verbEx.ToString());
            Assert.IsTrue(isMatch, "Should match http in start");
        }

        [TestMethod]
        public void StartOfLine_ThenHttpMaybeWww_DoesNotMatchWwwInStart()
        {
            verbEx.StartOfLine()
                .Then("http")
                .Maybe("www");

            var isMatch = verbEx.IsMatch("www");
            Assert.IsFalse(isMatch, "Should not match www in start");
        }

        [TestMethod]
        public void EndOfLine_AddDotComtEndOfLine_DoesMatchDotComInEnd()
        {
            verbEx.Add(".com")
                .EndOfLine();

            var isMatch = verbEx.IsMatch("www.google.com");
            Assert.IsTrue(isMatch, "Should match '.com' in end");
        }

        [TestMethod]
        public void EndOfLine_AddDotComEndOfLine_DoesNotMatchSlashInEnd()
        {
            verbEx.Add(".com")
                .EndOfLine();

            var isMatch = verbEx.IsMatch("http://www.google.com/");
            Assert.IsFalse(isMatch, "Should not match '/' in end");
        }

        [TestMethod]
        public void Add_AddDotCom_DoesNotMatchGoogleComWithoutDot()
        {
            verbEx.Add(".com");

            var isMatch = verbEx.IsMatch("http://www.googlecom/");
            Assert.IsFalse(isMatch, "Should not match 'ecom'");
        }
        
        [TestMethod]
        public void Or_AddComOrOrg_DoesMatchComAndOrg()
        {
            verbEx.Add("com").Or("org");

            Console.WriteLine(verbEx);
            Assert.IsTrue(verbEx.IsMatch("org"), "Should match 'org'");
            Assert.IsTrue(verbEx.IsMatch("com"), "Should match 'com'");
        }
        
        [TestMethod]
        public void Or_AddComOrOrg_RegexIsAsExpecteds()
        {
            verbEx.Add("com").Or("org");
            
            Assert.AreEqual("(com)|(org)", verbEx.ToString());
        }
    
        [TestMethod]
        public void Anything_StartOfLineAnythingEndOfline_DoesMatchAnyThing()
        {
            verbEx
                .StartOfLine()
                .Anything()
                .EndOfLine();


            var isMatch = verbEx.IsMatch("'!@#$%¨&*()__+{}'");
            Assert.IsTrue(isMatch, "Ooops, should match anything");
        }

        [TestMethod]
        public void WithAnyCase_AddwwwWithAnyCase_DoesMatchwWw()
        {
            verbEx.Add("www")
                .WithAnyCase();


            var isMatch = verbEx.IsMatch("wWw");
            Assert.IsTrue(isMatch, "Should match any case");
        }

        [TestMethod]
        public void WithAnyCase_SetsCorrectIgnoreCaseRegexOptionAndHasMultiLineRegexOptionAsDefault()
        {
            verbEx.WithAnyCase();

            var regex = verbEx.ToRegex();
            Assert.IsTrue(regex.Options.HasFlag(RegexOptions.IgnoreCase), "RegexOptions should have ignoreCase");
            Assert.IsTrue(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should have MultiLine as default");
        }

        [TestMethod]
        public void RemoveModifier_RemoveModifierM_RemovesMulitilineAsDefault()
        {
            var regex = verbEx.ToRegex();
            Assert.IsTrue(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should have MultiLine as default");

            verbEx.RemoveModifier('m');
            regex = verbEx.ToRegex();

            Assert.IsFalse(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should now have been removed");
        }

        [TestMethod]
        public void WithAnyCase_AddwwwWithAnyCaseFalse_DoesNotMatchwWw()
        {
            verbEx.Add("www")
                .WithAnyCase(false);


            var isMatch = verbEx.IsMatch("wWw");
            Assert.IsFalse(isMatch, "Should not match any case");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Sanitize_Handles_Null_String()
        {
            //Arrange
            string value = null;

            //Act
            //Assert
            value = verbEx.Sanitize(value);
        }

        [TestMethod]
        public void Sanitize_AddCharactersThatShouldBeEscaped_ReturnsEscapedString()
        {
            //Arrange
            string value = "*+?";
            string result = string.Empty;
            string expected = @"\*\+\?";

            //Act
            result = verbEx.Sanitize(value);

            //Assert
            Assert.AreEqual<string>(expected, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_WhenNullStringPassedAsParameter_ShouldThrowNullArgumentException()
        {
            //Arrange
            string value = null;

            //Act
            //Assert
            verbEx.Add(value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Range_WhenNullParameterPassed_ShouldThrowArgumentNullException()
        {
            //Arrange
            object[] value = null;

            //Act
            //Assert
            verbEx.Range(value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Range_WhenArrayParameterHasOnlyOneValue_ShouldThrowArgumentOutOfRangeException()
        {
            //Arrange
            object[] value = new object[1] { 0 };

            //Act
            //Assert
            verbEx.Range(value);
        }

        [TestMethod]
        public void Range_WhenArrayParameterHasValuesInReverseOrder_ReturnsCorrectResultForCorrectOrder()
        {
            //Arrange
            object[] inversedOrderArray = new object[2] { 9, 2 };
            verbEx.Range(inversedOrderArray);
            string lookupString = "testing 8 another test";

            //Act
            bool isMatch = verbEx.IsMatch(lookupString);

            //Assert
            Assert.IsTrue(isMatch);
        }

        [TestMethod]
        public void Range_WhenArrayContainsNullParameter_ItIsIgnoredAndRemovedFromList()
        {
            //Arrange
            object[] inversedOrderArray = new object[4] { 1, null, null, 7 };
            verbEx.Range(inversedOrderArray);
            string lookupString = "testing 5 testing";

            //Act
            bool isMatch = verbEx.IsMatch(lookupString);

            //Assert
            Assert.IsTrue(isMatch);

        }

        [TestMethod]
        public void Replace_WhenCalledImmediatelyAfteInitialize_ShouldNotThrowNullReferenceException()
        {
            //Arrange
            string value = "value";
            bool hasThrownNullReferenceEx = false;

            //Act
            try
            {
                verbEx.Replace(value);
            }
            catch (NullReferenceException)
            {
                hasThrownNullReferenceEx = true;
            }

            //Assert
            Assert.IsFalse(hasThrownNullReferenceEx);
        }

        [TestMethod]
        public void Range_WhenOddNumberOfItemsInArray_ShouldAppendLastElementWithOrClause()
        {
            //Arrange
            string text = "abcd7sdadqascdaswde";
            object[] range = new object[3] { 1, 6, 7 };

            //Act
            verbEx.Range(range);
            //Assert
            Assert.IsTrue(verbEx.IsMatch(text));
        }

        [TestMethod]
        public void Range_WhenOddNumberOfItemsInArray_ShouldAppendWithPipe()
        {
            //Arrange
            object[] range = new object[3] { 1, 6, 7 };
            string expectedExpression = "[1-6]|7";

            //Act
            verbEx.Range(range);

            //Assert
            Assert.AreEqual<string>(expectedExpression, verbEx.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Multiple_WhenNullArgumentPassed_ThrowsArgumentNullException()
        {
            //Arrange
            string argument = string.Empty;

            //Act
            //Assert
            verbEx.Multiple(argument);
        }
    
        [TestMethod]
        public void Then_VerbalExpressionsEmail_DoesMatchEmail()
        {
            verbEx.StartOfLine().Then(CommonRegex.Email);
            
            var isMatch = verbEx.IsMatch("test@github.com");
            Assert.IsTrue(isMatch, "Should match email address");
        }
        
        [TestMethod]
        public void Then_VerbalExpressionsEmail_DoesNotMatchUrl()
        {
            verbEx.StartOfLine().Then(CommonRegex.Email);
            
            var isMatch = verbEx.IsMatch("http://www.google.com");
            Assert.IsFalse(isMatch, "Should not match url address");
        }
    
        [TestMethod]
        public void Then_VerbalExpressionsUrl_DoesMatchUrl()
        {
            verbEx.StartOfLine()
                  .Then(CommonRegex.Url);

            Assert.IsTrue(verbEx.IsMatch("http://www.google.com"), "Should match url address");
            Assert.IsTrue(verbEx.IsMatch("https://www.google.com"), "Should match url address");
            Assert.IsTrue(verbEx.IsMatch("http://google.com"), "Should match url address");
        }	

        [TestMethod]
        public void Then_VerbalExpressionsUrl_DoesNotMatchEmail()
        {
            verbEx.StartOfLine().Then(CommonRegex.Url);

            Assert.IsFalse(verbEx.IsMatch("test@github.com"), "Should not match email address");
        }

        [TestMethod]
        public void Or_VerbalExpressionsUrlOrVerbalExpressionEmail_DoesMatchEmailAndUrl()
        {
            verbEx.Add(CommonRegex.Url)
                .Or(CommonRegex.Email);

            Console.WriteLine(verbEx);
            Assert.IsTrue(verbEx.IsMatch("test@github.com"), "Should match email address");
            Assert.IsTrue(verbEx.IsMatch("http://www.google.com"), "Should match url address");
        }

        [TestMethod]
        public void StartOfLine_WhenPlacedInRandomCallOrder_ShouldAppendAtTheBeginningOfTheExpression()
        {
            verbEx.Add("test")
                .Add("ing")
                .StartOfLine();

            string text = "testing1234";
            Assert.IsTrue(verbEx.IsMatch(text), "Should match that the text starts with test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Multiple_WhenNullOrEmptyValueParameterIsPassed_ShouldThrowArgumentException()
        {
            //Arrange
            string value = null;

            //Act
            //Assert
            verbEx.Multiple(value);
        }

        [TestMethod]
        public void Multiple_WhenParamIsGiven_ShouldMatchOneOrMultipleValuesGiven()
        {
            //Arrange
            string text = "testesting 123 yahoahoahou another test";
            string expectedExpression = "y(aho)+u";
            //Act
            verbEx.Add("y")
                .Multiple("aho")
                .Add("u");

            //Assert
            Assert.IsTrue(verbEx.Test(text));
            Assert.AreEqual<string>(expectedExpression, verbEx.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AnyOf_WhenValueParameterIsNullOrEmpty_ShouldThrowArgumentException()
        {
            string value = null;
            verbEx.AnyOf(value);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Any_WhenValueParameterIsNullOrEmpty_ShouldThrowArgumentException()
        {
            string value = null;
            verbEx.Any(value);
        }
    }
}
