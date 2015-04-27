namespace DevOpsFlex.Azure.Management
{
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using Core;
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
        /// Checks for the existence of a specific Azure Sql Database, if it doesn't exist it will create it.
        /// </summary>
        /// <param name="client">The <see cref="SqlManagementClient"/> that is performing the operation.</param>
        /// <param name="serverName">The name of the server that we want to use to create the database.</param>
        /// <param name="databaseName">The name of the database we are creating.</param>
        /// <param name="databaseEdition">The edition of the database we are creating.</param>
        /// <param name="collationName">The database collation name.</param>
        /// <param name="sizeInGb">The maximum database size in GB.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CheckCreateDatabase(
            this SqlManagementClient client,
            string serverName,
            string databaseName,
            string databaseEdition,
            string collationName,
            int sizeInGb)
        {
            Contract.Requires(client != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(serverName));
            Contract.Requires(!string.IsNullOrWhiteSpace(databaseName));
            Contract.Requires(!string.IsNullOrWhiteSpace(databaseEdition));
            Contract.Requires(!string.IsNullOrWhiteSpace(collationName));
            Contract.Requires(sizeInGb > 0 && sizeInGb <= 250);

            DatabaseGetResponse ns = null;

            try
            {
                ns = await client.Databases.GetAsync(serverName, databaseName);
            }
            catch (CloudException cex)
            {
                if (!cex.Error.Message.Contains(string.Format("Resource with the name '{0}' does not exist", databaseName))) throw;
            }

            if (ns != null) return;

            await client.Databases.CreateAsync(
                serverName,
                new DatabaseCreateParameters
                {
                    Name = databaseName,
                    Edition = databaseEdition,
                    CollationName = collationName,
                    MaximumDatabaseSizeInGB = sizeInGb
                });
        }

        /// <summary>
        /// Checks for the existence of a specific Azure Sql Database, if it doesn't exist it will create it.
        /// </summary>
        /// <param name="client">The <see cref="SqlManagementClient"/> that is performing the operation.</param>
        /// <param name="model">The DevOpsFlex rich model object that contains everything there is to know about this database spec.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CheckCreateDatabase(this SqlManagementClient client, SqlAzureDb model)
        {
            Contract.Requires(client != null);
            Contract.Requires(model != null);

            await client.CheckCreateDatabase(
                await FlexConfiguration.SqlServerChooser.Choose(client, model.System.Location.GetEnumDescription()),
                FlexConfiguration.GetNaming<SqlAzureDb>().GetSlotName(model),
                model.Edition.GetEnumDescription(),
                model.CollationName,
                model.MaximumDatabaseSizeInGB);
        }

        /// <summary>
        /// Checks for the existence of a specific Azure Sql Firewall rule, if it doesn't exist it will create it.
        /// </summary>
        /// <param name="client">The <see cref="SqlManagementClient"/> that is performing the operation.</param>
        /// <param name="serverName">The name of the server that we want to use to create the database.</param>
        /// <param name="parameters">The <see cref="FirewallRuleCreateParameters"/> set of parameters for the rule.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CheckCreateFirewallRule(this SqlManagementClient client, string serverName, FirewallRuleCreateParameters parameters)
        {
            Contract.Requires(client != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(serverName));
            Contract.Requires(parameters != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(parameters.Name));
            Contract.Requires(!string.IsNullOrWhiteSpace(parameters.StartIPAddress));
            Contract.Requires(!string.IsNullOrWhiteSpace(parameters.EndIPAddress));

            FirewallRuleGetResponse rule = null;

            try
            {
                rule = await client.FirewallRules.GetAsync(serverName, parameters.Name);
            }
            catch (CloudException cex)
            {
                if (!cex.Error.Message.Contains(string.Format("Resource with the name '{0}' does not exist", parameters.Name))) throw;
            }

            if (rule != null) return;

            await client.FirewallRules.CreateAsync(serverName, parameters);
        }

        /// <summary>
        /// Checks for the existence of a specific Azure Sql Firewall rule, if it doesn't exist it will create it.
        /// </summary>
        /// <param name="client">The <see cref="SqlManagementClient"/> that is performing the operation.</param>
        /// <param name="model">The DevOpsFlex rich model object that contains everything there is to know about this database spec.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CheckCreateFirewallRule(this SqlManagementClient client, SqlFirewallRule model)
        {
            Contract.Requires(client != null);
            Contract.Requires(model != null);

            await client.CheckCreateFirewallRule(
                await FlexConfiguration.SqlServerChooser.Choose(client, model.System.Location.GetEnumDescription()),
                model.AzureParameters);
        }
    }
}
