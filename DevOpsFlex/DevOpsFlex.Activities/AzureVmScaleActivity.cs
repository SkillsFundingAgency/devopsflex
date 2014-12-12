namespace DevOpsFlex.Activities
{
    using System;
    using System.Activities;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Build.Workflow.Tracking;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Management.Compute;
    using Microsoft.WindowsAzure.Management.Compute.Models;

    [BuildActivity(HostEnvironmentOption.All)]
    [ActivityTracking(ActivityTrackingOption.ActivityOnly)]
    public sealed class AzureVmScaleActivity : CodeActivity
    {
        public InArgument<string> SubscriptionId { get; set; }

        public InArgument<string> ManagementCertificate { get; set; }

        public InArgument<VmScaleDefinition[]> VirtualMachines { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var credentials = new CertificateCloudCredentials(
                SubscriptionId.Get(context),
                new X509Certificate2(Convert.FromBase64String(ManagementCertificate.Get(context))));

            Parallel.ForEach(
                VirtualMachines.Get(context),
                vm =>
                {
                    using (var client = new ComputeManagementClient(credentials))
                    {
                        switch (vm.Size)
                        {
                            case VirtualMachineSize.Stop:

                                client.VirtualMachines.Shutdown(
                                    vm.Name,
                                    vm.Name,
                                    vm.Name,
                                    new VirtualMachineShutdownParameters
                                    {
                                        PostShutdownAction = PostShutdownAction.StoppedDeallocated
                                    });

                                break;

                            case VirtualMachineSize.Small:
                            case VirtualMachineSize.ExtraSmall:
                            case VirtualMachineSize.Large:
                            case VirtualMachineSize.Medium:
                            case VirtualMachineSize.ExtraLarge:
                            case VirtualMachineSize.A5:
                            case VirtualMachineSize.A6:
                            case VirtualMachineSize.A7:
                            case VirtualMachineSize.A8:
                            case VirtualMachineSize.A9:

                                var currentVm = client.VirtualMachines.Get(vm.Name, vm.Name, vm.Name);

                                client.VirtualMachines.Update(
                                    vm.Name,
                                    vm.Name,
                                    vm.Name,
                                    new VirtualMachineUpdateParameters(
                                        currentVm.RoleName,
                                        currentVm.OSVirtualHardDisk)
                                    {
                                        RoleSize = vm.Size.ToAzureString()
                                    });

                                if (client.Deployments.GetByName(vm.Name, vm.Name).Status == DeploymentStatus.Suspended)
                                {
                                    client.VirtualMachines.Start(vm.Name, vm.Name, vm.Name);
                                }

                                break;

                            default:
                                throw new ArgumentOutOfRangeException(
                                    "context",
                                    "Unknown VM Size, this shouldn't happen, but the enumeration value isn't implemented in the acitivity switch");
                        }
                    }
                });
        }
    }
}
