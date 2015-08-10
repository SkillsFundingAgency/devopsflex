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
    using Data;
    using Data.Events;
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
        /// Contains the constant value of the container name for deployments in the storage account.
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
            HelpMessage = "The absolute path to the service packages that we want to deploy.")]
        public string[] PackagePaths { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The absolute path to the service configuration files that we want to deploy.")]
        public string[] ConfigurationPaths { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "The absolute path to the WAD definition files that we want to deploy.")]
        public string[] DiagnosticsConfigurationPaths { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the storage account where we want to send the deployments to.")]
        public string StorageAccountName { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "True if we want to deploy to staging and do a VIP swap when it's done, false if we just want to deploy to production.")]
        public SwitchParameter VipSwap { get; set; } = false;

        [Parameter(
            Mandatory = false,
            HelpMessage = "True if we want to delete the Staging deployment after we do the VIP swap, false if we just want to let the staging (old production) deployment active after the swap.")]
        public SwitchParameter DeleteStaging { get; set; } = false;

        [Parameter(
            Mandatory = false,
            HelpMessage = "True if we want to force a delete of the previous deployments and a new deployment instead of an upgrade if an existing deployment exists.")]
        public SwitchParameter ForceDelete { get; set; } = false;

        /// <summary>
        /// Processes the Push-CloudServices commandlet synchronously.
        /// </summary>
        protected override void ProcessRecord()
        {
            var azureSubscription = new AzureSubscription(SettingsPath, SubscriptionId);
            var azureCert = new X509Certificate2(Convert.FromBase64String(azureSubscription.ManagementCertificate));
            var credentials = new CertificateCloudCredentials(SubscriptionId, azureCert);
            FlexStreams.UseThreadQueue(ThreadAdapter);

            using (EventStream.Subscribe(e => WriteObject(e.Message)))
            using (var computeClient = new ComputeManagementClient(credentials))
            using (var storageClient = new StorageManagementClient(credentials))
            {
                var targetSlot = VipSwap ? DeploymentSlot.Staging : DeploymentSlot.Production;

                var storageAccount = storageClient.CreateContainerIfNotExistsAsync(
                    StorageAccountName,
                    StorageContainer,
                    BlobContainerPublicAccessType.Off, SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List).Result;

                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(StorageContainer);

                ProcessAsyncWork(ServiceNames.Select((t, i) => DeployCloudServiceAsync(computeClient, container, t, PackagePaths[i], ConfigurationPaths[i], DiagnosticsConfigurationPaths?[i], targetSlot))
                                             .ToArray());
            }

            WriteObject("All done!");
            base.ProcessRecord();
        }

        private async Task DeployCloudServiceAsync(
            ComputeManagementClient computeClient,
            CloudBlobContainer container,
            string serviceName,
            string packagePath,
            string configurationPath,
            string diagnosticsConfigurationPath,
            DeploymentSlot targetSlot)
        {
            ThreadAdapter.QueueObject(new BuildEvent(BuildEventType.Information, BuildEventImportance.Medium,
                $"[{serviceName}] Checking the existence of an existing deployment"));

            var deployment = await computeClient.GetAzureDeyploymentAsync(serviceName, targetSlot);

            if (ForceDelete && deployment != null)
            {
                ThreadAdapter.QueueObject(new BuildEvent(BuildEventType.Information, BuildEventImportance.Medium,
                    $"[{serviceName}] ForceDelete is true and found an existing deployment: Deleting it."));

                await computeClient.Deployments.DeleteBySlotAsync(serviceName, targetSlot);
                deployment = null;
            }

            var blob = container.GetBlockBlobReference(DateTime.Now.ToString("yyyyMMdd_HHmmss_") + Path.GetFileName(packagePath));

            ThreadAdapter.QueueObject(new BuildEvent(BuildEventType.Information, BuildEventImportance.Medium,
                $"[{serviceName}] Uploading the cloud service package to storage account {StorageAccountName} in the {StorageContainer} container."));

            await blob.UploadFromFileAsync(packagePath, FileMode.Open);

            if (diagnosticsConfigurationPath != null)
            {
                ThreadAdapter.QueueObject(new BuildEvent(BuildEventType.Information, BuildEventImportance.Medium,
                    $"[{serviceName}] Checking the Cloud Service for the PaaS Diagnostics extension -> Creating it if it doesn't exist."));

                var diagnosticsConfiguration = File.ReadAllText(diagnosticsConfigurationPath);
                var diagnosticsCreated = await computeClient.AddDiagnosticsExtensionIfNotExistsAsync(serviceName, diagnosticsConfiguration);

                if (!diagnosticsCreated)
                {
                    diagnosticsConfigurationPath = null;
                }
            }

            if (deployment == null)
            {
                ThreadAdapter.QueueObject(new BuildEvent(BuildEventType.Information, BuildEventImportance.Medium,
                    $"[{serviceName}] Found no previous deployments -> Creating a new deployment into {targetSlot.GetEnumDescription()}."));

                var createParams = new DeploymentCreateParameters
                {
                    Label = serviceName,
                    Name = $"{serviceName}{targetSlot.GetEnumDescription()}",
                    PackageUri = blob.Uri,
                    Configuration = File.ReadAllText(configurationPath),
                    StartDeployment = true
                };

                if (diagnosticsConfigurationPath != null)
                {
                    createParams.ExtensionConfiguration = new ExtensionConfiguration {AllRoles = new[] {new ExtensionConfiguration.Extension {Id = FlexConfiguration.FlexDiagnosticsExtensionId}}};
                }

                await computeClient.Deployments.CreateAsync(serviceName, targetSlot, createParams);
            }
            else
            {
                ThreadAdapter.QueueObject(new BuildEvent(BuildEventType.Information, BuildEventImportance.Medium,
                    $"[{serviceName}] Found a previous deployment -> Updating the current deployment in {targetSlot.GetEnumDescription()}."));

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

            if (VipSwap)
            {
                ThreadAdapter.QueueObject(new BuildEvent(BuildEventType.Information, BuildEventImportance.Medium,
                    $"[{serviceName}] Swapping the deployments."));

                await computeClient.Deployments.SwapAsync(
                    serviceName,
                    new DeploymentSwapParameters
                    {
                        SourceDeployment = $"{serviceName}{targetSlot.GetEnumDescription()}"
                    });

                if (DeleteStaging)
                {
                    ThreadAdapter.QueueObject(new BuildEvent(BuildEventType.Information, BuildEventImportance.Medium,
                        $"[{serviceName}] Deleting the staging deployment after the swap."));

                    await computeClient.Deployments.DeleteBySlotAsync(serviceName, DeploymentSlot.Staging);
                }
            }

            ThreadAdapter.QueueObject(new BuildEvent(BuildEventType.Information, BuildEventImportance.Medium,
                $"[{serviceName}] Deployment complete."));
        }
    }
}
