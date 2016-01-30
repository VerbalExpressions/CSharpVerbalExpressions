using CSharpVerbalExpressions;
using NUnit.Framework;

namespace VerbalExpressionsUnitTests
{

    [TestFixture]
    public class CaptureTests
    {
        [Test]
        public void BeginCaptureAndEndCapture_AddComOrOrg_RegexIsAsExpected()
        {
            // Arrange
            VerbalExpressions verbEx = VerbalExpressions.DefaultExpression;

            // Act
            verbEx.BeginCapture()
                  .Add("com")
                  .Or("org")
                  .EndCapture();

            // Assert
            Assert.AreEqual("((com)|(org))", verbEx.ToString());
        }

        [Test]
        public void BeginCaptureAndEndCapture_DuplicatesIdentifier_DoesMatch()
        {
            // Arrange
            VerbalExpressions verbEx = VerbalExpressions.DefaultExpression;
            const string TEST_STRING = "He said that that was the the correct answer.";

            // Act
            verbEx.BeginCapture()
                  .Word()
                  .EndCapture()
                  .Add(@"\s", false)
                  .BeginCapture()
                  .Add(@"\1", false)
                  .EndCapture();

            // Assert
            Assert.AreEqual(@"(\w+)\s(\1)", verbEx.ToString());
            Assert.IsTrue(verbEx.Test(TEST_STRING), "There is no duplicates in the textString.");
        }

        [Test]
        public void BeginCaptureWithName_CreateRegexGroupNameAsExpected()
        {
            // Arrange
            VerbalExpressions verbEx = VerbalExpressions.DefaultExpression;

            // Act
            verbEx.Add("COD")
                .BeginCapture("GroupNumber")
                .Any("0-9")
                .RepeatPrevious(3)
                .EndCapture()
                .Add("END");

            // Assert
            Assert.AreEqual(@"COD(?<GroupNumber>[0-9]{3})END", verbEx.ToString());
            Assert.AreEqual("123", verbEx.Capture("COD123END", "GroupNumber"));
        }
    }

}