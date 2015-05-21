namespace DevOpsFlex.Azure.Management.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Core;
    using Data;
    using Data.Naming;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Contains tests that target the extension class <see cref="DevOpsFlexDbContextExtensions"/>.
    /// </summary>
    [TestClass]
    public class DevOpsFlexDbContextExtensionsTest
    {
        /// <summary>
        /// Tests the ProvisionAllAsync on <see cref="AzureCloudService"/> with real SQL Db data (seed).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_AzureCloudService_ProvisionAll_End2End()
        {
            FlexDataConfiguration.Branch = "Main";
            FlexDataConfiguration.Configuration = "MO";

            using (var client = ManagementClient.CreateComputeClient())
            using (var context = new DevOpsFlexDbContext())
            {
                await context.Components.OfType<AzureCloudService>().ProvisionAllAsync(client);
            }
        }

        /// <summary>
        /// Tests the ReserveAllIpsAsync on <see cref="AzureCloudService"/> with real SQL Db data (seed).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_AzureCloudService_ReserveAllIps_End2End()
        {
            FlexDataConfiguration.Branch = "Main";
            FlexDataConfiguration.Configuration = "MO";

            using (var client = ManagementClient.CreateNetworkClient())
            using (var context = new DevOpsFlexDbContext())
            {
                await context.Components.OfType<AzureCloudService>().ReserveAllIpsAsync(client);
            }
        }

        /// <summary>
        /// Tests the ProvisionAllAsync on <see cref="AzureServiceBusNamespace"/> with real SQL Db data (seed).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_AzureServiceBusNamespace_ProvisionAll_End2End()
        {
            FlexDataConfiguration.Branch = "Main";
            FlexDataConfiguration.Configuration = "MO";
            FlexDataConfiguration.UseNaming(new LegacyFctSbNaming());

            using (var client = ManagementClient.CreateServiceBusClient())
            using (var context = new DevOpsFlexDbContext())
            {
                await context.Components.OfType<AzureServiceBusNamespace>().ProvisionAllAsync(client);
            }
        }

        /// <summary>
        /// Tests the ProvisionAllAsync on <see cref="SqlFirewallRule"/> with real SQL Db data (seed).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_SqlFirewallRule_ProvisionAll_End2End()
        {
            FlexDataConfiguration.Branch = "Main";
            FlexDataConfiguration.Configuration = "MO";

            using (var client = ManagementClient.CreateSqlClient())
            using (var context = new DevOpsFlexDbContext())
            {
                await context.SqlFirewallRules.ProvisionAllAsync(client);
            }
        }

        /// <summary>
        /// Tests the ProvisionAllAsync on <see cref="SqlAzureDb"/> with real SQL Db data (seed).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_SqlAzureDb_ProvisionAll_End2End()
        {
            FlexDataConfiguration.Branch = "Main";
            FlexDataConfiguration.Configuration = "djfr";

            using (var client = ManagementClient.CreateSqlClient())
            using (var context = new DevOpsFlexDbContext())
            {
                await context.Components.OfType<SqlAzureDb>().ProvisionAllAsync(client);
            }
        }

        /// <summary>
        /// Tests the ProvisionAllAsync on <see cref="AzureStorageContainer"/> with real SQL Db data (seed).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_AzureStorageContainer_ProvisionAll_End2End()
        {
            FlexDataConfiguration.Branch = "Main";
            FlexDataConfiguration.Configuration = "djfr";

            using (var client = ManagementClient.CreateStorageClient())
            using (var context = new DevOpsFlexDbContext())
            {
                await context.Components.OfType<AzureStorageContainer>().ProvisionAllAsync(client);
            }
        }

        /// <summary>
        /// Tests the ProvisionAllAsync on <see cref="AzureWebSite"/> with real SQL Db data (seed).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_AzureWebSite_ProvisionAll_End2End()
        {
            FlexDataConfiguration.Branch = "Main";
            FlexDataConfiguration.Configuration = "djfr";

            using (var client = ManagementClient.CreateWebSiteClient())
            using (var context = new DevOpsFlexDbContext())
            {
                await context.Components.OfType<AzureWebSite>().ProvisionAllAsync(client);
            }
        }
    }

    /// <summary>
    /// Names the service bus in the same way we started naming in the early environment creation.
    /// </summary>
    public class LegacyFctSbNaming : IName<AzureServiceBusNamespace>
    {
        public string GetSlotName(AzureServiceBusNamespace component, string branch, string configuration)
        {
            return "nservicebus-" + configuration.ToLower();
        }
    }
}
