namespace DevOpsFlex.PowerShell
{
    using System.Management.Automation;
    using Core;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;

    /// <summary>
    /// Copy-AzureTables commandlet implementation.
    /// </summary>
    [Cmdlet(VerbsCommon.Copy, "AzureTables")]
    public class CopyAzureTables : AsyncCmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The Storage Account name that we are reading the tables from.")]
        public string SourceAccountName { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The Storage Account key that we are reading the tables from.")]
        public string SourceAccountKey { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The Storage Account name that we are writing the tables to.")]
        public string TargetAccountName { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The Storage Account key that we are writing the tables to.")]
        public string TargetAccountKey { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "The Regex expression filter to figure out which tables to copy for a partial copy.")]
        public string TableRegexFilter { get; set; }

        /// <summary>
        /// Processes the Copy-AzureTables commandlet synchronously.
        /// </summary>
        protected override void ProcessRecord()
        {
            var sourceAccount = new CloudStorageAccount(new StorageCredentials(SourceAccountName, SourceAccountKey), true);
            var targetAccount = new CloudStorageAccount(new StorageCredentials(TargetAccountName, TargetAccountKey), true);

            ProcessAsyncWork(new[] {new CopyAzureTablesOperation(sourceAccount, targetAccount, TableRegexFilter).Execute()});

            WriteObject("Copy-AzureTables completed!");
            base.ProcessRecord();
        }
    }
}
