namespace DevOpsFlex.Core.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using UnitTests.Common;

    /// <summary>
    /// Constains tests that target the camel casing string extensions class.
    /// </summary>
    [TestClass]
    public class StringCamelExtensionsTest
    {

        #region GetUpperConcat

        /// <summary>
        /// Tests the GetUpperConcat with the "FundingStreamConfig" string.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetUpperConcat_WithThreeWords()
        {
            const string input = "FundingStreamConfig";

            var result = input.GetUpperConcat();

            Assert.AreEqual(result, "fsc");
        }

        /// <summary>
        /// Tests the GetUpperConcat with the "BT" string.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetUpperConcat_WithOnlyTwoUpperLetters()
        {
            const string input = "BT";

            var result = input.GetUpperConcat();

            Assert.AreEqual(result, "bt");
        }

        /// <summary>
        /// Tests the GetUpperConcat with a null string.
        /// </summary>
        /// <remarks>
        /// This test validates a Code Contract, so it will only pass if run with Code Contracts rewrite (usually Debug).
        /// </remarks>
        [TestMethod, TestCategory("Unit")]
        public void Ensure_GetUpperConcat_ValidatesNull()
        {
            FluentAssertionExtensions.ShouldThrowPreContract(() => StringCamelExtensions.GetUpperConcat(null));
        }

        /// <summary>
        /// Tests the GetUpperConcat with an empty string.
        /// </summary>
        /// <remarks>
        /// This test validates a Code Contract, so it will only pass if run with Code Contracts rewrite (usually Debug).
        /// </remarks>
        [TestMethod, TestCategory("Unit")]
        public void Ensure_GetUpperConcat_ValidatesEmpty()
        {
            FluentAssertionExtensions.ShouldThrowPreContract(() => "".GetUpperConcat());
        }

        /// <summary>
        /// Tests the GetUpperConcat with a string that only contains white space.
        /// </summary>
        /// <remarks>
        /// This test validates a Code Contract, so it will only pass if run with Code Contracts rewrite (usually Debug).
        /// </remarks>
        [TestMethod, TestCategory("Unit")]
        public void Ensure_GetUpperConcat_ValidatesWhiteSpace()
        {
            FluentAssertionExtensions.ShouldThrowPreContract(() => " ".GetUpperConcat());
        }

        /// <summary>
        /// Tests the GetUpperConcat with a string that only contains lower case, thus returning an empty string.
        /// </summary>
        /// <remarks>
        /// This test validates a Code Contract, so it will only pass if run with Code Contracts rewrite (usually Debug).
        /// </remarks>
        [TestMethod, TestCategory("Unit")]
        public void Ensure_GetUpperConcat_ValidatesBlankReturn()
        {
            FluentAssertionExtensions.ShouldThrowPostContract(() => "fsc".GetUpperConcat());
        }

        #endregion

        #region GetOneCharOneDigit

        /// <summary>
        /// Tests the GetOneCharOneDigit with a string that contains both letters and digits.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetOneCharOneDigit_With2CharReturn()
        {
            const string input = "Release10";

            var result = input.GetOneCharOneDigit();

            Assert.AreEqual("R1", result);
        }

        /// <summary>
        /// Tests the GetOneCharOneDigit with a string that only contains letters.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetOneCharOneDigit_With1LetterReturn()
        {
            const string input = "Release";

            var result = input.GetOneCharOneDigit();

            Assert.AreEqual("R", result);
        }

        /// <summary>
        /// Tests the GetOneCharOneDigit with a string that only contains digits.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetOneCharOneDigit_With1DigitReturn()
        {
            const string input = "2015";

            var result = input.GetOneCharOneDigit();

            Assert.AreEqual("2", result);
        }

        /// <summary>
        /// Tests the GetOneCharOneDigit with a null string.
        /// </summary>
        /// <remarks>
        /// This test validates a Code Contract, so it will only pass if run with Code Contracts rewrite (usually Debug).
        /// </remarks>
        [TestMethod, TestCategory("Unit")]
        public void Ensure_GetOneCharOneDigit_ValidatesNull()
        {
            FluentAssertionExtensions.ShouldThrowPreContract(() => StringCamelExtensions.GetOneCharOneDigit(null));
        }

        /// <summary>
        /// Tests the GetOneCharOneDigit with an empty string.
        /// </summary>
        /// <remarks>
        /// This test validates a Code Contract, so it will only pass if run with Code Contracts rewrite (usually Debug).
        /// </remarks>
        [TestMethod, TestCategory("Unit")]
        public void Ensure_GetOneCharOneDigit_ValidatesEmpty()
        {
            FluentAssertionExtensions.ShouldThrowPreContract(() => "".GetOneCharOneDigit());
        }

        /// <summary>
        /// Tests the GetOneCharOneDigit with a string that only contains white space.
        /// </summary>
        /// <remarks>
        /// This test validates a Code Contract, so it will only pass if run with Code Contracts rewrite (usually Debug).
        /// </remarks>
        [TestMethod, TestCategory("Unit")]
        public void Ensure_GetOneCharOneDigit_ValidatesWhiteSpace()
        {
            FluentAssertionExtensions.ShouldThrowPreContract(() => " ".GetOneCharOneDigit());
        }

        #endregion

        #region GetSqlServiceObjective

        /// <summary>
        /// Tests the GetSqlServiceObjective with the current S3 dimension string.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetSqlServiceObjective_WithS3Value()
        {
            const string objective = "Standard S3 resource allocation.";
            var result = objective.GetSqlServiceObjective();

            Assert.AreEqual("S3", result);
        }

        /// <summary>
        /// Tests the GetSqlServiceObjective with the current P2 dimension string.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetSqlServiceObjective_WithP2Value()
        {
            const string objective = "Standard P2 resource allocation.";
            var result = objective.GetSqlServiceObjective();

            Assert.AreEqual("P2", result);
        }

        /// <summary>
        /// Tests the GetSqlServiceObjective with a ficticious X99 dimension string.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetSqlServiceObjective_WithX99Value()
        {
            const string objective = "Standard X99 resource allocation.";
            var result = objective.GetSqlServiceObjective();

            Assert.AreEqual("X99", result);
        }

        /// <summary>
        /// Tests the GetSqlServiceObjective with a dimension string that contains an invalid char in the middle of
        /// the service objective, thus should return null.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetSqlServiceObjective_WithInvalidCharInMiddle()
        {
            const string objective = "Standard P2a2 resource allocation.";
            var result = objective.GetSqlServiceObjective();

            Assert.IsNull(result);
        }

        /// <summary>
        /// Tests the GetSqlServiceObjective with a dimension string that doesn't contain any service objective,
        /// thus should return null.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetSqlServiceObjective_WithoutValidValue()
        {
            const string objective = "Standard resource allocation.";
            var result = objective.GetSqlServiceObjective();

            Assert.IsNull(result);
        }

        /// <summary>
        /// Tests the GetSqlServiceObjective with a null string.
        /// </summary>
        /// <remarks>
        /// This test validates a Code Contract, so it will only pass if run with Code Contracts rewrite (usually Debug).
        /// </remarks>
        [TestMethod, TestCategory("Unit")]
        public void Ensure_GetSqlServiceObjective_ValidatesNull()
        {
            FluentAssertionExtensions.ShouldThrowPreContract(() => StringCamelExtensions.GetSqlServiceObjective(null));
        }

        /// <summary>
        /// Tests the GetSqlServiceObjective with a string that only contains white space.
        /// </summary>
        /// <remarks>
        /// This test validates a Code Contract, so it will only pass if run with Code Contracts rewrite (usually Debug).
        /// </remarks>
        [TestMethod, TestCategory("Unit")]
        public void Ensure_GetSqlServiceObjective_ValidatesEmpty()
        {
            FluentAssertionExtensions.ShouldThrowPreContract(() => "".GetSqlServiceObjective());
        }

        #endregion

    }
}
