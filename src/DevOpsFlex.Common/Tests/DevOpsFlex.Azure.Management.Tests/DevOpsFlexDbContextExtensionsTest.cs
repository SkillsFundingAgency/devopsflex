namespace DevOpsFlex.Azure.Management.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
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
        /// Tests the ProvisionAll on <see cref="AzureCloudService"/> with real SQL Db data (seed).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_AzureCloudService_ProvisionAll_End2End()
        {
            FlexDataConfiguration.Branch = "Main";
            FlexDataConfiguration.Configuration = "MO";

            using (var client = ManagementClient.CreateComputeClient())
            using (var context = new DevOpsFlexDbContext())
            {
                await context.Components.OfType<AzureCloudService>().ProvisionAll(client);
            }
        }

        /// <summary>
        /// Tests the ReserveAllIps on <see cref="AzureCloudService"/> with real SQL Db data (seed).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_AzureCloudService_ReserveAllIps_End2End()
        {
            FlexDataConfiguration.Branch = "Main";
            FlexDataConfiguration.Configuration = "MO";

            using (var client = ManagementClient.CreateNetworkClient())
            using (var context = new DevOpsFlexDbContext())
            {
                await context.Components.OfType<AzureCloudService>().ReserveAllIps(client);
            }
        }

        /// <summary>
        /// Tests the ProvisionAll on <see cref="AzureServiceBusNamespace"/> with real SQL Db data (seed).
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
                await context.Components.OfType<AzureServiceBusNamespace>().ProvisionAll(client);
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
