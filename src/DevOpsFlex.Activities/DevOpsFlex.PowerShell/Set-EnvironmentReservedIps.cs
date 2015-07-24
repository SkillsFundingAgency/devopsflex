namespace DevOpsFlex.PowerShell
{
    using System.Management.Automation;

    /// <summary>
    /// Set-EnvironmentReservedIps commandlet implementation.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "EnvironmentReservedIps")]
    public class SetEnvironmentReservedIps : AsyncCmdlet
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
            HelpMessage = "The name of the branch we want to target.")]
        public string Branch { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The .Net project configuration we want to target.")]
        public string Configuration { get; set; }

        /// <summary>
        /// Processes the Set-EnvironmentReservedIps commandlet synchronously.
        /// </summary>
        protected override void ProcessRecord()
        {
        }
    }
}
