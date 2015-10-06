namespace DevOpsFlex.PowerShell
{
    using System.Management.Automation;
    using Core;

    /// <summary>
    /// Copy-AzureTables commandlet implementation.
    /// </summary>
    [Cmdlet(VerbsCommon.Copy, "AzureTables")]
    public class CopyAzureTables : AsyncCmdlet
    {
        /// <summary>
        /// Processes the Copy-AzureTables commandlet synchronously.
        /// </summary>
        protected override void ProcessRecord()
        {
        }
    }
}
