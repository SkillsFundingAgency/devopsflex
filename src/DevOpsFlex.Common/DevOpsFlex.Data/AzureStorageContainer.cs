namespace DevOpsFlex.Data
{
    using System;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class AzureStorageContainer : DevOpsComponent
    {
        public BlobContainerPublicAccessType PublicAccess { get; set; }

        public SharedAccessBlobPermissions Acl { get; set; }
    }
}
