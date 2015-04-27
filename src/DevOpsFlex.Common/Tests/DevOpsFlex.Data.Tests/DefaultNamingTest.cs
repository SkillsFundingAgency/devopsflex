namespace DevOpsFlex.Data.Tests
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Naming;

    /// <summary>
    /// Contains tests that target the <see cref="DefaultNaming{T}"/> class.
    /// </summary>
    [TestClass]
    public class DefaultNamingTest
    {

        #region GetSlotName

        /// <summary>
        /// Tests a slot name for a component in the main branch.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetSlotName_UnderMainBranch()
        {
            const string logicalName = "something";
            const string systemName = "tsys";
            const string branch = "Main";
            const string configuration = "TEST";

            var expected = string.Concat(systemName.ToLower().Take(DefaultNaming<AzureCloudService>.MaxSystemLength)) + "-" +
                           string.Concat(logicalName.ToLower().Take(DefaultNaming<AzureCloudService>.MaxComponentLength)) + "-" +
                           string.Concat(configuration.ToLower().Take(DefaultNaming<AzureCloudService>.MaxConfigurationLength));

            FlexDataConfiguration.RootBranch = branch;

            var component =
                new AzureCloudService
                {
                    LogicalName = logicalName,
                    System = new DevOpsSystem {LogicalName = systemName}
                };

            var namer = new DefaultNaming<AzureCloudService>();

            var result = namer.GetSlotName(component, branch, configuration);

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Tests a slot name for a component outside the main branch.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetSlotName_OutsideMainBranch()
        {
            const string logicalName = "something";
            const string systemName = "tsys";
            const string branch = "Release10";
            const string configuration = "TEST";

            var expected = string.Concat(systemName.ToLower().Take(DefaultNaming<AzureCloudService>.MaxSystemLength)) + "r1" + "-" +
                           string.Concat(logicalName.ToLower().Take(DefaultNaming<AzureCloudService>.MaxComponentLength)) + "-" +
                           string.Concat(configuration.ToLower().Take(DefaultNaming<AzureCloudService>.MaxConfigurationLength));

            FlexDataConfiguration.RootBranch = "Main";

            var component =
                new AzureCloudService
                {
                    LogicalName = logicalName,
                    System = new DevOpsSystem { LogicalName = systemName }
                };

            var namer = new DefaultNaming<AzureCloudService>();

            var result = namer.GetSlotName(component, branch, configuration);

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Tests a slot name for a component that has a system name that needs truncating.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetSlotName_TruncatesSystemName()
        {
            const string logicalName = "something";
            const string branch = "Main";
            const string configuration = "TEST";

            var systemName = Guid.NewGuid().ToString();

            var expected = string.Concat(systemName.ToLower().Take(DefaultNaming<AzureCloudService>.MaxSystemLength)) + "-" +
                           string.Concat(logicalName.ToLower().Take(DefaultNaming<AzureCloudService>.MaxComponentLength)) + "-" +
                           string.Concat(configuration.ToLower().Take(DefaultNaming<AzureCloudService>.MaxConfigurationLength));

            FlexDataConfiguration.RootBranch = branch;

            var component =
                new AzureCloudService
                {
                    LogicalName = logicalName,
                    System = new DevOpsSystem { LogicalName = systemName }
                };

            var namer = new DefaultNaming<AzureCloudService>();

            var result = namer.GetSlotName(component, branch, configuration);

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Tests a slot name for a component that has a logical name that needs truncating.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetSlotName_TruncatesComponentName()
        {
            const string systemName = "tsys";
            const string branch = "Main";
            const string configuration = "TEST";

            var logicalName = Guid.NewGuid().ToString();

            var expected = string.Concat(systemName.ToLower().Take(DefaultNaming<AzureCloudService>.MaxSystemLength)) + "-" +
                           string.Concat(logicalName.ToLower().Take(DefaultNaming<AzureCloudService>.MaxComponentLength)) + "-" +
                           string.Concat(configuration.ToLower().Take(DefaultNaming<AzureCloudService>.MaxConfigurationLength));

            FlexDataConfiguration.RootBranch = branch;

            var component =
                new AzureCloudService
                {
                    LogicalName = logicalName,
                    System = new DevOpsSystem { LogicalName = systemName }
                };

            var namer = new DefaultNaming<AzureCloudService>();

            var result = namer.GetSlotName(component, branch, configuration);

            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Tests a slot name for a component that has a configuration name that needs truncating.
        /// </summary>
        [TestMethod, TestCategory("Unit")]
        public void Test_GetSlotName_TruncatesConfigurationName()
        {
            const string logicalName = "something";
            const string systemName = "tsys";
            const string branch = "Main";

            var configuration = Guid.NewGuid().ToString();

            var expected = string.Concat(systemName.ToLower().Take(DefaultNaming<AzureCloudService>.MaxSystemLength)) + "-" +
                           string.Concat(logicalName.ToLower().Take(DefaultNaming<AzureCloudService>.MaxComponentLength)) + "-" +
                           string.Concat(configuration.ToLower().Take(DefaultNaming<AzureCloudService>.MaxConfigurationLength));

            FlexDataConfiguration.RootBranch = branch;

            var component =
                new AzureCloudService
                {
                    LogicalName = logicalName,
                    System = new DevOpsSystem { LogicalName = systemName }
                };

            var namer = new DefaultNaming<AzureCloudService>();

            var result = namer.GetSlotName(component, branch, configuration);

            Assert.AreEqual(expected, result);
        }

        #endregion

    }
}
