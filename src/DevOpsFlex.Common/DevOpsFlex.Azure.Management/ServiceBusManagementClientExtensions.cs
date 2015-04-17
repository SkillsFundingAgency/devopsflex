namespace DevOpsFlex.Azure.Management
{
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using Hyak.Common;
    using Microsoft.WindowsAzure.Management.ServiceBus;
    using Microsoft.WindowsAzure.Management.ServiceBus.Models;

    /// <summary>
    /// Extends the <see cref="ServiceBusManagementClient"/> with usefull extensions that the devopsflex
    /// activities and commandlets need in order to achieve their execution.
    /// </summary>
    public static class ServiceBusManagementClientExtensions
    {
        /// <summary>
        /// Checks for the existence of a specific Azure Web Site, if it doesn't exist it will create it.
        /// </summary>
        /// <param name="client">The <see cref="ServiceBusManagementClient"/> that is performing the operation.</param>
        /// <param name="nsName">The name of the namespace we want to create.</param>
        /// <param name="region">The region where we want to create the namespace.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CheckCreateNamespace(this ServiceBusManagementClient client, string nsName, string region)
        {
            Contract.Requires(client != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(nsName));
            Contract.Requires(!string.IsNullOrWhiteSpace(region));

            ServiceBusNamespaceResponse ns = null;

            try
            {
                ns = await client.Namespaces.GetAsync(nsName);
            }
            catch (CloudException cex)
            {
                // TODO: At the moment of this writing, the ErrorCode in the exception was not in place, so it's a general catch with a contains and that's bad.
                // TODO: This needs to change to the proper trapping of the ErrorCode when they put this in place in the MAML.

                if (!cex.Message.Contains("Request to a downlevel service failed.")) throw;
            }

            if (ns != null) return;

            await client.Namespaces.CreateAsync(nsName, region);
        }
    }
}
