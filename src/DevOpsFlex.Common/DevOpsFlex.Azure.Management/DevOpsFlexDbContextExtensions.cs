namespace DevOpsFlex.Azure.Management
{
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.WindowsAzure.Management.Compute;
    using Microsoft.WindowsAzure.Management.Network;

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
        /// Provisions all the services in the <see cref="IQueryable"/> of <see cref="AzureCloudService"/>.
        /// </summary>
        /// <param name="services">The list of <see cref="AzureCloudService"/> to provision.</param>
        /// <param name="client">The <see cref="ComputeManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task ProvisionAll(this IQueryable<AzureCloudService> services, ComputeManagementClient client)
        {
            var tasks = services.ToList()
                                .Select(
                                    async s =>
                                    {
                                        await client.CheckCreateCloudService(s);
                                    });

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Reserves all IPs for services in the <see cref="IQueryable"/> of <see cref="AzureCloudService"/> that need IP reservation.
        /// </summary>
        /// <param name="services">The list of <see cref="AzureCloudService"/> to reserve IPs for.</param>
        /// <param name="client">The <see cref="ComputeManagementClient"/> that is performing the operation.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task ReserveAllIps(this IQueryable<AzureCloudService> services, NetworkManagementClient client)
        {
            var tasks = services.Where(s => s.ReserveIp)
                                .ToList()
                                .Select(
                                    async s =>
                                    {
                                        await client.CheckCreateReservedIp(s);
                                    });

            await Task.WhenAll(tasks);
        }
    }
}
