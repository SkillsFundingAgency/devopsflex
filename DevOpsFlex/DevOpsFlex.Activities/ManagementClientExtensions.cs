namespace DevOpsFlex.Activities
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Management.Compute;
    using Microsoft.WindowsAzure.Management.Compute.Models;

    public static class ManagementClientExtensions
    {
        public static DeploymentGetResponse GetAzureDeyployment(this ComputeManagementClient client, string serviceName, DeploymentSlot slot)
        {
            try
            {
                return client.Deployments.GetBySlot(serviceName, slot);

            }
            catch (CloudException ex)
            {
                if (ex.ErrorCode == "ResourceNotFound")
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