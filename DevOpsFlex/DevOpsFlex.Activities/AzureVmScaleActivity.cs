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

    /// <summary>
    /// A TFS Build Workflow activity that can be used to scale a set of VMs on a specific
    /// Azure subscription. It supports multiple sizes, one per VM, and supports the ability
    /// to wait for all VMs to be ready again.
    /// </summary>
    [BuildActivity(HostEnvironmentOption.All)]
    [ActivityTracking(ActivityTrackingOption.ActivityOnly)]
    public sealed class AzureVmScaleActivity : CodeActivity
    {
        /// <summary>
        /// Gets and sets the subscription Id that this activity targets.
        /// </summary>
        public InArgument<string> SubscriptionId { get; set; }

        /// <summary>
        /// Gets and sets the management certificate that this activity targets.
        /// </summary>
        public InArgument<string> ManagementCertificate { get; set; }

        /// <summary>
        /// Gets and sets the list of Virtual Machines that we intend to scale.
        /// </summary>
        public InArgument<VmScaleDefinition[]> VirtualMachines { get; set; }

        /// <summary>
        /// Gets and sets a boolean flag that indicates if we want to wait for all VMs
        /// to be ready again before terminating the activity execution.
        /// This is usefull if we want the build workflow to notify developers that the environment
        /// is ready again, by tracking build notifications.
        /// </summary>
        public InArgument<bool> WaitForVms { get; set; }

        /// <summary>
        /// Gets and sets the timeout time in minutes that we are willing to wait for VMs. After this
        /// period has elapsed, all Tasks awaiting VMs (one task per VM) will be canceled and the activity
        /// execute will complete.
        /// </summary>
        public InArgument<int> TimeoutMinutes { get; set; }

        /// <summary>
        /// Performs the execution of the activity.
        /// </summary>
        /// <param name="context">The execution context under which the activity executes.</param>
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
                                client.DeallocateVm(vm.Name);
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
                                client.ResizeVm(vm.Name, vm.Size.ToAzureString());

                                if (WaitForVms.Get(context))
                                {
                                    client.WaitForVmReady(vm.Name, TimeoutMinutes.Get(context));
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
