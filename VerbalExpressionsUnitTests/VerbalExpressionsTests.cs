using System;
using System.Text.RegularExpressions;
using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{
    [TestFixture]
    public class VerbalExpressionsTests
    {
        private VerbalExpressions verbEx;

        [SetUp]
        public void Setup()
        {
            verbEx = VerbalExpressions.DefaultExpression;
        }

        [TearDown]
        public void Teardown()
        {
            verbEx = null;
        }

        [Test]
        public void TestingIfWeHaveAValidURL()
        {
            verbEx = VerbalExpressions.DefaultExpression
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

        [Test]
        public void StartOfLine_CreatesCorrectRegex()
        {
            verbEx.StartOfLine();
            Assert.AreEqual("^", verbEx.ToString(), "missing start of line regex");
        }
        
        [Test]
        public void StartOfLine_ThenHttpMaybeWww_DoesMatchHttpInStart()
        {
            
            verbEx.StartOfLine()
                .Then("http")
                .Maybe("www");

            var isMatch = Regex.IsMatch("http", verbEx.ToString());
            Assert.IsTrue(isMatch, "Should match http in start");
        }

        [Test]
        public void StartOfLine_ThenHttpMaybeWww_DoesNotMatchWwwInStart()
        {
            
            verbEx.StartOfLine()
                .Then("http")
                .Maybe("www");

            var isMatch = verbEx.IsMatch("www");
            Assert.IsFalse(isMatch, "Should not match www in start");
        }

        [Test]
        public void EndOfLine_AddDotComtEndOfLine_DoesMatchDotComInEnd()
        {
            
            verbEx.Add(".com")
                .EndOfLine();

            var isMatch = verbEx.IsMatch("www.google.com");
            Assert.IsTrue(isMatch, "Should match '.com' in end");
        }

        [Test]
        public void EndOfLine_AddDotComEndOfLine_DoesNotMatchSlashInEnd()
        {
            
            verbEx.Add(".com")
                .EndOfLine();

            var isMatch = verbEx.IsMatch("http://www.google.com/");
            Assert.IsFalse(isMatch, "Should not match '/' in end");
        }

        [Test]
        public void Add_AddDotCom_DoesNotMatchGoogleComWithoutDot()
        {
            
            verbEx.Add(".com");

            var isMatch = verbEx.IsMatch("http://www.googlecom/");
            Assert.IsFalse(isMatch, "Should not match 'ecom'");
        }
        
        [Test]
        public void Or_AddComOrOrg_DoesMatchComAndOrg()
        {
            
            verbEx.Add("com").Or("org");

            Console.WriteLine(verbEx);
            Assert.IsTrue(verbEx.IsMatch("org"), "Should match 'org'");
            Assert.IsTrue(verbEx.IsMatch("com"), "Should match 'com'");
        }
        
        [Test]
        public void Or_AddComOrOrg_RegexIsAsExpecteds()
        {
            
            verbEx.Add("com").Or("org");
            
            Assert.AreEqual("(com)|(org)", verbEx.ToString());
        }
    
        [Test]
        public void Anything_StartOfLineAnythingEndOfline_DoesMatchAnyThing()
        {
            
            verbEx
                .StartOfLine()
                .Anything()
                .EndOfLine();


            var isMatch = verbEx.IsMatch("'!@#$%¨&*()__+{}'");
            Assert.IsTrue(isMatch, "Ooops, should match anything");
        }

        [Test]
        public void WithAnyCase_AddwwwWithAnyCase_DoesMatchwWw()
        {
            
            verbEx.Add("www")
                .WithAnyCase();


            var isMatch = verbEx.IsMatch("wWw");
            Assert.IsTrue(isMatch, "Should match any case");
        }

        [Test]
        public void WithAnyCase_SetsCorrectIgnoreCaseRegexOptionAndHasMultiLineRegexOptionAsDefault()
        {
            
            verbEx.WithAnyCase();

            var regex = verbEx.ToRegex();
            Assert.IsTrue(regex.Options.HasFlag(RegexOptions.IgnoreCase), "RegexOptions should have ignoreCase");
            Assert.IsTrue(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should have MultiLine as default");
        }

        [Test]
        public void RemoveModifier_RemoveModifierM_RemovesMulitilineAsDefault()
        {
            
            var regex = verbEx.ToRegex();
            Assert.IsTrue(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should have MultiLine as default");

            verbEx.RemoveModifier('m');
            regex = verbEx.ToRegex();

            Assert.IsFalse(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should now have been removed");
        }

        [Test]
        public void WithAnyCase_AddwwwWithAnyCaseFalse_DoesNotMatchwWw()
        {
            
            verbEx.Add("www")
                .WithAnyCase(false);


            var isMatch = verbEx.IsMatch("wWw");
            Assert.IsFalse(isMatch, "Should not match any case");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Sanitize_Handles_Null_String()
        {
            //Arrange
            
            string value = null;

            //Act
            //Assert
            value = verbEx.Sanitize(value);
        }

        [Test]
        public void Sanitize_AddCharactersThatShouldBeEscaped_ReturnsEscapedString()
        {
            //Arrange
            
            string value = "*+?";
            string result = string.Empty;
            string expected = @"\*\+\?";

            //Act
            result = verbEx.Sanitize(value);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_WhenNullStringPassedAsParameter_ShouldThrowNullArgumentException()
        {
            //Arrange
            
            string value = null;

            //Act
            //Assert
            verbEx.Add(value);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Range_WhenNullParameterPassed_ShouldThrowArgumentNullException()
        {
            //Arrange
            
            object[] value = null;

            //Act
            //Assert
            verbEx.Range(value);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Range_WhenArrayParameterHasOnlyOneValue_ShouldThrowArgumentOutOfRangeException()
        {
            //Arrange
            
            object[] value = new object[1] { 0 };

            //Act
            //Assert
            verbEx.Range(value);
        }

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
        public void Range_WhenOddNumberOfItemsInArray_ShouldAppendWithPipe()
        {
            //Arrange
            
            object[] range = new object[3] { 1, 6, 7 };
            string expectedExpression = "[1-6]|7";

            //Act
            verbEx.Range(range);

            //Assert
            Assert.AreEqual(expectedExpression, verbEx.ToString());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Multiple_WhenNullArgumentPassed_ThrowsArgumentNullException()
        {
            //Arrange
            
            string argument = string.Empty;

            //Act
            //Assert
            verbEx.Multiple(argument);
        }
    
        [Test]
        public void Then_VerbalExpressionsEmail_DoesMatchEmail()
        {
            
            verbEx.StartOfLine().Then(CommonRegex.Email);
            
            var isMatch = verbEx.IsMatch("test@github.com");
            Assert.IsTrue(isMatch, "Should match email address");
        }
        
        [Test]
        public void Then_VerbalExpressionsEmail_DoesNotMatchUrl()
        {
            
            verbEx.StartOfLine().Then(CommonRegex.Email);
            
            var isMatch = verbEx.IsMatch("http://www.google.com");
            Assert.IsFalse(isMatch, "Should not match url address");
        }
    
        [Test]
        public void Then_VerbalExpressionsUrl_DoesMatchUrl()
        {
            
            verbEx.StartOfLine()
                  .Then(CommonRegex.Url);

            Assert.IsTrue(verbEx.IsMatch("http://www.google.com"), "Should match url address");
            Assert.IsTrue(verbEx.IsMatch("https://www.google.com"), "Should match url address");
            Assert.IsTrue(verbEx.IsMatch("http://google.com"), "Should match url address");
        }	

        [Test]
        public void Then_VerbalExpressionsUrl_DoesNotMatchEmail()
        {
            
            verbEx.StartOfLine().Then(CommonRegex.Url);

            Assert.IsFalse(verbEx.IsMatch("test@github.com"), "Should not match email address");
        }

        [Test]
        public void Or_VerbalExpressionsUrlOrVerbalExpressionEmail_DoesMatchEmailAndUrl()
        {
            
            verbEx.Add(CommonRegex.Url)
                .Or(CommonRegex.Email);

            Assert.IsTrue(verbEx.IsMatch("test@github.com"), "Should match email address");
            Assert.IsTrue(verbEx.IsMatch("http://www.google.com"), "Should match url address");
        }

        [Test]
        public void StartOfLine_WhenPlacedInRandomCallOrder_ShouldAppendAtTheBeginningOfTheExpression()
        {
            
            verbEx.Add("test")
                .Add("ing")
                .StartOfLine();

            string text = "testing1234";
            Assert.IsTrue(verbEx.IsMatch(text), "Should match that the text starts with test");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Multiple_WhenNullOrEmptyValueParameterIsPassed_ShouldThrowArgumentException()
        {
            //Arrange
            
            string value = null;

            //Act
            //Assert
            verbEx.Multiple(value);
        }

        [Test]
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
            Assert.AreEqual(expectedExpression, verbEx.ToString());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AnyOf_WhenValueParameterIsNullOrEmpty_ShouldThrowArgumentException()
        {
            //Arrange
            
            string value = null;

            //Act
            //Assert
            verbEx.AnyOf(value);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Any_WhenValueParameterIsNullOrEmpty_ShouldThrowArgumentException()
        {
            //Arrange
            
            string value = null;

            //Act
            //Assert
            verbEx.Any(value);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Find_WhenNullParameterValueIsPassed_ThrowsArgumentException()
        {
            //Arrange
            
            string value = null;

            //Act
            //Assert
            verbEx.Find(value);
        }

        [Test]
        public void LineBreak_WhenCalled_ReturnsExpectedExpression()
        {
            //Arrange
            
            string text = string.Format("testin with {0} line break",Environment.NewLine);

            //Act
            verbEx.LineBreak();
            //Assert
            Assert.IsTrue(verbEx.Test(text));
        }

        [Test]
        public void Br_WhenCalled_ReturnsExpectedExpression()
        {
            //Arrange
            
            string text = string.Format("testin with {0} line break", Environment.NewLine);

            //Act
            verbEx.Br();
            //Assert
            Assert.IsTrue(verbEx.Test(text));
        }

        [Test]
        public void Tab_WhenCalled_ReturnsExpectedExpression()
        {
            //Arrange
            
            string text = string.Format("text that contains {0} a tab",@"\t");

            //Act
            verbEx.Tab();

            //Assert
            Assert.IsTrue(verbEx.Test(text));
        }

        [Test]
        public void Word_WhenCalled_ReturnsExpectedNumberOfWords()
        {
            //Arrange
            
            string text = "three words here";
            int expectedCount = 3;
            
            //Act
            verbEx.Word();
            Regex currentExpression = verbEx.ToRegex();
            int result = currentExpression.Matches(text).Count;

            //Assert
            Assert.AreEqual(expectedCount, result);
        }

        [Test]
        public void UseOneLineSearchOption_WhenCalled_ShouldChangeMultilineModifier()
        {
            //Arrange
            verbEx.UseOneLineSearchOption(false);
            var regex = verbEx.ToRegex();
            Assert.IsTrue(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should now be present");
            //Act
            verbEx.UseOneLineSearchOption(true);
            //Assert
            regex = verbEx.ToRegex();
            Assert.IsFalse(regex.Options.HasFlag(RegexOptions.Multiline), "RegexOptions should now have been removed");
        }

        [Test]
        public void Maybe_WhenCalled_UsesCommonRegexUrl()
        {
            verbEx.Maybe(CommonRegex.Url);

            Assert.IsTrue(verbEx.IsMatch("http://www.google.com"), "Should match url address");
        }

        [Test]
        public void Maybe_WhenCalled_UsesCommonRegexEmail()
        {
            verbEx.Maybe(CommonRegex.Email);

            Assert.IsTrue(verbEx.IsMatch("test@github.com"), "Should match email address");
        }

        [Test]
        public void AddModifier_AddModifierI_RemovesCase()
        {
            verbEx.Add("teststring")
                .AddModifier('i');

            Assert.IsTrue(verbEx.IsMatch("TESTSTRING"));

        }

        [Test]
        public void AddModifier_AddModifierX_IgnoreWhitspace()
        {
            verbEx.Add("test string")
                  .AddModifier('x');

            Assert.IsTrue(verbEx.IsMatch("test string #comment"));
        }

        [Test]
        public void AddModifier_AddModifierM_Multiline()
        {
            //Arrange
            string text = string.Format("testin with {0} line break", Environment.NewLine);

            //Act
            verbEx.AddModifier('m');

            //Assert
            Assert.IsTrue(verbEx.Test(text));
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
