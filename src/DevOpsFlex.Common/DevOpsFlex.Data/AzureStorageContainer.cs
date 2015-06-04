namespace DevOpsFlex.Data
{
    using Microsoft.WindowsAzure.Storage.Blob;

    public class AzureStorageContainer : DevOpsComponent
    {
        public BlobContainerPublicAccessType PublicAccess { get; set; }

        public SharedAccessBlobPermissions Acl { get; set; }
    }
}
