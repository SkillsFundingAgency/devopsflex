namespace DevOpsFlex.Azure.Management.Tests
{
    using System;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Data.PublishSettings;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Management.WebSites;
    using Microsoft.WindowsAzure.Management.WebSites.Models;

    [TestClass]
    public class WebSiteManagementClientExtensionsTest
    {
        /// <summary>
        /// Specifies the relative or absolute path to the publish settings file for the target subscription.
        /// </summary>
        private const string SettingsPath = @"C:\PublishSettings\sfa_beta.publishsettings";

        /// <summary>
        /// Specifies the subscription Id that we want to target.
        /// This subscription needs to be defined and found in the publish settings file.
        /// </summary>
        private const string SubscriptionId = "102d951b-78c0-4e48-80d4-a9c13baca2ad";

        /// <summary>
        /// Tests the creation of a Web Site that doesn't exist (puts a Guid part in the site name).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_CheckCreateWebSite_WithNewSite()
        {
            using (var client = CreateClient())
            {
                const string webspace = WebSpaceNames.NorthEuropeWebSpace;

                var parameters =
                    new WebSiteCreateParameters
                    {
                        Name = "fct-" + Guid.NewGuid().ToString().Split('-').Last(),
                        ServerFarm = await FlexConfiguration.WebPlanChooser.Choose(client, webspace)
                    };

                try
                {
                    await client.CheckCreateWebSite(webspace, parameters);

                    var webSite = await client.WebSites.GetAsync(webspace, parameters.Name, null);
                    Assert.IsNotNull(webSite);
                }
                finally
                {
                    client.WebSites.Delete(
                        webspace,
                        parameters.Name,
                        new WebSiteDeleteParameters
                        {
                            DeleteAllSlots = true,
                            DeleteEmptyServerFarm = false,
                            DeleteMetrics = true
                        });
                }
            }
        }

        /// <summary>
        /// Tests the creation of a Web Site that doesn't exist (puts a Guid part in the site name).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_CheckCreateWebSite_WithExistingSite()
        {
            using (var client = CreateClient())
            {
                const string webspace = WebSpaceNames.NorthEuropeWebSpace;

                var parameters =
                    new WebSiteCreateParameters
                    {
                        Name = "fct-" + Guid.NewGuid().ToString().Split('-').Last(),
                        ServerFarm = await FlexConfiguration.WebPlanChooser.Choose(client, webspace)
                    };

                try
                {
                    await client.WebSites.CreateAsync(webspace, parameters);
                    await client.CheckCreateWebSite(webspace, parameters);
                }
                finally
                {
                    client.WebSites.Delete(
                        webspace,
                        parameters.Name,
                        new WebSiteDeleteParameters
                        {
                            DeleteAllSlots = true,
                            DeleteEmptyServerFarm = false,
                            DeleteMetrics = true
                        });
                }
            }
        }

        /// <summary>
        /// Creates a standard <see cref="WebSiteManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </summary>
        /// <returns>
        /// A standard <see cref="WebSiteManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </returns>
        private static WebSiteManagementClient CreateClient()
        {
            var azureSubscription = new AzureSubscription(SettingsPath, SubscriptionId);

            return new WebSiteManagementClient(
                new CertificateCloudCredentials(
                    SubscriptionId,
                    new X509Certificate2(Convert.FromBase64String(azureSubscription.ManagementCertificate))));
        }
    }
}
