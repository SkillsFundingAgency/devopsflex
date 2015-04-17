﻿namespace DevOpsFlex.Azure.Management.Tests
{
    using System;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
    using Core;
    using Data;
    using Data.PublishSettings;
    using Microsoft.Azure;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.ServiceBus;

    [TestClass]
    public class ServiceBusManagementClientExtensionsTest
    {

        #region Integration Tests

        /// <summary>
        /// Specifies the relative or absolute path to the publish settings file for the target subscription.
        /// </summary>
        private const string SettingsPath = @"C:\PublishSettings\sfa_beta.publishsettings";

        /// <summary>
        /// Specifies the subscription Id that we want to target.
        /// This subscription needs to be defined and found in the publish settings file.
        /// </summary>
        private const string SubscriptionId = "102d951b-78c0-4e48-80d4-a9c13baca2ad";

        /// <summary>
        /// Tests the creation of a Service Bus namespace that doesn't exist (puts a Guid part in the namespace name).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_CheckCreateNamespace_WithNewNamespace()
        {
            using (var client = CreateClient())
            {
                var region = ServiceBusRegions.NorthEurope.GetEnumDescription();
                var nsName = "fct-" + Guid.NewGuid().ToString().Split('-').Last();

                try
                {
                    await client.CheckCreateNamespace(nsName, region);

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
            using (var client = CreateClient())
            {
                var region = ServiceBusRegions.NorthEurope.GetEnumDescription();
                var nsName = "fct-" + Guid.NewGuid().ToString().Split('-').Last();

                try
                {
                    await client.Namespaces.CreateAsync(nsName, region);
                    await client.CheckCreateNamespace(nsName, region);

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

        /// <summary>
        /// Creates a standard <see cref="ServiceBusManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </summary>
        /// <returns>
        /// A standard <see cref="ServiceBusManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </returns>
        private static ServiceBusManagementClient CreateClient()
        {
            var azureSubscription = new AzureSubscription(SettingsPath, SubscriptionId);

            return new ServiceBusManagementClient(
                new CertificateCloudCredentials(
                    SubscriptionId,
                    new X509Certificate2(Convert.FromBase64String(azureSubscription.ManagementCertificate))));
        }
    }
}
