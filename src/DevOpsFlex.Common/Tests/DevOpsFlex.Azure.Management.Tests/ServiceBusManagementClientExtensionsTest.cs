namespace DevOpsFlex.Azure.Management.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Core;
    using Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.ServiceBus;

    /// <summary>
    /// Contains tests that target the <see cref="ServiceBusManagementClientExtensions"/> extension class
    /// for the <see cref="ServiceBusManagementClient"/>.
    /// </summary>
    [TestClass]
    public class ServiceBusManagementClientExtensionsTest
    {

        #region Integration Tests

        /// <summary>
        /// Tests the creation of a Service Bus namespace that doesn't exist (puts a Guid part in the namespace name).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_CheckCreateNamespace_WithNewNamespace()
        {
            using (var client = ManagementClient.CreateServiceBusClient())
            {
                var region = ServiceBusRegions.NorthEurope.GetEnumDescription();
                var nsName = "fct-" + Guid.NewGuid().ToString().Split('-').Last();

                try
                {
                    await client.CheckCreateNamespaceAsync(nsName, region);

                    var ns = await client.Namespaces.GetAsync(nsName);
                    Assert.IsNotNull(ns);
                }
                finally
                {
                    DeleteNamespace(client, nsName, 20);
                }
            }
        }

        /// <summary>
        /// Tests the creation of a Service Bus namespace that doesn't exist (puts a Guid part in the namespace name).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_CheckCreateNamespace_WithExistingNamespace()
        {
            using (var client = ManagementClient.CreateServiceBusClient())
            {
                var region = ServiceBusRegions.NorthEurope.GetEnumDescription();
                var nsName = "fct-" + Guid.NewGuid().ToString().Split('-').Last();

                try
                {
                    await client.Namespaces.CreateAsync(nsName, region);
                    await client.CheckCreateNamespaceAsync(nsName, region);

                    var ns = await client.Namespaces.GetAsync(nsName);
                    Assert.IsNotNull(ns);
                }
                finally
                {
                    DeleteNamespace(client, nsName, 20);
                }
            }
        }

        /// <summary>
        /// Deletes a Service Bus namespace. Can handle transitioning states by recursing itself
        /// on exception.
        /// </summary>
        /// <param name="client">The <see cref="ServiceBusManagementClient"/> that is performing the operation.</param>
        /// <param name="nsName">The name of the namespace that we want to delete.</param>
        /// <param name="retries">Number of retries for "Active" state before giving up on deletion.</param>
        private static void DeleteNamespace(IServiceBusManagementClient client, string nsName, int retries)
        {
            if (retries-- < 0)
                throw new InvalidOperationException(
                    string.Format(
                        "Reached the maximum number of retries before I could delete the namespace. The namespace {0} needs to be deleted manually",
                        nsName));

            try
            {
                client.Namespaces.Delete(nsName);
            }
            catch (Exception)
            {
                Thread.Sleep(10000);
                DeleteNamespace(client, nsName, retries);
            }
        }

        #endregion

    }
}
