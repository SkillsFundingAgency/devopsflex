namespace DevOpsFlex.Core.Tests
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            AssertionExtensions.ShouldThrow<Exception>(() => StringCamelExtensions.GetUpperConcat(null))
                               .Where(e => e.GetType().Name == "ContractException")
                               .Which.Message.ToLower().Should().Contain("precondition failed:");
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
            AssertionExtensions.ShouldThrow<Exception>(() => "".GetUpperConcat())
                               .Where(e => e.GetType().Name == "ContractException")
                               .Which.Message.ToLower().Should().Contain("precondition failed:");
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
            AssertionExtensions.ShouldThrow<Exception>(() => " ".GetUpperConcat())
                               .Where(e => e.GetType().Name == "ContractException")
                               .Which.Message.ToLower().Should().Contain("precondition failed:");
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
            AssertionExtensions.ShouldThrow<Exception>(() => "fsc".GetUpperConcat())
                               .Where(e => e.GetType().Name == "ContractException")
                               .Which.Message.ToLower().Should().Contain("postcondition failed:");
        }

        #endregion

    }
}
