namespace DevOpsFlex.CRM.PowerShell
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Management.Automation;
    using System.Net;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Core;
    using Core.Events;
    using Management;
    using Microsoft.Xrm.Sdk.Deployment;
    using Microsoft.Xrm.Sdk.Deployment.Proxy;

    /// <summary>
    /// Represents the PowerShell commandlet implementation for the New-XrmOrganization in the
    /// Ciber toolkit.
    /// </summary>
    [Cmdlet("New", "XrmOrganization")]
    public class NewXrmOrganizationCmdlet : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "The URL location of the CRM deployment service.")]
        public string DeploymentServiceUrl { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The name of the SQL server on which the organization database is installed.")]
        public string SqlServerName { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The URL of the Reporting Services that CRM is connected to.")]
        public string SsrsUrl { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The SQL collation property that the organization will use to sort and compare data characters.")]
        public string SqlCollation { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "Whether information is being collected for the customer experience improvement program.")]
        public bool SqmIsEnabled { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The unique name for the organization.")]
        public string OrganizationUniqueName { get; set; }

        [Parameter(
            Mandatory = true,
            HelpMessage = "The display name, or long name, of the organization database.")]
        public string OrganizationFriendlyName { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "The base currency code for the organization.")]
        public string OrganizationBaseCurrencyCode { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "The base currency name for the organization.")]
        public string OrganizationBaseCurrencyName { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "The number of decimal places that can be used for the base currency.")]
        public int OrganizationBaseCurrencyPrecision { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "The base currency symbol for the organization.")]
        public string OrganizationBaseCurrencySymbol { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "The base language code for the organization.")]
        public int OrganizationBaseLanguageCode { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "The username for the Sql user if integrated security is off.")]
        public string SqlUsername { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "The password for the Sql user if integrated security is off.")]
        public string SqlPassword { get; set; }

        [Parameter(
            Mandatory = false,
            HelpMessage = "The PSCredential identity account to relay to the CRM Deployment service.")]
        public PSCredential Credential { get; set; }

        /// <summary>
        /// Processes the New-XrmOrganisation commandlet synchronously.
        /// </summary>
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        protected override void ProcessRecord()
        {
            using (var eventStream = new Subject<BuildEvent>())
            {
                var credential = Credential != null ?
                    Credential.GetNetworkCredential() :
                    CredentialCache.DefaultNetworkCredentials;

                var crmDb = SqlUsername != null && SqlPassword != null ?
                    new CrmDatabase(SqlServerName, SqlUsername, SqlPassword) :
                    new CrmDatabase(SqlServerName);

                eventStream.Where(e => e.Type == BuildEventType.Information)
                           .Subscribe(e => WriteVerbose($"[{DateTime.Now:HH:mm:ss.FFFF}] {e.Message}"));

                eventStream.Where(e => e.Type == BuildEventType.Warning)
                           .Subscribe(e => WriteWarning($"[{DateTime.Now:HH:mm:ss.FFFF}] {e.Message}"));

                eventStream.Where(e => e.Type == BuildEventType.Error)
                           .Subscribe(e => WriteError(new ErrorRecord(new Exception(e.Message), e.Message, ErrorCategory.InvalidOperation, this)));

                try
                {
                    AsyncPump.Run(async delegate { await Reprovision(eventStream, crmDb, credential); });
                }
                finally
                {
                    crmDb.Dispose();
                }
            }

            base.ProcessRecord();
        }

        /// <summary>
        /// Reprovisions the Xrm Organisation by dropping the database and re-creating the organisation
        /// </summary>
        /// <param name="eventStream">The Build Event stream</param>
        /// <param name="crmDb">An object representing the CRM Database</param>
        /// <param name="credential">The Network Credential to use on the CRM deployment client.</param>
        /// <returns>The async <see cref="Task"/> unit of work.</returns>
        private async Task Reprovision(IObserver<BuildEvent> eventStream, CrmDatabase crmDb, NetworkCredential credential)
        {
            using (var client = ProxyClientHelper.CreateClient(new Uri(DeploymentServiceUrl)))
            {
                if (client.ClientCredentials != null)
                {
                    client.ClientCredentials.Windows.ClientCredential = credential;
                }

                var crmDatabaseName = OrganizationUniqueName + "_MSCRM";

                try
                {
                    if (await client.OrganizationExists(OrganizationUniqueName))
                    {
                        await client.DisableOrganizationAsync(OrganizationUniqueName, eventStream);
                        await client.DeleteOrganizationAsync(OrganizationUniqueName, eventStream);
                    }
                    else
                    {
                        eventStream.OnNext(new BuildEvent(BuildEventType.Warning, BuildEventImportance.Medium, $"The Organization {OrganizationUniqueName} doesn't exist in the CRM server. Is this intentional?"));
                    }

                    if (await crmDb.DatabaseExists(crmDatabaseName))
                    {
                        await crmDb.DropDatabaseAsync(crmDatabaseName, eventStream);
                    }
                    else
                    {
                        eventStream.OnNext(new BuildEvent(BuildEventType.Warning, BuildEventImportance.Medium, $"The Organization Database {crmDatabaseName} doesn't exist in the SQL Server. Is this intentional?"));
                    }

                    await client.CreateOrganizationAsync(
                        new Organization
                        {
                            BaseCurrencyCode = OrganizationBaseCurrencyCode,
                            BaseCurrencyName = OrganizationBaseCurrencyName,
                            BaseCurrencyPrecision = OrganizationBaseCurrencyPrecision,
                            BaseCurrencySymbol = OrganizationBaseCurrencySymbol,
                            BaseLanguageCode = OrganizationBaseLanguageCode,
                            UniqueName = OrganizationUniqueName,
                            FriendlyName = OrganizationFriendlyName,
                            SqlCollation = SqlCollation,
                            SqlServerName = SqlServerName.Replace(".cloudapp.net", ""),
                            SrsUrl = SsrsUrl,
                            SqmIsEnabled = SqmIsEnabled
                        },
                        eventStream);
                }
                catch (AggregateException ex)
                {
                    var fault = ex.InnerExceptions.First() as FaultException<DeploymentServiceFault>;
                    if (fault == null) throw ex.InnerExceptions.First();

                    dynamic clunkyCast = fault.Detail.ErrorDetails;
                    foreach (KeyValuePair<string, object> p in clunkyCast)
                    {
                        eventStream.OnNext(new BuildEvent(BuildEventType.Error, BuildEventImportance.High, $"{p.Key} : {p.Value}"));
                    }
                }
            }
        }
    }
}
