﻿namespace DevOpsFlex.Activities
{
    using System;
    using System.Activities;
    using System.ComponentModel;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Azure.Management;
    using Core;
    using Data.PublishSettings;
    using Designer;
    using Microsoft.Azure;
    using Microsoft.TeamFoundation.Build.Client;
    using Microsoft.TeamFoundation.Build.Workflow.Tracking;
    using Microsoft.WindowsAzure.Management.Compute;

    /// <summary>
    /// A TFS Build Workflow activity that can be used to scale a set of VMs on a specific
    /// Azure subscription. It supports multiple sizes, one per VM, and supports the ability
    /// to wait for all VMs to be ready again.
    /// </summary>
    [BuildActivity(HostEnvironmentOption.All)]
    [ActivityTracking(ActivityTrackingOption.ActivityOnly)]
    [Designer(typeof(AzureVmScaleActivityDesigner))]
    public sealed class AzureVmScaleActivity : CodeActivity
    {
        /// <summary>
        /// Gets and sets the subscription Id that this activity targets.
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets and sets the management certificate that this activity targets.
        /// </summary>
        public string SettingsPath { get; set; }

        /// <summary>
        /// Gets and sets the list of Virtual Machines that we intend to scale.
        /// </summary>
        public InArgument<VmScaleDefinition[]> VirtualMachines { get; set; }

        /// <summary>
        /// Gets and sets the current value of the visual checkbox in the activity designer to swap icons.
        /// </summary>
        public bool VisualCheck { get; set; }

        /// <summary>
        /// Performs the execution of the activity.
        /// </summary>
        /// <param name="context">The execution context under which the activity executes.</param>
        protected override void Execute(CodeActivityContext context)
        {
            var azureSubscription = new AzureSubscription(SettingsPath, SubscriptionId);

            var credentials = new CertificateCloudCredentials(
                SubscriptionId,
                new X509Certificate2(Convert.FromBase64String(azureSubscription.ManagementCertificate)));

            var tasks = VirtualMachines.Get(context).Select(
                async vm =>
                {
                    using (var client = new ComputeManagementClient(credentials))
                    {
                        switch (vm.Size)
                        {
                            case VirtualMachineSize.Stop:
                                await client.DeallocateVmAsync(vm.Name);
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
                                await client.ResizeVmAsync(vm.Name, vm.Size.GetEnumDescription());
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(
                                    nameof(context),
                                    @"Unknown VM Size, this shouldn't happen, but the enumeration value isn't implemented in the acitivity switch");
                        }
                    }
                });

            Task.WhenAll(tasks);
        }
    }
}
