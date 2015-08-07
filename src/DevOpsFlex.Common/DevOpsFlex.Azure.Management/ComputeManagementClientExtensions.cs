namespace DevOpsFlex.Azure.Management
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Data;
    using Data.Events;
    using Hyak.Common;
    using Microsoft.WindowsAzure.Management.Compute;
    using Microsoft.WindowsAzure.Management.Compute.Models;

    /// <summary>
    /// Extends the <see cref="ComputeManagementClient"/> with usefull extensions that the devopsflex
    /// activities and commandlets need in order to achieve their execution.
    /// </summary>
    public static class ComputeManagementClientExtensions
    {
        /// <summary>
        /// Resizes the target VM into the new size. If the VM is deallocated it will start the VM
        /// after it resizes it.
        /// </summary>
        /// <param name="client">The <see cref="ComputeManagementClient"/> that we want to use to connect to the Azure subscription.</param>
        /// <param name="name">The name of the VM that we want to resize.</param>
        /// <param name="size">The target size for the VM.</param>
        public static async Task ResizeVmAsync(this ComputeManagementClient client, string name, string size)
        {
            Contract.Requires(client != null);

            var currentVm = await client.VirtualMachines.GetAsync(name, name, name);

            await client.VirtualMachines.UpdateAsync(
                name, name, name,
                new VirtualMachineUpdateParameters(
                    currentVm.RoleName,
                    currentVm.OSVirtualHardDisk)
                {
                    RoleSize = size
                });

            if (client.Deployments.GetByName(name, name).Status == DeploymentStatus.Suspended)
            {
                await client.VirtualMachines.StartAsync(name, name, name);
            }
        }

        /// <summary>
        /// Stops a VM in deallocated mode, so that we are no longer paying for it. Deallocating a VM will actually release resources like
        /// public IPs, so allocating the VM again will give you a new set of resources, be aware of changes in this space.
        /// </summary>
        /// <param name="client">The <see cref="ComputeManagementClient"/> that we want to use to connect to the Azure subscription.</param>
        /// <param name="name">The name of the VM that we want to resize.</param>
        public static async Task DeallocateVmAsync(this ComputeManagementClient client, string name)
        {
            Contract.Requires(client != null);

            await client.VirtualMachines.ShutdownAsync(
                name, name, name,
                new VirtualMachineShutdownParameters
                {
                    PostShutdownAction = PostShutdownAction.StoppedDeallocated
                });
        }

        /// <summary>
        /// Gets the AzureDeployment for a specifc slot on a cloud service.
        /// </summary>
        /// <param name="client">The <see cref="ComputeManagementClient"/> that is performing the operation.</param>
        /// <param name="serviceName">The name of the cloud service.</param>
        /// <param name="slot">The Cloud Service slot.</param>
        /// <returns>The cloud service deployment.</returns>
        public static async Task<DeploymentGetResponse> GetAzureDeyploymentAsync(this ComputeManagementClient client, string serviceName, DeploymentSlot slot)
        {
            Contract.Requires(client != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(serviceName));

            try
            {
                return await client.Deployments.GetBySlotAsync(serviceName, slot);
            }
            catch (CloudException cex)
            {
                if (cex.Error.Code == "ResourceNotFound")
                {
                    return null;
                }

                throw;
            }
        }

        /// <summary>
        /// Gets a list of VMs in the target subscription.
        /// </summary>
        /// <param name="client">The <see cref="ComputeManagementClient"/> that is performing the operation.</param>
        /// <returns>The full list of VMs in the subscription.</returns>
        /// <remarks>
        /// While debugging the enumeration might not be visible if tasks haven't all completed.
        /// To change this behaviour just force the enumeration after calling this by using the <see cref="Enumerable.ToList{T}(IEnumerable{T})"/>.
        /// </remarks>
        public static async Task<IEnumerable<string>> ListVmsAsync(this ComputeManagementClient client)
        {
            Contract.Requires(client != null);

            return (await client.HostedServices.ListAsync())
                .Select(async s => await client.GetAzureDeyploymentAsync(s.ServiceName, DeploymentSlot.Production))
                .Select(t => t.Result)
                .Where(d => d != null && d.Roles.Count > 0)
                .SelectMany(d => d.Roles.Where(r => r.RoleType == VirtualMachineRoleType.PersistentVMRole.ToString()))
                .Select(r => r.RoleName);
        }

        /// <summary>
        /// Checks for the existence of a specific Cloud Service, if it doesn't exist it will create it.
        /// </summary>
        /// <param name="client">The <see cref="ComputeManagementClient"/> that is performing the operation.</param>
        /// <param name="parameters">The <see cref="HostedServiceCreateParameters"/> that define the service we want to create.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CreateServiceIfNotExistsAsync(this ComputeManagementClient client, HostedServiceCreateParameters parameters)
        {
            Contract.Requires(client != null);
            Contract.Requires(parameters != null);

            HostedServiceGetResponse service = null;
            FlexStreams.Publish(new CheckIfExistsEvent(AzureResource.CloudService, parameters.ServiceName));

            try
            {
                service = await client.HostedServices.GetAsync(parameters.ServiceName);
            }
            catch (CloudException cex)
            {
                if (cex.Error.Code != "ResourceNotFound") throw;
            }

            if (service != null)
            {
                FlexStreams.Publish(new FoundExistingEvent(AzureResource.CloudService, parameters.ServiceName));
                return;
            }

            await client.HostedServices.CreateAsync(parameters);
            FlexStreams.Publish(new ProvisionEvent(AzureResource.CloudService, parameters.ServiceName));
        }

        /// <summary>
        /// Checks for the existence of a specific Cloud Service, if it doesn't exist it will create it.
        /// </summary>
        /// <param name="client">The <see cref="ComputeManagementClient"/> that is performing the operation.</param>
        /// <param name="model">The DevOpsFlex rich model object that contains everything there is to know about this cloud service spec.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CreateServiceIfNotExistsAsync(this ComputeManagementClient client, AzureCloudService model)
        {
            Contract.Requires(client != null);
            Contract.Requires(model != null);

            await client.CreateServiceIfNotExistsAsync(model.AzureParameters);
        }

        /// <summary>
        /// Adds the DevOpsFlex PaaS Diagnostics extension if it doesn't exist yet on the service.
        /// </summary>
        /// <param name="client">The <see cref="ComputeManagementClient"/> that is performing the operation.</param>
        /// <param name="serviceName">The name of the cloud service.</param>
        /// <param name="publicConfiguration">The public configuration XML that is to be applied to the extension.</param>
        /// <returns></returns>
        public static async Task AddDiagnosticsExtensionIfNotExistsAsync(this ComputeManagementClient client, string serviceName, string publicConfiguration)
        {
            var diagnosticsExtensions = (await client.HostedServices.ListExtensionsAsync(serviceName)).Where(e => e.Type == "PaaSDiagnostics").ToList();
            var extension = diagnosticsExtensions.FirstOrDefault(e => e.Id == FlexConfiguration.FlexDiagnosticsExtensionId);

            if (extension == null)
            {
                foreach (var ext in diagnosticsExtensions)
                {
                    await client.HostedServices.DeleteExtensionAsync(serviceName, ext.Id);
                }

                var storageAccount = Regex.Match(publicConfiguration, "<StorageAccount>([^<]*)</StorageAccount>", RegexOptions.Multiline)
                                          .Groups.OfType<Group>()
                                          .First(g => g.GetType() == typeof(Group))
                                          .Value;

                client.HostedServices.AddExtension(serviceName, new HostedServiceAddExtensionParameters
                {
                    Id = FlexConfiguration.FlexDiagnosticsExtensionId,
                    PublicConfiguration = publicConfiguration,
                    PrivateConfiguration = $"<?xml version=\"1.0\" encoding=\"utf - 8\"?><PrivateConfig xmlns=\"http://schemas.microsoft.com/ServiceHosting/2010/10/DiagnosticsConfiguration\"><StorageAccount name=\"{storageAccount}\" /></PrivateConfig>",
                    ProviderNamespace = "Microsoft.Azure.Diagnostics",
                    Type = "PaaSDiagnostics", // TODO: PaaSAntimalware
                    Version = "1.*"
                });
            }
        }
    }
}
