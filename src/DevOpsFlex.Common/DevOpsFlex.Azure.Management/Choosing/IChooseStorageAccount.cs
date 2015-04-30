namespace DevOpsFlex.Azure.Management.Choosing
{
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.Storage;

    /// <summary>
    /// Provides a contract to override the Storage Account choosing pipeline.
    /// </summary>
    public interface IChooseStorageAccount
    {
        /// <summary>
        /// Finds a valid storage account in the subscription to create containers on.
        /// </summary>
        /// <param name="client">The <see cref="StorageManagementClient"/> that is performing the operation.</param>
        /// <returns>A suitable storage account name if one is found, null otherwise.</returns>
        Task<string> Choose(StorageManagementClient client);
    }
}
