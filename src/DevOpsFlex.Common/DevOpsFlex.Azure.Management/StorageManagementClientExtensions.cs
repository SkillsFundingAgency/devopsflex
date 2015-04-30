﻿namespace DevOpsFlex.Azure.Management
{
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Data;
    using Microsoft.WindowsAzure.Management.Storage;
    using Microsoft.WindowsAzure.Management.Storage.Models;
    using Microsoft.WindowsAzure.Storage;

    /// <summary>
    /// Extends the <see cref="StorageManagementClient"/> with usefull extensions that the devopsflex
    /// activities and commandlets need in order to achieve their execution.
    /// </summary>
    public static class StorageManagementClientExtensions
    {
        /// <summary>
        /// Checks for the existence of a specific storage container, if it doesn't exist it will create it.
        /// </summary>
        /// <param name="client">The <see cref="StorageManagementClient"/> that is performing the operation.</param>
        /// <param name="accountName">The name of the storage account that we want to create the container in.</param>
        /// <param name="containerName">The name of the container that we are about to create.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CreateContainerIfNotExistsAsync(this StorageManagementClient client, string accountName, string containerName)
        {
            Contract.Requires(client != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(accountName));
            Contract.Requires(!string.IsNullOrWhiteSpace(containerName));
            Contract.Requires(containerName.Length >= 3);
            Contract.Requires(containerName.Length <= 63);

            var key = (await client.StorageAccounts.GetKeysAsync(accountName)).PrimaryKey;

            var storageAccount = CloudStorageAccount.Parse(string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", accountName, key));
            await storageAccount.CreateCloudBlobClient().GetContainerReference(containerName).CreateIfNotExistsAsync();

            // TODO: setup container access level during creation.
            // TODO: create proper ACLs on the container based on the system spec.
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

            var account = (await client.StorageAccounts.ListAsync())
                .FirstOrDefault(a => a.Name.Contains(FlexDataConfiguration.StoraAccountString) &&
                                     a.Properties.AccountType == model.System.StorageType.GetEnumDescription());

            if (account == null)
            {
                accountName = model.System.LogicalName + "-" + FlexDataConfiguration.StoraAccountString;

                await client.StorageAccounts.CreateAsync(
                    new StorageAccountCreateParameters
                    {
                        Name = accountName,
                        Location = model.System.Location.GetEnumDescription(),
                        AccountType = model.System.StorageType.GetEnumDescription()
                    });
            }
            else
            {
                accountName = account.Name;
            }

            await client.CreateContainerIfNotExistsAsync(
                accountName,
                FlexConfiguration.GetNaming<AzureStorageContainer>()
                                 .GetSlotName(model, FlexDataConfiguration.Branch, FlexDataConfiguration.Configuration));
        }
    }
}
