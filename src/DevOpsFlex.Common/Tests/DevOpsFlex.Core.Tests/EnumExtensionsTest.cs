namespace DevOpsFlex.Core.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Contains tests that target the <see cref="EnumExtensions"/> utility/mapping class for enums.
    /// </summary>
    [TestClass]
    public class EnumExtensionsTest
    {

        #region GetEnumDescription

        /// <summary>
        /// Holds a constant to the description attribute for the GetEnumDescription tests.
        /// </summary>
        private const string EnumDescription = "Some Description";

        /// <summary>
        /// Tests GetEnumDescription with an enum value that contains the <see cref="System.ComponentModel.DescriptionAttribute"/>.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetEnumDescription_WithAValidDescription()
        {
            var result = SomeEnumWithDescription.HasDescription.GetEnumDescription();

            Assert.AreEqual(result, EnumDescription);
        }

        /// <summary>
        /// Tests GetEnumDescription with an enum value that does not contain the <see cref="System.ComponentModel.DescriptionAttribute"/>.
        /// Ensures that the ToString() representation is returned instead.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetEnumDescription_WithNoDescription()
        {
            var result = SomeEnumWithDescription.NoDescription.GetEnumDescription();

            Assert.AreEqual(result, SomeEnumWithDescription.NoDescription.ToString());
        }

        /// <summary>
        /// Utility enum for the GetEnumDescription tests.
        /// </summary>
        enum SomeEnumWithDescription
        {
            [System.ComponentModel.Description(EnumDescription)] HasDescription,
            NoDescription
        }

        #endregion

    }
}
