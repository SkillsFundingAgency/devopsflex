namespace DevOpsFlex.Azure.Management.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Choosing;
    using Core;
    using Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.Sql;
    using Microsoft.WindowsAzure.Management.Sql.Models;

    /// <summary>
    /// Contains tests that target the <see cref="SqlManagementClientExtensions"/> extension class
    /// for the <see cref="SqlManagementClient"/>.
    /// </summary>
    [TestClass]
    public class SqlManagementClientExtensionsTest
    {

        #region Integration Tests

        /// <summary>
        /// Tests CheckCreateDatabase with the creation of a new database that doesn't exist in the server.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_CheckCreateDatabase_WithNewDatabase()
        {
            const int dbSize = 5;
            const string dbCollation = "SQL_Latin1_General_CP1_CI_AS";
            var dbEdition = SqlAzureEdition.Standard.GetEnumDescription();

            using (var client = ManagementClient.CreateSqlClient())
            {
                var dbName = "fct-" + Guid.NewGuid().ToString().Split('-').Last();

                var chooser = new DefaultAzureSqlServerChooser();

                var server = await chooser.Choose(client, SystemLocation.WestEurope.GetEnumDescription());
                Assert.IsFalse(string.IsNullOrWhiteSpace(server));

                try
                {
                    await client.CheckCreateDatabase(server, dbName, dbEdition, dbCollation, dbSize);

                    var webSite = await client.Databases.GetAsync(server, dbName, new CancellationToken());
                    Assert.IsNotNull(webSite);
                }
                finally
                {
                    client.Databases.Delete(server, dbName);
                }
            }
        }

        /// <summary>
        /// Tests CheckCreateDatabase with the creation of a new database that already exists in the server.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_CheckCreateDatabase_WithExistingDatabase()
        {
            const int dbSize = 5;
            const string dbCollation = "SQL_Latin1_General_CP1_CI_AS";
            var dbEdition = SqlAzureEdition.Standard.GetEnumDescription();

            using (var client = ManagementClient.CreateSqlClient())
            {
                var dbName = "fct-" + Guid.NewGuid().ToString().Split('-').Last();

                var chooser = new DefaultAzureSqlServerChooser();

                var server = await chooser.Choose(client, SystemLocation.WestEurope.GetEnumDescription());
                Assert.IsFalse(string.IsNullOrWhiteSpace(server));

                try
                {
                    await client.Databases.CreateAsync(
                        server,
                        new DatabaseCreateParameters
                        {
                            Name = dbName,
                            Edition = dbEdition,
                            CollationName = dbCollation,
                            MaximumDatabaseSizeInGB = dbSize

                        });

                    await client.CheckCreateDatabase(server, dbName, dbEdition, dbCollation, dbSize);
                }
                finally
                {
                    client.Databases.Delete(server, dbName);
                }
            }
        }

        /// <summary>
        /// Tests CheckCreateFirewallRule with the creation of a new firewall rule that doesn't exist in the server.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_CheckCreateFirewallRule_WithNewFirewallRule()
        {
            const string startIp = "111.111.111.111";
            const string endIp = "111.111.111.111";

            using (var client = ManagementClient.CreateSqlClient())
            {
                var ruleName = "fct-" + Guid.NewGuid().ToString().Split('-').Last();

                var chooser = new DefaultAzureSqlServerChooser();

                var server = await chooser.Choose(client, SystemLocation.WestEurope.GetEnumDescription());
                Assert.IsFalse(string.IsNullOrWhiteSpace(server));

                try
                {
                    await client.CheckCreateFirewallRule(
                        server,
                        new FirewallRuleCreateParameters
                        {
                            Name = ruleName,
                            StartIPAddress = startIp,
                            EndIPAddress = endIp
                        });

                    var rule = await client.FirewallRules.GetAsync(server, ruleName);
                    Assert.IsNotNull(rule);
                }
                finally
                {
                    client.FirewallRules.Delete(server, ruleName);
                }
            }
        }

        /// <summary>
        /// Tests CheckCreateFirewallRule with the creation of a new firewall rule that already exists in the server.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_CheckCreateFirewallRule_WithExistingFirewallRule()
        {
            const string startIp = "111.111.111.111";
            const string endIp = "111.111.111.111";

            using (var client = ManagementClient.CreateSqlClient())
            {
                var ruleName = "fct-" + Guid.NewGuid().ToString().Split('-').Last();

                var chooser = new DefaultAzureSqlServerChooser();

                var server = await chooser.Choose(client, SystemLocation.WestEurope.GetEnumDescription());
                Assert.IsFalse(string.IsNullOrWhiteSpace(server));

                try
                {
                    await client.FirewallRules.CreateAsync(
                        server,
                        new FirewallRuleCreateParameters
                        {
                            Name = ruleName,
                            StartIPAddress = startIp,
                            EndIPAddress = endIp
                        });

                    await client.CheckCreateFirewallRule(
                        server,
                        new FirewallRuleCreateParameters
                        {
                            Name = ruleName,
                            StartIPAddress = startIp,
                            EndIPAddress = endIp
                        });
                }
                finally
                {
                    client.FirewallRules.Delete(server, ruleName);
                }
            }
        }

        #endregion

    }
}
