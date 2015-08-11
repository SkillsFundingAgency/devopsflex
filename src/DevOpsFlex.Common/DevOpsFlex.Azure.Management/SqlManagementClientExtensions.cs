namespace DevOpsFlex.Azure.Management
{
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Core;
    using Core.Events;
    using Data;
    using Hyak.Common;
    using Microsoft.WindowsAzure.Management.Sql;
    using Microsoft.WindowsAzure.Management.Sql.Models;

    /// <summary>
    /// Extends the <see cref="SqlManagementClient"/> with usefull extensions that the devopsflex
    /// activities and commandlets need in order to achieve their execution.
    /// </summary>
    public static class SqlManagementClientExtensions
    {
        /// <summary>
        /// Gate object for the sql server choosing process.
        /// </summary>
        private static readonly object SqlServerGate = new object();

        /// <summary>
        /// Checks for the existence of a specific Azure Sql Database, if it doesn't exist it will create it.
        /// </summary>
        /// <param name="client">The <see cref="SqlManagementClient"/> that is performing the operation.</param>
        /// <param name="serverName">The name of the server that we want to use to create the database.</param>
        /// <param name="databaseName">The name of the database we are creating.</param>
        /// <param name="databaseEdition">The edition of the database we are creating.</param>
        /// <param name="collationName">The database collation name.</param>
        /// <param name="sizeInGb">The maximum database size in GB.</param>
        /// <param name="createAppUser"></param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CreateDatabaseIfNotExistsAsync(
            this SqlManagementClient client,
            string serverName,
            string databaseName,
            string databaseEdition,
            string collationName,
            int sizeInGb,
            bool createAppUser)
        {
            Contract.Requires(client != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(serverName));
            Contract.Requires(!string.IsNullOrWhiteSpace(databaseName));
            Contract.Requires(!string.IsNullOrWhiteSpace(databaseEdition));
            Contract.Requires(!string.IsNullOrWhiteSpace(collationName));
            Contract.Requires(sizeInGb > 0 && sizeInGb <= 250);

            DatabaseGetResponse ns = null;
            FlexStreams.Publish(new CheckIfExistsEvent(AzureResource.SqlDatabase, databaseName));

            try
            {
                ns = await client.Databases.GetAsync(serverName, databaseName);
            }
            catch (CloudException cex)
            {
                if (!cex.Error.Message.Contains($"Resource with the name '{databaseName}' does not exist")) throw;
            }

            if (ns != null)
            {
                FlexStreams.Publish(new FoundExistingEvent(AzureResource.SqlDatabase, databaseName));
                return;
            }

            await client.Databases.CreateAsync(
                serverName,
                new DatabaseCreateParameters
                {
                    Name = databaseName,
                    Edition = databaseEdition,
                    CollationName = collationName,
                    MaximumDatabaseSizeInGB = sizeInGb,
                });

            FlexStreams.Publish(new ProvisionEvent(AzureResource.SqlDatabase, databaseName));

            if (!createAppUser) return;

            using (var adb = new DevOpsAzureDatabase(serverName, databaseName, FlexConfiguration.FlexSaUser, FlexConfiguration.FlexSaPwd))
            {
                await adb.CreateDatabaseUserAsync(FlexConfiguration.FlexAppUser, FlexConfiguration.FlexAppUser, "dbo");
            }
        }

        /// <summary>
        /// Checks for the existence of a specific Azure Sql Database, if it doesn't exist it will create it.
        /// </summary>
        /// <param name="client">The <see cref="SqlManagementClient"/> that is performing the operation.</param>
        /// <param name="model">The DevOpsFlex rich model object that contains everything there is to know about this database spec.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CreateDatabaseIfNotExistsAsync(this SqlManagementClient client, SqlAzureDb model)
        {
            Contract.Requires(client != null);
            Contract.Requires(model != null);

            string serverName;

            lock (SqlServerGate)
            {
                FlexStreams.Publish(new CheckForParentResourceEvent(AzureResource.SqlServer, AzureResource.SqlDatabase, model.Name));
                serverName = FlexConfiguration.SqlServerChooser.Choose(client, model.System.Location.GetEnumDescription()).Result;

                if (serverName == null)
                {
                    string serverMaxVersion = null;

                    try
                    {
                        client.Servers.Create(
                            new ServerCreateParameters
                            {
                                AdministratorUserName = FlexConfiguration.FlexSaUser,
                                AdministratorPassword = FlexConfiguration.FlexSaPwd,
                                Location = model.System.Location.GetEnumDescription(),
                                Version = "100.0" // This needs to be an invalid version number
                            });
                    }
                    catch (CloudException ex)
                    {
                        if (ex.Error.Code == "40856") // SQL Version doesn't exist in the target location
                        {
                            var serverVersions = Regex.Match(ex.Error.Message, "server versions: '([^']*)'.").Groups[1].Value.Split(',');
                            var maxVersion = serverVersions.Max((s => decimal.Parse(s)));
                            serverMaxVersion = serverVersions.First(s => s.Contains(maxVersion.ToString("F1", CultureInfo.InvariantCulture)));
                        }
                        else
                        {
                            throw;
                        }
                    }

                    serverName = client.Servers.Create(
                        new ServerCreateParameters
                        {
                            AdministratorUserName = FlexConfiguration.FlexSaUser,
                            AdministratorPassword = FlexConfiguration.FlexSaPwd,
                            Location = model.System.Location.GetEnumDescription(),
                            Version = serverMaxVersion
                        }).ServerName;

                    FlexStreams.Publish(new ProvisionEvent(AzureResource.SqlServer, serverName));
                }
                else
                {
                    FlexStreams.Publish(new FoundExistingEvent(AzureResource.SqlServer, serverName));
                }
            }

            var dbName = FlexConfiguration.GetNaming<SqlAzureDb>()
                                          .GetSlotName(
                                              model,
                                              FlexDataConfiguration.Branch,
                                              FlexDataConfiguration.Configuration);

            await client.CreateDatabaseIfNotExistsAsync(serverName, dbName, model.Edition.GetEnumDescription(), model.CollationName, model.MaximumDatabaseSizeInGB, model.CreateAppUser);
        }

        /// <summary>
        /// Checks for the existence of a specific Azure Sql Firewall rule, if it doesn't exist it will create it.
        /// </summary>
        /// <param name="client">The <see cref="SqlManagementClient"/> that is performing the operation.</param>
        /// <param name="serverName">The name of the server that we want to use to create the database.</param>
        /// <param name="parameters">The <see cref="FirewallRuleCreateParameters"/> set of parameters for the rule.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CreateFirewallRuleIfNotExistsAsync(this SqlManagementClient client, string serverName, FirewallRuleCreateParameters parameters)
        {
            Contract.Requires(client != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(serverName));
            Contract.Requires(parameters != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(parameters.Name));
            Contract.Requires(!string.IsNullOrWhiteSpace(parameters.StartIPAddress));
            Contract.Requires(!string.IsNullOrWhiteSpace(parameters.EndIPAddress));

            FirewallRuleGetResponse rule = null;
            FlexStreams.Publish(new CheckIfExistsEvent(AzureResource.FirewallRule, parameters.Name));

            try
            {
                rule = await client.FirewallRules.GetAsync(serverName, parameters.Name);
            }
            catch (CloudException cex)
            {
                if (!cex.Error.Message.Contains($"Resource with the name '{parameters.Name}' does not exist")) throw;
            }

            if (rule != null)
            {
                FlexStreams.Publish(new FoundExistingEvent(AzureResource.FirewallRule, parameters.Name));
                return;
            }

            await client.FirewallRules.CreateAsync(serverName, parameters);
            FlexStreams.Publish(new ProvisionEvent(AzureResource.FirewallRule, parameters.Name));
        }

        /// <summary>
        /// Checks for the existence of a specific Azure Sql Firewall rule, if it doesn't exist it will create it.
        /// </summary>
        /// <param name="client">The <see cref="SqlManagementClient"/> that is performing the operation.</param>
        /// <param name="model">The DevOpsFlex rich model object that contains everything there is to know about this database spec.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CreateFirewallRuleIfNotExistsAsync(this SqlManagementClient client, SqlFirewallRule model)
        {
            Contract.Requires(client != null);
            Contract.Requires(model != null);

            await client.CreateFirewallRuleIfNotExistsAsync(
                await FlexConfiguration.SqlServerChooser.Choose(client, model.System.Location.GetEnumDescription()),
                model.AzureParameters);
        }
    }
}
