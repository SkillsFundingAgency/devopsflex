namespace DevOpsFlex.Azure.Management.Choosing
{
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.WindowsAzure.Management.Storage;

    /// <summary>
    /// Provides a default Storage Account chooser object.
    /// </summary>
    public class DefaultStorageAccountChooser : IChooseStorageAccount
    {
        /// <summary>
        /// Finds a valid Storage Account in the subscription to create storage containers on.
        /// </summary>
        /// <param name="client">The <see cref="StorageManagementClient"/> that is performing the operation.</param>
        /// <param name="storageType">The type of storage that the account should have.</param>
        /// <returns>A suitable storage account name if one is found, null otherwise.</returns>
        public async Task<string> Choose(StorageManagementClient client, string storageType)
        {
            return (await client.StorageAccounts.ListAsync())
                .FirstOrDefault(a => a.Name.Contains(FlexDataConfiguration.StoraAccountString) &&
                                     a.Properties.AccountType == storageType)?
                .Name;
        }
    }
}
