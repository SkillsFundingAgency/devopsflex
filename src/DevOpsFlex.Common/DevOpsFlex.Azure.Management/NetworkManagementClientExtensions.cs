namespace DevOpsFlex.Azure.Management
{
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using System.Xml.XPath;
    using Core;
    using Data;
    using Hyak.Common;
    using Microsoft.WindowsAzure.Management.Network;
    using Microsoft.WindowsAzure.Management.Network.Models;

    /// <summary>
    /// Extends the <see cref="NetworkManagementClient"/> with usefull extensions that the devopsflex
    /// activities and commandlets need in order to achieve their execution.
    /// </summary>
    public static class NetworkManagementClientExtensions
    {
        /// <summary>
        /// Checks for the existence of a specific Azure Reserved IP, if it doesn't exist it will create it.
        /// </summary>
        /// <param name="client">The <see cref="NetworkManagementClient"/> that is performing the operation.</param>
        /// <param name="ipName">The name of the Reserved IP.</param>
        /// <param name="location">The Azure location of the Reserved IP.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CheckCreateReservedIpAsync(this NetworkManagementClient client, string ipName, string location)
        {
            Contract.Requires(client != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(ipName));
            Contract.Requires(!string.IsNullOrWhiteSpace(location));

            NetworkReservedIPGetResponse service = null;

            try
            {
                service = await client.ReservedIPs.GetAsync(ipName);
            }
            catch (CloudException cex)
            {
                if (cex.Error.Code != "ResourceNotFound") throw;
            }

            if (service != null) return;

            await client.ReservedIPs.CreateAsync(
                new NetworkReservedIPCreateParameters
                {
                    Name = ipName,
                    Location = location
                });
        }

        /// <summary>
        /// Checks for the existence of a specific Azure Reserved IP, if it doesn't exist it will create it.
        /// </summary>
        /// <param name="client">The <see cref="NetworkManagementClient"/> that is performing the operation.</param>
        /// <param name="model">The DevOpsFlex rich model object that contains everything there is to know about this cloud service spec.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CheckCreateReservedIpAsync(this NetworkManagementClient client, AzureCloudService model)
        {
            Contract.Requires(client != null);
            Contract.Requires(model != null);
            Contract.Requires(model.System != null);

            var ipName = FlexDataConfiguration.GetNaming<AzureCloudService>()
                                              .GetSlotName(
                                                  model,
                                                  FlexDataConfiguration.Branch,
                                                  FlexDataConfiguration.Configuration)
                         + "-rip";

            await client.CheckCreateReservedIpAsync(ipName, model.System.Location.GetEnumDescription());
        }
    }
}
