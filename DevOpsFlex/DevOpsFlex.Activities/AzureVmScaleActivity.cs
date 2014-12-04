namespace DevOpsFlex.Activities
{
    using System;
    using System.Activities;
    using System.Security.Cryptography.X509Certificates;
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

        public InArgument<string[]> VirtualMachines { get; set; }

        public InArgument<VirtualMachineSize> VirtualMachineSize { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var credentials = new CertificateCloudCredentials(
                SubscriptionId.Get(context),
                new X509Certificate2(Convert.FromBase64String(ManagementCertificate.Get(context))));

            using (var client = new ComputeManagementClient(credentials))
            {
                foreach (var vm in VirtualMachines.Get(context))
                {
                    var currentVm = client.VirtualMachines.Get(vm, vm, vm);
                    var parameters = new VirtualMachineUpdateParameters(currentVm.RoleName, currentVm.OSVirtualHardDisk)
                    {
                        RoleSize = VirtualMachineSize.Get(context).ToAzureString()
                    };

                    client.VirtualMachines.Update(vm, vm, vm, parameters);
                }
            }
        }
    }
}
