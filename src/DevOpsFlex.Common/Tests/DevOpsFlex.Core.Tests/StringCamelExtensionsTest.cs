namespace DevOpsFlex.Core.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StringCamelExtensionsTest
    {
        [TestMethod]
        public void Test_GetUpperConcat_WithThreeWords()
        {
            const string input = "FundingStreamConfig";

            var result = input.GetUpperConcat();

            Assert.AreEqual(result, "fsc");
        }
    }
}
