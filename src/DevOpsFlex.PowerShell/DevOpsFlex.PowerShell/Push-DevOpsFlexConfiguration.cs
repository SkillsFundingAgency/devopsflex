namespace DevOpsFlex.PowerShell
{
    using System;
    using System.Management.Automation;
    using Azure.Management;
    using Data;

    /// <summary>
    /// Push-DevOpsFlexConfiguration commandlet implementation.
    /// </summary>
    [Cmdlet(VerbsCommon.Push, "DevOpsFlexConfiguration")]
    public class PushDevOpsFlexConfiguration : AsyncCmdlet
    {
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
            HelpMessage = "The SqlConnectionString to the DevOpsFlex database.")]
        public string SqlConnectionString { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the branch we want to target.")]
        public string Branch { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The .Net project configuration we want to target.")]
        public string Configuration { get; set; }

        /// <summary>
        /// Processes the Push-DevOpsFlexConfiguration commandlet synchronously.
        /// </summary>
        protected override void ProcessRecord()
        {
            FlexDataConfiguration.Branch = Branch;
            FlexDataConfiguration.Configuration = Configuration;
            FlexStreams.UseThreadQueue(ThreadAdapter);

            using (EventStream.Subscribe(e => WriteObject(e.Message)))
            using (var context = new DevOpsFlexDbContext(SqlConnectionString))
            {
                ProcessAsyncWork(context.ProvisionAll(SubscriptionId, SettingsPath));
            }
        }
    }
}
