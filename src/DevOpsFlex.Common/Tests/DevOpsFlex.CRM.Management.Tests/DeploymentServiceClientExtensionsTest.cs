namespace DevOpsFlex.CRM.Management.Tests
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using Core.Events;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Xrm.Sdk.Deployment;
    using Microsoft.Xrm.Sdk.Deployment.Proxy;

    /// <summary>
    /// Contains deployment extension methods for the <see cref="DeploymentServiceClient"/> in the CRM SDK.
    /// </summary>
    [TestClass]
    public class DeploymentServiceClientExtensionsTest
    {
        private readonly NetworkCredential _credential = new NetworkCredential("mega", "mega", "dev");

        public TestContext TestContext { get; set; }

        #region Integration

        /// <summary>
        /// The URL constant for the deployment service that we want to target in the Integration Tests.
        /// </summary>
        private const string DeploymentServiceUrl = "http://fct-ci-crm-01.cloudapp.net/xrmdeployment/2011/deployment.svc";

        /// <summary>
        /// Tests the creation of an Organization in CRM and asserts that it was created.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_CreateOrganization()
        {
            const string organizationName = "TestIntegration";

            using (var eventStream = new Subject<BuildEvent>())
            using (var client = ProxyClientHelper.CreateClient(new Uri(DeploymentServiceUrl)))
            {
                eventStream.Subscribe(e => TestContext.WriteLine($"[{e.Type.ToString()}] [{e.Importance.ToString()}] {e.Message}"));

                if (client.ClientCredentials != null)
                {
                    client.ClientCredentials.Windows.ClientCredential = _credential;
                }

                client.CreateOrganizationAsync(
                    new Organization
                    {
                        BaseCurrencyCode = "GBP",
                        BaseCurrencyName = "Pound Sterling",
                        BaseCurrencyPrecision = 2,
                        BaseCurrencySymbol = "£",
                        BaseLanguageCode = 1033,
                        FriendlyName = organizationName,
                        UniqueName = organizationName,
                        SqlCollation = "Latin1_General_CI_AI",
                        SqlServerName = "fct-ci-crm-01",
                        SrsUrl = "http://fct-ci-crm-01/reportserver",
                        SqmIsEnabled = false
                    },
                    eventStream).Wait();

                var exists = client.OrganizationExists(organizationName).Result;
                Assert.IsTrue(exists);
            }
        }

        /// <summary>
        /// Tests the disable and delete of an Organization in CRM and asserts that it no longer exists in CRM.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_DisableOrganization_And_DeleteOrganization()
        {
            const string organizationName = "TestIntegration";

            using (var eventStream = new Subject<BuildEvent>())
            using (var client = ProxyClientHelper.CreateClient(new Uri(DeploymentServiceUrl)))
            {
                eventStream.Subscribe(e => TestContext.WriteLine($"[{e.Type.ToString()}] [{e.Importance.ToString()}] {e.Message}"));

                if (client.ClientCredentials != null)
                {
                    client.ClientCredentials.Windows.ClientCredential = _credential;
                }

                client.DisableOrganizationAsync(organizationName, eventStream).Wait();
                client.DeleteOrganizationAsync(organizationName, eventStream).Wait();

                var exists = client.OrganizationExists(organizationName).Result;
                Assert.IsFalse(exists);
            }
        }

        /// <summary>
        /// Tests the creation of an Organization in CRM and asserts that it was created.
        /// Then it tests the disable and delete of an Organization in CRM and asserts that
        /// it no longer exists in CRM.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_CreateOrganization_DisableOrganization_And_DeleteOrganization()
        {
            const string organizationName = "TestIntegration";

            using (var eventStream = new Subject<BuildEvent>())
            using (var client = ProxyClientHelper.CreateClient(new Uri(DeploymentServiceUrl)))
            {
                eventStream.Subscribe(e => TestContext.WriteLine($"[{e.Type.ToString()}] [{e.Importance.ToString()}] {e.Message}"));

                if (client.ClientCredentials != null)
                {
                    client.ClientCredentials.Windows.ClientCredential = _credential;
                }

                client.CreateOrganizationAsync(
                    new Organization
                    {
                        BaseCurrencyCode = "GBP",
                        BaseCurrencyName = "Pound Sterling",
                        BaseCurrencyPrecision = 2,
                        BaseCurrencySymbol = "£",
                        BaseLanguageCode = 1033,
                        FriendlyName = organizationName,
                        UniqueName = organizationName,
                        SqlCollation = "Latin1_General_CI_AI",
                        SqlServerName = "fct-ci-crm-01",
                        SrsUrl = "http://fct-ci-crm-01/reportserver",
                        SqmIsEnabled = false
                    },
                    eventStream).Wait();

                var exists = client.OrganizationExists(organizationName).Result;
                Assert.IsTrue(exists);

                client.DisableOrganizationAsync(organizationName, eventStream).Wait();
                client.DeleteOrganizationAsync(organizationName, eventStream).Wait();

                exists = client.OrganizationExists(organizationName).Result;
                Assert.IsFalse(exists);
            }
        }

        #endregion

    }

    /// <summary>
    /// Contains extension methods to support integration tests to the <see cref="DeploymentServiceClient"/> object
    /// in the CRM SDK Deployment library.
    /// </summary>
    public static class DeploymentServiceClientTestExtensions
    {
        /// <summary>
        /// Checks is the requested organization exists in the target CRM deployment.
        /// </summary>
        /// <param name="client">The <see cref="DeploymentServiceClient"/> that we are using to call CRM.</param>
        /// <param name="organization">The name of the Organization we want to check for.</param>
        /// <returns>True if the organization exists, false otherwise.</returns>
        public static async Task<bool> OrganizationExists(this DeploymentServiceClient client, string organization)
        {
            EntityInstanceId org = null;
            await Task.Factory.StartNew(() => org = client.RetrieveAll(DeploymentEntityType.Organization)
                                                          .FirstOrDefault(item => item.Name == organization));

            return org != null;
        }
    }
}
