namespace DevOpsFlex.Data
{
    using System;

    public class AzureStorageContainer : DevOpsComponent
    {
        public ContainerExternalAccess ExternalAccess { get; set; }

        public ContainerAcl Acl { get; set; }
    }

    public enum ContainerExternalAccess : short
    {
        Public  = 1,
        Private = 2
    }

    [Flags]
    public enum ContainerAcl : short
    {
        Read    = 1 << 1,
        Write   = 1 << 2,
        Manage  = 1 << 3
    }
}
