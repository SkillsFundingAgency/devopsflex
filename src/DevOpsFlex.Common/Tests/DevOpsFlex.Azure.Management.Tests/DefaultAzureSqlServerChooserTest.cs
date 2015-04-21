namespace DevOpsFlex.Azure.Management.Tests
{
    using System.Threading.Tasks;
    using Choosing;
    using Core;
    using Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Contains tests that target the <see cref="DefaultAzureSqlServerChooser"/> class in the choosing pipeline.
    /// </summary>
    [TestClass]
    public class DefaultAzureSqlServerChooserTest
    {

        #region Integration Tests

        /// <summary>
        /// Containst the string name of an Azure region where the subscription in use doesn't have SQL Servers.
        /// </summary>
        private readonly string _regionWithoutSqlServers = SystemLocation.EastAsia.GetEnumDescription();

        /// <summary>
        /// Containst the string name of an Azure region where the subscription in use has at least one SQL Server.
        /// </summary>
        private readonly string _regionWithSqlServers = SystemLocation.WestEurope.GetEnumDescription();

        /// <summary>
        /// Tests Choose against a region without SQL Servers and expects a null return.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_Choose_WithNoValidServer()
        {
            using (var client = ManagementClient.CreateSqlClient())
            {
                var chooser = new DefaultAzureSqlServerChooser();

                var server = await chooser.Choose(client, _regionWithoutSqlServers);

                Assert.IsNull(server);
            }
        }

        /// <summary>
        /// Tests Choose against a region with SQL Servers and expects a non-null return.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_Choose_WithValidServer()
        {
            using (var client = ManagementClient.CreateSqlClient())
            {
                var chooser = new DefaultAzureSqlServerChooser();

                var server = await chooser.Choose(client, _regionWithSqlServers);

                Assert.IsFalse(string.IsNullOrWhiteSpace(server));
            }
        }

        #endregion

    }
}
