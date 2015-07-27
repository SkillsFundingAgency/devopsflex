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
        public static IEnumerable<Task> ProvisionAllAsync(this IEnumerable<AzureCloudService> services, ComputeManagementClient client)
        {
            Contract.Requires(services != null);
            Contract.Requires(client != null);

            return services.Select(s =>
                Task.Factory.StartNew(() => client.CreateServiceIfNotExistsAsync(s).Wait()));
        }

        /// <summary>
        /// Reserves all IPs for services in the <see cref="IEnumerable{T}"/> of <see cref="AzureCloudService"/> that need IP reservation.
        /// </summary>
        /// <param name="services">The list of <see cref="AzureCloudService"/> to reserve IPs for.</param>
        /// <param name="client">The <see cref="ComputeManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static IEnumerable<Task> ReserveAllIpsAsync(this IEnumerable<AzureCloudService> services, NetworkManagementClient client)
        {
            Contract.Requires(services != null);
            Contract.Requires(client != null);

            return services.Where(s => s.ReserveIp)
                           .Select(s =>
                               Task.Factory.StartNew(() => client.ReserveIpIfNotReservedAsync(s).Wait()));
        }

        /// <summary>
        /// Provisions all the namespaces in the <see cref="IEnumerable{T}"/> of <see cref="AzureServiceBusNamespace"/>.
        /// </summary>
        /// <param name="namespaces">The list of <see cref="AzureServiceBusNamespace"/> to provision.</param>
        /// <param name="client">The <see cref="ServiceBusManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static IEnumerable<Task> ProvisionAllAsync(this IEnumerable<AzureServiceBusNamespace> namespaces, ServiceBusManagementClient client)
        {
            Contract.Requires(namespaces != null);
            Contract.Requires(client != null);

            return namespaces.Select(n =>
                Task.Factory.StartNew(() => client.CreateNamespaceIfNotExistsAsync(n).Wait()));
        }

        /// <summary>
        /// Provisions all the databases in the <see cref="IEnumerable{T}"/> of <see cref="SqlAzureDb"/>.
        /// </summary>
        /// <param name="databases">The list of <see cref="SqlAzureDb"/> to provision.</param>
        /// <param name="client">The <see cref="SqlManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static IEnumerable<Task> ProvisionAllAsync(this IEnumerable<SqlAzureDb> databases, SqlManagementClient client)
        {
            Contract.Requires(databases != null);
            Contract.Requires(client != null);

            return databases.Select(d =>
                Task.Factory.StartNew(() => client.CreateDatabaseIfNotExistsAsync(d).Wait()));
        }

        /// <summary>
        /// Provisions all the firewall rules in the <see cref="IEnumerable{T}"/> of <see cref="SqlFirewallRule"/>.
        /// </summary>
        /// <param name="rules">The list of <see cref="SqlFirewallRule"/> to provision.</param>
        /// <param name="client">The <see cref="SqlManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static IEnumerable<Task> ProvisionAllAsync(this IEnumerable<SqlFirewallRule> rules, SqlManagementClient client)
        {
            Contract.Requires(rules != null);
            Contract.Requires(client != null);

            return rules.Select(r =>
                Task.Factory.StartNew(() => client.CreateFirewallRuleIfNotExistsAsync(r).Wait()));
        }

        /// <summary>
        /// Provisions all the storage containers in the <see cref="IEnumerable{T}"/> of <see cref="AzureStorageContainer"/>.
        /// </summary>
        /// <param name="containers">The list of <see cref="AzureStorageContainer"/> to provision.</param>
        /// <param name="client">The <see cref="StorageManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static IEnumerable<Task> ProvisionAllAsync(this IEnumerable<AzureStorageContainer> containers, StorageManagementClient client)
        {
            Contract.Requires(containers != null);
            Contract.Requires(client != null);

            return containers.Select(c =>
                Task.Factory.StartNew(() => client.CreateContainerIfNotExistsAsync(c).Wait()));
        }

        /// <summary>
        /// Provisions all the web sites in the <see cref="IEnumerable{T}"/> of <see cref="AzureWebSite"/>.
        /// </summary>
        /// <param name="sites">The list of <see cref="AzureWebSite"/> to provision.</param>
        /// <param name="client">The <see cref="WebSiteManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static IEnumerable<Task> ProvisionAllAsync(this IEnumerable<AzureWebSite> sites, WebSiteManagementClient client)
        {
            Contract.Requires(sites != null);
            Contract.Requires(client != null);

            return sites.Select(s =>
                Task.Factory.StartNew(() => client.CreateWebSiteIfNotExistsAsync(s).Wait()));
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
            Contract.Requires(!string.IsNullOrEmpty(subscriptionId));
            Contract.Requires(!string.IsNullOrEmpty(settingsPath));

            await Task.WhenAll(context.ProvisionAll(subscriptionId, settingsPath));
        }

        /// <summary>
        /// Provisions all the components in the <see cref="DevOpsFlexDbContext"/>.
        /// </summary>
        /// <param name="context">The database context that we want to provision components from.</param>
        /// <param name="subscriptionId">The subscription Id where we want to provision in.</param>
        /// <param name="settingsPath">The path to the settings file with the management certificate.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static Task[] ProvisionAll(this DevOpsFlexDbContext context, string subscriptionId, string settingsPath)
        {
            Contract.Requires(context != null);
            Contract.Requires(!string.IsNullOrEmpty(subscriptionId));
            Contract.Requires(!string.IsNullOrEmpty(settingsPath));

            var azureSubscription = new AzureSubscription(settingsPath, subscriptionId);
            var azureCert = new X509Certificate2(Convert.FromBase64String(azureSubscription.ManagementCertificate));

            var computeClient = new ComputeManagementClient(new CertificateCloudCredentials(subscriptionId, azureCert));
            var networkClient = new NetworkManagementClient(new CertificateCloudCredentials(subscriptionId, azureCert));
            var sbClient = new ServiceBusManagementClient(new CertificateCloudCredentials(subscriptionId, azureCert));
            var sqlClient = new SqlManagementClient(new CertificateCloudCredentials(subscriptionId, azureCert));
            var storageClient = new StorageManagementClient(new CertificateCloudCredentials(subscriptionId, azureCert));
            var webSiteClient = new WebSiteManagementClient(new CertificateCloudCredentials(subscriptionId, azureCert));

            var foo = context.Components.Include(o => o.System).OfType<AzureCloudService>().ToList().ProvisionAllAsync(computeClient)
                             .Concat(context.Components.Include(o => o.System).OfType<AzureCloudService>().ToList().ReserveAllIpsAsync(networkClient))
                             .Concat(context.Components.Include(o => o.System).OfType<AzureServiceBusNamespace>().ToList().ProvisionAllAsync(sbClient))
                             .Concat(context.Components.Include(o => o.System).OfType<SqlAzureDb>().ToList().ProvisionAllAsync(sqlClient))
                             .Concat(context.Components.Include(o => o.System).OfType<AzureStorageContainer>().ToList().ProvisionAllAsync(storageClient))
                             .Concat(context.Components.Include(o => o.System).OfType<AzureWebSite>().ToList().ProvisionAllAsync(webSiteClient))
                             .ToArray();

            return foo;
        }
    }
}
