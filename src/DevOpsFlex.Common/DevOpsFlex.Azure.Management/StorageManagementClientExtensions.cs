namespace DevOpsFlex.Azure.Management
{
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using Core;
    using Data;
    using Data.Events;
    using Microsoft.WindowsAzure.Management.Storage;
    using Microsoft.WindowsAzure.Management.Storage.Models;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Extends the <see cref="StorageManagementClient"/> with usefull extensions that the devopsflex
    /// activities and commandlets need in order to achieve their execution.
    /// </summary>
    public static class StorageManagementClientExtensions
    {
        /// <summary>
        /// Gate object for the storage account choosing process.
        /// </summary>
        private static readonly object StorageAccountGate = new object();

        /// <summary>
        /// Checks for the existence of a specific storage container, if it doesn't exist it will create it.
        /// </summary>
        /// <param name="client">The <see cref="StorageManagementClient"/> that is performing the operation.</param>
        /// <param name="accountName">The name of the storage account that we want to create the container in.</param>
        /// <param name="containerName">The name of the container that we are about to create.</param>
        /// <param name="publicAccess">The public access level for the container.</param>
        /// <param name="permissions">The set of permissions that the ACL for this container must have.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task<CloudStorageAccount> CreateContainerIfNotExistsAsync(
            this StorageManagementClient client,
            string accountName,
            string containerName,
            BlobContainerPublicAccessType publicAccess,
            SharedAccessBlobPermissions permissions)
        {
            Contract.Requires(client != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(accountName));
            Contract.Requires(!string.IsNullOrWhiteSpace(containerName));
            Contract.Requires(containerName.Length >= 3);
            Contract.Requires(containerName.Length <= 63);

            var key = (await client.StorageAccounts.GetKeysAsync(accountName)).PrimaryKey;

            var storageAccount = CloudStorageAccount.Parse($"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={key}");
            var container = storageAccount.CreateCloudBlobClient().GetContainerReference(containerName);

            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = publicAccess });
            FlexStreams.Publish(new ProvisionEvent(AzureResource.StorageContainer, containerName));

            var acl = container.GetSharedAccessSignature(new SharedAccessBlobPolicy { Permissions = permissions });
            FlexStreams.Publish(new StorageKeyEvent(accountName, containerName, acl));

            return storageAccount;
        }

        /// <summary>
        /// Checks for the existence of a specific storage container, if it doesn't exist it will create it.
        /// It also checks for a specific storage account that suits the system, if it doesn't exist in the subscription
        /// it will create it before attempting to create the container.
        /// </summary>
        /// <param name="client">The <see cref="StorageManagementClient"/> that is performing the operation.</param>
        /// <param name="model">The DevOpsFlex rich model object that contains everything there is to know about this database spec.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CreateContainerIfNotExistsAsync(this StorageManagementClient client, AzureStorageContainer model)
        {
            Contract.Requires(client != null);
            Contract.Requires(model != null);
            Contract.Requires(model.System != null);

            string accountName;

            lock (StorageAccountGate)
            {
                FlexStreams.Publish(new CheckForParentResourceEvent(AzureResource.StorageAccount, AzureResource.StorageContainer, model.Name));
                accountName = FlexConfiguration.StorageAccountChooser.Choose(client, model.System.StorageType.GetEnumDescription()).Result;
                StorageAccountGetResponse account = null;

                try
                {
                    account = client.StorageAccounts.Get(accountName);
                }
                catch
                {
                    // ignored
                }

                if (account == null)
                {
                    accountName = (model.System.LogicalName + FlexDataConfiguration.StorageAccountString + FlexDataConfiguration.Branch).ToLower();

                    client.StorageAccounts.Create(
                        new StorageAccountCreateParameters
                        {
                            Name = accountName,
                            Location = model.System.Location.GetEnumDescription(),
                            AccountType = model.System.StorageType.GetEnumDescription()
                        });

                    FlexStreams.Publish(new ProvisionEvent(AzureResource.StorageAccount, accountName));
                }
                else
                {
                    accountName = account.StorageAccount.Name;
                    FlexStreams.Publish(new FoundExistingEvent(AzureResource.StorageAccount, accountName));
                }
            }

            await client.CreateContainerIfNotExistsAsync(
                accountName,
                FlexConfiguration.GetNaming<AzureStorageContainer>()
                                 .GetSlotName(model, FlexDataConfiguration.Branch, FlexDataConfiguration.Configuration),
                model.PublicAccess,
                model.Acl);
        }
    }
}
