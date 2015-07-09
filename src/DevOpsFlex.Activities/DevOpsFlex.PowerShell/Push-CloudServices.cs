namespace DevOpsFlex.PowerShell
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Management.Automation;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
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
    public class PushCloudServices : AsyncCmdlet
    {
        /// <summary>
        /// 
        /// </summary>
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
        public string[] ServiceNames { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The absolute path to the service package that we want to deploy.")]
        public string[] PackagePaths { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The absolute path to the service configuration file that we want to deploy.")]
        public string[] ConfigurationPaths { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the storage account where we want to send the deployments to.")]
        public string StorageAccountName { get; set; }

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
                var targetSlot = VipSwap ? DeploymentSlot.Staging : DeploymentSlot.Production;

                var storageAccount = storageClient.CreateContainerIfNotExistsAsync(
                    StorageAccountName,
                    StorageContainer,
                    BlobContainerPublicAccessType.Off, SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List).Result;

                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(StorageContainer);

                ProcessAsyncWork(ServiceNames.Select((t, i) => DeployCloudService(computeClient, container, t, PackagePaths[i], ConfigurationPaths[i], targetSlot))
                                             .ToArray());
            }

            WriteVerbose("All done!");
            base.ProcessRecord();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="computeClient"></param>
        /// <param name="container"></param>
        /// <param name="serviceName"></param>
        /// <param name="packagePath"></param>
        /// <param name="configurationPath"></param>
        /// <param name="targetSlot"></param>
        /// <returns></returns>
        private async Task DeployCloudService(
            ComputeManagementClient computeClient,
            CloudBlobContainer container,
            string serviceName,
            string packagePath,
            string configurationPath,
            DeploymentSlot targetSlot)
        {
            ThreadAdapter.QueueObject($"[{serviceName}] Checking the existence of an existing deployment");
            var deployment = await computeClient.GetAzureDeyploymentAsync(serviceName, targetSlot);

            if (ForceDelete && deployment != null)
            {
                ThreadAdapter.QueueObject($"[{serviceName}] ForceDelete is true and found an existing deployment: Deleting it.");
                await computeClient.Deployments.DeleteBySlotAsync(serviceName, targetSlot);
                deployment = null;
            }

            var blob = container.GetBlockBlobReference(DateTime.Now.ToString("yyyyMMdd_HHmmss_") + Path.GetFileName(packagePath));
            ThreadAdapter.QueueObject($"[{serviceName}] Uploading the cloud service package to storage account {StorageAccountName} in the {StorageContainer} container.");
            await blob.UploadFromFileAsync(packagePath, FileMode.Open);

            if (deployment == null)
            {
                ThreadAdapter.QueueObject($"[{serviceName}] Found no previous deployments -> Creating a new Deployment.");
                await computeClient.Deployments.CreateAsync(
                    serviceName,
                    targetSlot,
                    new DeploymentCreateParameters
                    {
                        Label = serviceName,
                        Name = $"{serviceName}{targetSlot.GetEnumDescription()}",
                        PackageUri = blob.Uri,
                        Configuration = File.ReadAllText(configurationPath),
                        StartDeployment = true
                    });
            }
            else
            {
                ThreadAdapter.QueueObject($"[{serviceName}] Found a previous deployment -> Updating the current deployment.");
                await computeClient.Deployments.UpgradeBySlotAsync(
                    serviceName,
                    targetSlot,
                    new DeploymentUpgradeParameters
                    {
                        Label = serviceName,
                        PackageUri = blob.Uri,
                        Configuration = File.ReadAllText(configurationPath)
                    });
            }

            ThreadAdapter.QueueObject($"[{serviceName}] Finished deployment.");
        }
    }
}
