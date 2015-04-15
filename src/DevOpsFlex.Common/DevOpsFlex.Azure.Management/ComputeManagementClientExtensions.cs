namespace DevOpsFlex.Azure.Management
{
    using System.Collections.Generic;
    using System.Linq;
    using Hyak.Common;
    using Microsoft.WindowsAzure.Management.Compute;
    using Microsoft.WindowsAzure.Management.Compute.Models;

    /// <summary>
    /// Extends the <see cref="ComputeManagementClient"/> with usefull extensions that the devopsflex
    /// activities need in order to achieve their execution.
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
        public static void ResizeVm(this ComputeManagementClient client, string name, string size)
        {
            var currentVm = client.VirtualMachines.Get(name, name, name);

            client.VirtualMachines.Update(
                name, name, name,
                new VirtualMachineUpdateParameters(
                    currentVm.RoleName,
                    currentVm.OSVirtualHardDisk)
                {
                    RoleSize = size
                });

            if (client.Deployments.GetByName(name, name).Status == DeploymentStatus.Suspended)
            {
                client.VirtualMachines.Start(name, name, name);
            }
        }

        /// <summary>
        /// Stops a VM in deallocated mode, so that we are no longer paying for it. Deallocating a VM will actually release resources like
        /// public IPs, so allocating the VM again will give you a new set of resources, be aware of changes in this space.
        /// </summary>
        /// <param name="client">The <see cref="ComputeManagementClient"/> that we want to use to connect to the Azure subscription.</param>
        /// <param name="name">The name of the VM that we want to resize.</param>
        public static void DeallocateVm(this ComputeManagementClient client, string name)
        {
            client.VirtualMachines.Shutdown(
                name, name, name,
                new VirtualMachineShutdownParameters
                {
                    PostShutdownAction = PostShutdownAction.StoppedDeallocated
                });
        }

        public static DeploymentGetResponse GetAzureDeyployment(this ComputeManagementClient client, string serviceName, DeploymentSlot slot)
        {
            try
            {
                return client.Deployments.GetBySlot(serviceName, slot);

            }
            catch (CloudException ex)
            {
                if (ex.Error.Code == "ResourceNotFound")
                {
                    return null;
                }

                throw;
            }
        }

        public static IEnumerable<string> GetVms(this ComputeManagementClient client)
        {
            var hostedServices = client.HostedServices.List();

            return hostedServices.Select(s => client.GetAzureDeyployment(s.ServiceName, DeploymentSlot.Production))
                                 .Where(d => d != null && d.Roles.Count > 0)
                                 .SelectMany(d => d.Roles.Where(r => r.RoleType == VirtualMachineRoleType.PersistentVMRole.ToString()))
                                 .Select(r => r.RoleName);
        }
    }
}
