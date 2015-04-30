namespace DevOpsFlex.Azure.Management.Tests
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using Data.PublishSettings;
    using Microsoft.Azure;
    using Microsoft.WindowsAzure.Management.Compute;
    using Microsoft.WindowsAzure.Management.Network;
    using Microsoft.WindowsAzure.Management.ServiceBus;
    using Microsoft.WindowsAzure.Management.Sql;
    using Microsoft.WindowsAzure.Management.Storage;
    using Microsoft.WindowsAzure.Management.WebSites;

    /// <summary>
    /// Contains utility methods to generate all the MAML clients needed by the integration tests.
    /// </summary>
    public static class ManagementClient
    {
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
        /// Creates a standard <see cref="ComputeManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </summary>
        /// <returns>
        /// A standard <see cref="ComputeManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </returns>
        internal static ComputeManagementClient CreateComputeClient()
        {
            var azureSubscription = new AzureSubscription(SettingsPath, SubscriptionId);

            return new ComputeManagementClient(
                new CertificateCloudCredentials(
                    SubscriptionId,
                    new X509Certificate2(Convert.FromBase64String(azureSubscription.ManagementCertificate))));
        }

        /// <summary>
        /// Creates a standard <see cref="WebSiteManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </summary>
        /// <returns>
        /// A standard <see cref="WebSiteManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </returns>
        internal static WebSiteManagementClient CreateWebSiteClient()
        {
            var azureSubscription = new AzureSubscription(SettingsPath, SubscriptionId);

            return new WebSiteManagementClient(
                new Microsoft.WindowsAzure.CertificateCloudCredentials(
                    SubscriptionId,
                    new X509Certificate2(Convert.FromBase64String(azureSubscription.ManagementCertificate))));
        }

        /// <summary>
        /// Creates a standard <see cref="ServiceBusManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </summary>
        /// <returns>
        /// A standard <see cref="ServiceBusManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </returns>
        internal static ServiceBusManagementClient CreateServiceBusClient()
        {
            var azureSubscription = new AzureSubscription(SettingsPath, SubscriptionId);

            return new ServiceBusManagementClient(
                new CertificateCloudCredentials(
                    SubscriptionId,
                    new X509Certificate2(Convert.FromBase64String(azureSubscription.ManagementCertificate))));
        }

        /// <summary>
        /// Creates a standard <see cref="SqlManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </summary>
        /// <returns>
        /// A standard <see cref="SqlManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </returns>
        internal static SqlManagementClient CreateSqlClient()
        {
            var azureSubscription = new AzureSubscription(SettingsPath, SubscriptionId);

            return new SqlManagementClient(
                new CertificateCloudCredentials(
                    SubscriptionId,
                    new X509Certificate2(Convert.FromBase64String(azureSubscription.ManagementCertificate))));
        }

        /// <summary>
        /// Creates a standard <see cref="NetworkManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </summary>
        /// <returns>
        /// A standard <see cref="NetworkManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </returns>
        internal static NetworkManagementClient CreateNetworkClient()
        {
            var azureSubscription = new AzureSubscription(SettingsPath, SubscriptionId);

            return new NetworkManagementClient(
                new CertificateCloudCredentials(
                    SubscriptionId,
                    new X509Certificate2(Convert.FromBase64String(azureSubscription.ManagementCertificate))));
        }

        /// <summary>
        /// Creates a standard <see cref="StorageManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </summary>
        /// <returns>
        /// A standard <see cref="StorageManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </returns>
        internal static StorageManagementClient CreateStorageClient()
        {
            var azureSubscription = new AzureSubscription(SettingsPath, SubscriptionId);

            return new StorageManagementClient(
                new CertificateCloudCredentials(
                    SubscriptionId,
                    new X509Certificate2(Convert.FromBase64String(azureSubscription.ManagementCertificate))));
        }
    }
}
