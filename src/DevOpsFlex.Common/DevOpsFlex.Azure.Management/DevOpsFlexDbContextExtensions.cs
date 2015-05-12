namespace DevOpsFlex.Azure.Management
{
    using System.Data.Entity;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.WindowsAzure.Management.Compute;
    using Microsoft.WindowsAzure.Management.Network;
    using Microsoft.WindowsAzure.Management.ServiceBus;
    using Microsoft.WindowsAzure.Management.Sql;
    using Microsoft.WindowsAzure.Management.Storage;

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
        /// Provisions all the services in the <see cref="IQueryable{T}"/> of <see cref="AzureCloudService"/>.
        /// </summary>
        /// <param name="services">The list of <see cref="AzureCloudService"/> to provision.</param>
        /// <param name="client">The <see cref="ComputeManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task ProvisionAllAsync(this IQueryable<AzureCloudService> services, ComputeManagementClient client)
        {
            Contract.Requires(services != null);
            Contract.Requires(client != null);

            var tasks = (await services.ToListAsync())
                .Select(
                    async s =>
                    {
                        await client.CreateServiceIfNotExistsAsync(s);
                    });

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Reserves all IPs for services in the <see cref="IQueryable"/> of <see cref="AzureCloudService"/> that need IP reservation.
        /// </summary>
        /// <param name="services">The list of <see cref="AzureCloudService"/> to reserve IPs for.</param>
        /// <param name="client">The <see cref="ComputeManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task ReserveAllIpsAsync(this IQueryable<AzureCloudService> services, NetworkManagementClient client)
        {
            Contract.Requires(services != null);
            Contract.Requires(client != null);

            var tasks = (await services.Where(s => s.ReserveIp).ToListAsync())
                .Select(
                    async s =>
                    {
                        await client.ReserveIpIfNotReservedAsync(s);
                    });

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Provisions all the namespaces in the <see cref="IQueryable{T}"/> of <see cref="AzureServiceBusNamespace"/>.
        /// </summary>
        /// <param name="namespaces">The list of <see cref="AzureServiceBusNamespace"/> to provision.</param>
        /// <param name="client">The <see cref="ServiceBusManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task ProvisionAllAsync(this IQueryable<AzureServiceBusNamespace> namespaces, ServiceBusManagementClient client)
        {
            Contract.Requires(namespaces != null);
            Contract.Requires(client != null);

            var tasks = (await namespaces.ToListAsync())
                .Select(
                    async n =>
                    {
                        await client.CreateNamespaceIfNotExistsAsync(n);
                    });

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Provisions all the databases in the <see cref="IQueryable{T}"/> of <see cref="SqlAzureDb"/>.
        /// </summary>
        /// <param name="databases">The list of <see cref="SqlAzureDb"/> to provision.</param>
        /// <param name="client">The <see cref="SqlManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task ProvisionAllAsync(this IQueryable<SqlAzureDb> databases, SqlManagementClient client)
        {
            Contract.Requires(databases != null);
            Contract.Requires(client != null);

            var tasks = (await databases.ToListAsync())
                .Select(
                    async d =>
                    {
                        await client.CreateDatabaseIfNotExistsAsync(d);
                    });

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Provisions all the firewall rules in the <see cref="IQueryable{T}"/> of <see cref="SqlFirewallRule"/>.
        /// </summary>
        /// <param name="rules">The list of <see cref="SqlFirewallRule"/> to provision.</param>
        /// <param name="client">The <see cref="SqlManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task ProvisionAllAsync(this IQueryable<SqlFirewallRule> rules, SqlManagementClient client)
        {
            Contract.Requires(rules != null);
            Contract.Requires(client != null);

            var tasks = (await rules.ToListAsync())
                .Select(
                    async r =>
                    {
                        await client.CreateFirewallRuleIfNotExistsAsync(r);
                    });

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Provisions all the storage containers in the <see cref="IQueryable{T}"/> of <see cref="AzureStorageContainer"/>.
        /// </summary>
        /// <param name="containers">The list of <see cref="AzureStorageContainer"/> to provision.</param>
        /// <param name="client">The <see cref="StorageManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task ProvisionAllAsync(this IQueryable<AzureStorageContainer> containers, StorageManagementClient client)
        {
            Contract.Requires(containers != null);
            Contract.Requires(client != null);

            var tasks = (await containers.ToListAsync())
                .Select(
                    async c =>
                    {
                        await client.CreateContainerIfNotExistsAsync(c);
                    });

            await Task.WhenAll(tasks);
        }
    }
}
