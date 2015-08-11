namespace DevOpsFlex.PowerShell
{
    using System;
    using System.Linq;
    using System.Management.Automation;
    using System.Security.Cryptography.X509Certificates;
    using Azure.Management;
    using Core;
    using Data.PublishSettings;
    using Microsoft.Azure;
    using Microsoft.WindowsAzure.Management.Compute;
    using Microsoft.WindowsAzure.Management.Network;

    /// <summary>
    /// Set-ReservedIpConfiguration commandlet implementation.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "ReservedIpConfiguration")]
    public class SetReservedIpConfiguration : AsyncCmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The Subscription Id that we are targetting for the deployments.")]
        public string SubscriptionId { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The absolute path to the Azure publish settings file.")]
        public string SettingsPath { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The SqlConnectionString to the DevOpsFlex database.")]
        public string SqlConnectionString { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the branch we want to target.")]
        public string Branch { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The .Net project configuration we want to target.")]
        public string Configuration { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The local path to where the TFS mapping is.")]
        public string TfsProjectMapPath { get; set; }

        /// <summary>
        /// Processes the Set-ReservedIpConfiguration commandlet synchronously.
        /// </summary>
        protected override void ProcessRecord()
        {
            var azureSubscription = new AzureSubscription(SettingsPath, SubscriptionId);
            var azureCert = new X509Certificate2(Convert.FromBase64String(azureSubscription.ManagementCertificate));
            var credentials = new CertificateCloudCredentials(SubscriptionId, azureCert);

            using (var computeClient = new ComputeManagementClient(credentials))
            using (var networkClient = new NetworkManagementClient(credentials))
            {
                //#1 Find all the Cloud Servies
                var services = computeClient.HostedServices.List().Select(s => s.ServiceName);

                //#2 Find all the Reserved IPs
                var reservedIps = networkClient.ReservedIPs.List().Select(i => i.Name);

                // and match them to Cloud Services using the naming conventions
                var serviceMatches = services.Where(s => reservedIps.Contains(s + "-rip"));

                //#3 Foreach match

                    //#A Look at the proper service configuration and check if the Reserved IP assignment is there

                    //#B If the assignment isn't there, open the XML service configuration and add it

                    //[NOTE] Do NOT DO anything TFS wise, it is then up to the user to checkin the changes or not, this code shouldn't do anything against TFS
            }
        }
    }
}
