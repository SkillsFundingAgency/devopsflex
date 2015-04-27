﻿namespace DevOpsFlex.Azure.Management.Tests
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
            using (var client = ManagementClient.CreateSqlClient())
            {
                var dbName = "fct-" + Guid.NewGuid().ToString().Split('-').Last();

                var chooser = new DefaultAzureSqlServerChooser();

                var server = await chooser.Choose(client, SystemLocation.WestEurope.GetEnumDescription());
                Assert.IsFalse(string.IsNullOrWhiteSpace(server));

                try
                {
                    await client.CheckCreateDatabase(server, dbName, SqlAzureEdition.Standard.GetEnumDescription(), "SQL_Latin1_General_CP1_CI_AS", 5);

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
        /// Tests CheckCreateFirewallRule with the creation of a new firewall rule that doesn't exist in the server.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_CheckCreateFirewallRule_WithNewFirewallRule()
        {
            using (var client = ManagementClient.CreateSqlClient())
            {
                var ruleName = "fct-" + Guid.NewGuid().ToString().Split('-').Last();

                var chooser = new DefaultAzureSqlServerChooser();

                var server = await chooser.Choose(client, SystemLocation.WestEurope.GetEnumDescription());
                Assert.IsFalse(string.IsNullOrWhiteSpace(server));

                try
                {
                    await client.CheckCreateFirewallRule(server, new FirewallRuleCreateParameters
                    {
                        Name = ruleName,
                        StartIPAddress = "111.111.111.111",
                        EndIPAddress = "111.111.111.111"
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

        #endregion

    }
}
