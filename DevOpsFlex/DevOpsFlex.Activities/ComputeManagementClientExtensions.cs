namespace DevOpsFlex.Activities
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Management.Compute;
    using Microsoft.WindowsAzure.Management.Compute.Models;

    /// <summary>
    /// Extends the <see cref="ComputeManagementClient"/> with usefull extensions that the devopsflex
    /// activities need in order to achieve their execution.
    /// </summary>
    public static class ComputeManagementClientExtensions
    {
        /// <summary>
        /// Resizes the target VM into the new size. If the VM is deallocated with will start it
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

        /// <summary>
        /// Waits for a VM to be ready again. It goes into an infite loop checking for <see cref="DeploymentStatus"/> to either be
        /// <see cref="DeploymentStatus.Running"/> or <see cref="DeploymentStatus.Suspended"/>. It supports a timeout cancellation
        /// policy, where it will stop waiting after the timeout period has elapsed.
        /// </summary>
        /// <param name="client">The <see cref="ComputeManagementClient"/> that we want to use to connect to the Azure subscription.</param>
        /// <param name="name">The name of the VM that we want to resize.</param>
        /// <param name="timeoutMinutes"></param>
        public static void WaitForVmReady(this ComputeManagementClient client, string name, int timeoutMinutes = 0)
        {
            var tokenSource = new CancellationTokenSource();
            var deployment = client.Deployments.GetByName(name, name);

            if (timeoutMinutes > 0)
            {
                tokenSource.CancelAfter(TimeSpan.FromMinutes(timeoutMinutes));
            }

            Task.Factory
                .StartNew(
                    async () =>
                    {
                        while (deployment.Status != DeploymentStatus.Running ||
                               deployment.Status != DeploymentStatus.Suspended)
                        {
                            await Task.Delay(5000, tokenSource.Token);
                        }
                    }, tokenSource.Token)
                .Wait(tokenSource.Token);
        }
    }
}
