namespace DevOpsFlex.Azure.Management.Tests
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using Data;
    using Data.Events;
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
        public void Test_AzureCloudService_ProvisionAll_End2End()
        {
            FlexDataConfiguration.Branch = "Main";
            FlexDataConfiguration.Configuration = "MO";

            using (var client = ManagementClient.CreateComputeClient())
            using (var context = new DevOpsFlexDbContext())
            {
                var tasks = context.Components.OfType<AzureCloudService>().ProvisionAllAsync(client);
                Task.WaitAll(tasks.ToArray());
            }
        }

        /// <summary>
        /// Tests the ReserveAllIpsAsync on <see cref="AzureCloudService"/> with real SQL Db data (seed).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_AzureCloudService_ReserveAllIps_End2End()
        {
            FlexDataConfiguration.Branch = "Main";
            FlexDataConfiguration.Configuration = "MO";

            using (var client = ManagementClient.CreateNetworkClient())
            using (var context = new DevOpsFlexDbContext())
            {
                var tasks = context.Components.OfType<AzureCloudService>().ReserveAllIpsAsync(client);
                Task.WaitAll(tasks.ToArray());
            }
        }

        /// <summary>
        /// Tests the ProvisionAllAsync on <see cref="AzureServiceBusNamespace"/> with real SQL Db data (seed).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_AzureServiceBusNamespace_ProvisionAll_End2End()
        {
            FlexDataConfiguration.Branch = "Main";
            FlexDataConfiguration.Configuration = "MO";
            FlexDataConfiguration.UseNaming(new LegacyFctSbNaming());

            using (var client = ManagementClient.CreateServiceBusClient())
            using (var context = new DevOpsFlexDbContext())
            {
                var tasks = context.Components.OfType<AzureServiceBusNamespace>().ProvisionAllAsync(client);
                Task.WaitAll(tasks.ToArray());
            }
        }

        /// <summary>
        /// Tests the ProvisionAllAsync on <see cref="SqlFirewallRule"/> with real SQL Db data (seed).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_SqlFirewallRule_ProvisionAll_End2End()
        {
            FlexDataConfiguration.Branch = "Main";
            FlexDataConfiguration.Configuration = "MO";

            using (var client = ManagementClient.CreateSqlClient())
            using (var context = new DevOpsFlexDbContext())
            {
                var tasks = context.SqlFirewallRules.ProvisionAllAsync(client);
                Task.WaitAll(tasks.ToArray());
            }
        }

        /// <summary>
        /// Tests the ProvisionAllAsync on <see cref="SqlAzureDb"/> with real SQL Db data (seed).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_SqlAzureDb_ProvisionAll_End2End()
        {
            FlexDataConfiguration.Branch = "Main";
            FlexDataConfiguration.Configuration = "djfr";

            using (var client = ManagementClient.CreateSqlClient())
            using (var context = new DevOpsFlexDbContext())
            {
                var tasks = context.Components.OfType<SqlAzureDb>().ProvisionAllAsync(client);
                Task.WaitAll(tasks.ToArray());
            }
        }

        /// <summary>
        /// Tests the ProvisionAllAsync on <see cref="AzureStorageContainer"/> with real SQL Db data (seed).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_AzureStorageContainer_ProvisionAll_End2End()
        {
            FlexDataConfiguration.Branch = "Main";
            FlexDataConfiguration.Configuration = "djfr";

            using (var client = ManagementClient.CreateStorageClient())
            using (var context = new DevOpsFlexDbContext())
            {
                var tasks = context.Components.OfType<AzureStorageContainer>().ProvisionAllAsync(client);
                Task.WaitAll(tasks.ToArray());
            }
        }

        /// <summary>
        /// Tests the ProvisionAllAsync on <see cref="AzureWebSite"/> with real SQL Db data (seed).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_AzureWebSite_ProvisionAll_End2End()
        {
            FlexDataConfiguration.Branch = "Main";
            FlexDataConfiguration.Configuration = "djfr";

            using (var client = ManagementClient.CreateWebSiteClient())
            using (var context = new DevOpsFlexDbContext())
            {
                var tasks = context.Components.OfType<AzureWebSite>().ProvisionAllAsync(client);
                Task.WaitAll(tasks.ToArray());
            }
        }

        /// <summary>
        /// Tests the ProvisionAllAsync on <see cref="AzureWebSite"/> with real SQL Db data (seed).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_DbContext_ProvisionAll_End2End()
        {
            FlexDataConfiguration.Branch = "Main";
            FlexDataConfiguration.Configuration = "PS";
            FlexStreams.BuildEventsObservable.Subscribe(e => Debug.WriteLine($"[DevOpsFlex] {e.Message}"));

            using (var context = new DevOpsFlexDbContext())
            {
                await context.ProvisionAllAsync(ManagementClient.SubscriptionId, ManagementClient.SettingsPath);
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
