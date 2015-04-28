namespace DevOpsFlex.Azure.Management
{
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using Core;
    using Data;
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
        /// <param name="sbName">The name of the namespace we want to create.</param>
        /// <param name="region">The region where we want to create the namespace.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CheckCreateNamespace(this ServiceBusManagementClient client, string sbName, string region)
        {
            Contract.Requires(client != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(sbName));
            Contract.Requires(!string.IsNullOrWhiteSpace(region));

            ServiceBusNamespaceResponse sb = null;

            try
            {
                sb = await client.Namespaces.GetAsync(sbName);
            }
            catch (CloudException cex)
            {
                // TODO: At the moment of this writing, the ErrorCode in the exception was not in place, so it's a general catch with a contains and that's bad.
                // TODO: This needs to change to the proper trapping of the ErrorCode when they put this in place in the MAML.

                if (!cex.Message.Contains("Request to a downlevel service failed.")) throw;
            }

            if (sb != null) return;

            await client.Namespaces.CreateAsync(sbName, region);
        }

        /// <summary>
        /// Checks for the existence of a specific Azure Web Site, if it doesn't exist it will create it.
        /// </summary>
        /// <param name="client">The <see cref="ServiceBusManagementClient"/> that is performing the operation.</param>
        /// <param name="model">The DevOpsFlex rich model object that contains everything there is to know about this service bus spec.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CheckCreateNamespace(this ServiceBusManagementClient client, AzureServiceBusNamespace model)
        {
            Contract.Requires(client != null);
            Contract.Requires(model != null);

            await client.CheckCreateNamespace(
                FlexDataConfiguration.GetNaming<AzureServiceBusNamespace>()
                                     .GetSlotName(
                                         model,
                                         FlexDataConfiguration.Branch,
                                         FlexDataConfiguration.Configuration),
                model.Region.GetEnumDescription());
        }
    }
}
