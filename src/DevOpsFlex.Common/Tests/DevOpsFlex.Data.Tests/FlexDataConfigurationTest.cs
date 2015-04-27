namespace DevOpsFlex.Data.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Naming;

    /// <summary>
    /// Contains tests for the static class <see cref="FlexDataConfiguration"/>.
    /// </summary>
    [TestClass]
    public class FlexDataConfigurationTest
    {
        #region UseNaming

        /// <summary>
        /// Tests UseNaming with a single override registration.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_UseNaming_SingleOverride()
        {
            var mockName = new Mock<IName<DevOpsComponent>>();
            FlexDataConfiguration.NamingMap.Clear();

            FlexDataConfiguration.UseNaming(mockName.Object);

            Assert.AreEqual(FlexDataConfiguration.GetNaming<DevOpsComponent>(), mockName.Object);
        }

        /// <summary>
        /// Tests UseNaming by overriding the same <see cref="Type"/> twice.
        /// Ensures that only the last override is matched.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_UseNaming_DualOverride()
        {
            var mockName1 = new Mock<IName<DevOpsComponent>>();
            var mockName2 = new Mock<IName<DevOpsComponent>>();
            FlexDataConfiguration.NamingMap.Clear();

            FlexDataConfiguration.UseNaming(mockName1.Object);
            FlexDataConfiguration.UseNaming(mockName2.Object);

            Assert.AreEqual(FlexDataConfiguration.GetNaming<DevOpsComponent>(), mockName2.Object);
        }

        #endregion

        #region GetNaming

        /// <summary>
        /// Tests that GetNaming will return the <see cref="DefaultNaming{T}"/> when no mappings are defined.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetNaming_ReturnsDefaultOnBlankMapping()
        {
            FlexDataConfiguration.NamingMap.Clear();

            Assert.AreEqual(FlexDataConfiguration.GetNaming<DevOpsComponent>().GetType(), typeof(DefaultNaming<DevOpsComponent>));
        }

        #endregion

    }
}
