namespace DevOpsFlex.PowerShell
{
    using System;
    using System.IO;
    using System.Management.Automation;
    using System.Security.Cryptography.X509Certificates;
    using Azure.Management;
    using Core;
    using Data.PublishSettings;
    using Microsoft.Azure;
    using Microsoft.WindowsAzure.Management.Compute;
    using Microsoft.WindowsAzure.Management.Compute.Models;
    using Microsoft.WindowsAzure.Management.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Push-CloudServices commandlet implementation.
    /// </summary>
    [Cmdlet(VerbsCommon.Push, "CloudServices")]
    public class PushCloudServices : Cmdlet
    {
        private const string StorageContainer = "servicedeployments";

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
            HelpMessage = "The name of the service that we are targetting for the deployments.")]
        public string ServiceName { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the storage account where we want to send the deployments to.")]
        public string StorageAccountName { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The absolute path to the service package that we want to deploy.")]
        public string PackagePath { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The absolute path to the service configuration file that we want to deploy.")]
        public string ConfigurationPath { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "True if we want to deploy to staging and do a VIP swap when it's done, false if we just want to deploy to production.")]
        public bool VipSwap { get; set; } = false;

        [Parameter(
            Mandatory = false,
            HelpMessage = "True if we want to force a delete of the previous deployments and a new deployment instead of an upgrade if an existing deployment exists.")]
        public bool ForceDelete { get; set; } = false;

        /// <summary>
        /// Processes the Deploy-CloudServices commandlet synchronously.
        /// </summary>
        protected override void ProcessRecord()
        {
            var azureSubscription = new AzureSubscription(SettingsPath, SubscriptionId);
            var azureCert = new X509Certificate2(Convert.FromBase64String(azureSubscription.ManagementCertificate));

            using (var computeClient = new ComputeManagementClient(new CertificateCloudCredentials(SubscriptionId, azureCert)))
            using (var storageClient = new StorageManagementClient(new CertificateCloudCredentials(SubscriptionId, azureCert)))
            {
                var targetSlot = VipSwap ? DeploymentSlot.Production : DeploymentSlot.Staging;

                WriteVerbose("Checking the existence of an existing deployment");
                var deployment = computeClient.GetAzureDeyploymentAsync(ServiceName, targetSlot).Result;

                if (ForceDelete && deployment != null)
                {
                    WriteVerbose("ForceDelete is true and found an existing deployment: Deleting it.");
                    computeClient.Deployments.DeleteBySlotAsync(ServiceName, targetSlot).Wait();
                    deployment = null;
                }

                var storageAccount = storageClient.CreateContainerIfNotExistsAsync(
                    StorageAccountName,
                    StorageContainer,
                    BlobContainerPublicAccessType.Off, SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List).Result;

                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(StorageContainer);

                var blob = container.GetBlockBlobReference(DateTime.Now.ToString("yyyyMMdd_HHmmss_") + Path.GetFileName(PackagePath));
                WriteVerbose($"Uploading the cloud service package to storage account {StorageAccountName} in the {StorageContainer} container.");
                blob.UploadFromFileAsync(PackagePath, FileMode.Open).Wait();

                if (deployment == null)
                {
                    WriteVerbose("Found no previous deployments -> Creating a new Deployment.");
                    computeClient.Deployments.CreateAsync(
                        ServiceName,
                        targetSlot,
                        new DeploymentCreateParameters
                        {
                            Label = ServiceName,
                            Name = $"{ServiceName}{targetSlot.GetEnumDescription()}",
                            PackageUri = blob.Uri,
                            Configuration = File.ReadAllText(ConfigurationPath),
                            StartDeployment = true
                        }).Wait();
                }
                else
                {
                    WriteVerbose("Found a previous deployment -> Updating the current deployment.");
                    computeClient.Deployments.UpgradeBySlotAsync(
                        ServiceName,
                        targetSlot,
                        new DeploymentUpgradeParameters
                        {
                            Label = ServiceName,
                            PackageUri = blob.Uri,
                            Configuration = File.ReadAllText(ConfigurationPath)
                        }).Wait();
                }
            }

            WriteVerbose("All done!");
            base.ProcessRecord();
        }
    }
}
