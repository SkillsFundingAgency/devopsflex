namespace DevOpsFlex.Azure.Management
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Data;
    using Data.PublishSettings;
    using Microsoft.Azure;
    using Microsoft.WindowsAzure.Management.Compute;
    using Microsoft.WindowsAzure.Management.Network;
    using Microsoft.WindowsAzure.Management.ServiceBus;
    using Microsoft.WindowsAzure.Management.Sql;
    using Microsoft.WindowsAzure.Management.Storage;
    using Microsoft.WindowsAzure.Management.WebSites;

    /// <summary>
    /// Extends the <see cref="DevOpsFlexDbContext"/> with usefull extensions that the devopsflex
    /// activities and commandlets need in order to achieve their execution.
    /// </summary>
    /// <remarks>
    /// These are methods that don't belong in the Data assembly, thus are written has extensions
    /// to the DbContext.
    /// </remarks>
    public static class DevOpsFlexDbContextExtensions
    {
        /// <summary>
        /// Provisions all the services in the <see cref="IEnumerable{T}"/> of <see cref="AzureCloudService"/>.
        /// </summary>
        /// <param name="services">The list of <see cref="AzureCloudService"/> to provision.</param>
        /// <param name="client">The <see cref="ComputeManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task ProvisionAllAsync(this IEnumerable<AzureCloudService> services, ComputeManagementClient client)
        {
            Contract.Requires(services != null);
            Contract.Requires(client != null);

            var tasks = services.Select(
                async s =>
                {
                    await client.CreateServiceIfNotExistsAsync(s);
                });

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Reserves all IPs for services in the <see cref="IEnumerable{T}"/> of <see cref="AzureCloudService"/> that need IP reservation.
        /// </summary>
        /// <param name="services">The list of <see cref="AzureCloudService"/> to reserve IPs for.</param>
        /// <param name="client">The <see cref="ComputeManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task ReserveAllIpsAsync(this IEnumerable<AzureCloudService> services, NetworkManagementClient client)
        {
            Contract.Requires(services != null);
            Contract.Requires(client != null);

            var tasks = services.Where(s => s.ReserveIp)
                                .Select(
                                    async s =>
                                    {
                                        await client.ReserveIpIfNotReservedAsync(s);
                                    });

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Provisions all the namespaces in the <see cref="IEnumerable{T}"/> of <see cref="AzureServiceBusNamespace"/>.
        /// </summary>
        /// <param name="namespaces">The list of <see cref="AzureServiceBusNamespace"/> to provision.</param>
        /// <param name="client">The <see cref="ServiceBusManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task ProvisionAllAsync(this IEnumerable<AzureServiceBusNamespace> namespaces, ServiceBusManagementClient client)
        {
            Contract.Requires(namespaces != null);
            Contract.Requires(client != null);

            var tasks = namespaces.Select(
                async n =>
                {
                    await client.CreateNamespaceIfNotExistsAsync(n);
                });

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Provisions all the databases in the <see cref="IEnumerable{T}"/> of <see cref="SqlAzureDb"/>.
        /// </summary>
        /// <param name="databases">The list of <see cref="SqlAzureDb"/> to provision.</param>
        /// <param name="client">The <see cref="SqlManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task ProvisionAllAsync(this IEnumerable<SqlAzureDb> databases, SqlManagementClient client)
        {
            Contract.Requires(databases != null);
            Contract.Requires(client != null);

            var tasks = databases.Select(
                async d =>
                {
                    await client.CreateDatabaseIfNotExistsAsync(d);
                });

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Provisions all the firewall rules in the <see cref="IEnumerable{T}"/> of <see cref="SqlFirewallRule"/>.
        /// </summary>
        /// <param name="rules">The list of <see cref="SqlFirewallRule"/> to provision.</param>
        /// <param name="client">The <see cref="SqlManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task ProvisionAllAsync(this IEnumerable<SqlFirewallRule> rules, SqlManagementClient client)
        {
            Contract.Requires(rules != null);
            Contract.Requires(client != null);

            var tasks = rules.Select(
                async r =>
                {
                    await client.CreateFirewallRuleIfNotExistsAsync(r);
                });

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Provisions all the storage containers in the <see cref="IEnumerable{T}"/> of <see cref="AzureStorageContainer"/>.
        /// </summary>
        /// <param name="containers">The list of <see cref="AzureStorageContainer"/> to provision.</param>
        /// <param name="client">The <see cref="StorageManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task ProvisionAllAsync(this IEnumerable<AzureStorageContainer> containers, StorageManagementClient client)
        {
            Contract.Requires(containers != null);
            Contract.Requires(client != null);

            var tasks = containers.Select(
                async c =>
                {
                    await client.CreateContainerIfNotExistsAsync(c);
                });

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Provisions all the web sites in the <see cref="IEnumerable{T}"/> of <see cref="AzureWebSite"/>.
        /// </summary>
        /// <param name="sites">The list of <see cref="AzureWebSite"/> to provision.</param>
        /// <param name="client">The <see cref="WebSiteManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task ProvisionAllAsync(this IEnumerable<AzureWebSite> sites, WebSiteManagementClient client)
        {
            Contract.Requires(sites != null);
            Contract.Requires(client != null);

            var tasks = sites.Select(
                async s =>
                {
                    await client.CreateWebSiteIfNotExistsAsync(s);
                });

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Provisions all the components in the <see cref="DevOpsFlexDbContext"/>.
        /// </summary>
        /// <param name="context">The database context that we want to provision components from.</param>
        /// <param name="subscriptionId">The subscription Id where we want to provision in.</param>
        /// <param name="settingsPath">The path to the settings file with the management certificate.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task ProvisionAllAsync(this DevOpsFlexDbContext context, string subscriptionId, string settingsPath)
        {
            Contract.Requires(context != null);
            Contract.Requires(string.IsNullOrEmpty(subscriptionId));
            Contract.Requires(string.IsNullOrEmpty(settingsPath));

            var azureSubscription = new AzureSubscription(settingsPath, subscriptionId);
            var azureCert = new X509Certificate2(Convert.FromBase64String(azureSubscription.ManagementCertificate));

            using (var computeClient = new ComputeManagementClient(new CertificateCloudCredentials(subscriptionId, azureCert)))
            using (var networkClient = new NetworkManagementClient(new CertificateCloudCredentials(subscriptionId, azureCert)))
            using (var sbClient = new ServiceBusManagementClient(new CertificateCloudCredentials(subscriptionId, azureCert)))
            using (var sqlClient = new SqlManagementClient(new CertificateCloudCredentials(subscriptionId, azureCert)))
            using (var storageClient = new StorageManagementClient(new CertificateCloudCredentials(subscriptionId, azureCert)))
            using (var webSiteClient = new WebSiteManagementClient(new CertificateCloudCredentials(subscriptionId, azureCert)))
            {
                var tasks = new[]
                {
                    context.Components.OfType<AzureCloudService>().ToList().ProvisionAllAsync(computeClient),
                    context.Components.OfType<AzureCloudService>().ToList().ReserveAllIpsAsync(networkClient),
                    context.Components.OfType<AzureServiceBusNamespace>().ToList().ProvisionAllAsync(sbClient),
                    context.Components.OfType<SqlAzureDb>().ToList().ProvisionAllAsync(sqlClient),
                    context.Components.OfType<AzureStorageContainer>().ToList().ProvisionAllAsync(storageClient),
                    context.Components.OfType<AzureWebSite>().ToList().ProvisionAllAsync(webSiteClient)
                };

                await Task.WhenAll(tasks);
            }
        }
    }
}
