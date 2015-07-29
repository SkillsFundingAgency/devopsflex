namespace DevOpsFlex.PowerShell
{
    using System.Management.Automation;

    /// <summary>
    /// Set-ReservedIpConfiguration commandlet implementation.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "ReservedIpConfiguration")]
    public class SetReservedIpConfiguration : AsyncCmdlet
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

        [Parameter(
            Mandatory = true,
            HelpMessage = "The local path to where the TFS mapping is.")]
        public string TfsProjectMapPath { get; set; }

        /// <summary>
        /// Processes the Set-ReservedIpConfiguration commandlet synchronously.
        /// </summary>
        protected override void ProcessRecord()
        {
            //#1 Find all the Cloud Servies

            //#2 Find all the Reserved IPs and match them to Cloud Services using the naming conventions

            //#3 Foreach match

                //#A Look at the proper service configuration and check if the Reserved IP assignment is there

                //#B If the 
        }
    }
}
