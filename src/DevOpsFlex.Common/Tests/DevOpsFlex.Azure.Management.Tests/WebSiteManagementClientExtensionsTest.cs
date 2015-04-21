namespace DevOpsFlex.Azure.Management.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Management.WebSites;
    using Microsoft.WindowsAzure.Management.WebSites.Models;

    /// <summary>
    /// Contains tests that target the <see cref="WebSiteManagementClientExtensions"/> extension class
    /// for the <see cref="WebSiteManagementClient"/>.
    /// </summary>
    [TestClass]
    public class WebSiteManagementClientExtensionsTest
    {

        #region Integration Tests

        /// <summary>
        /// Tests the creation of a Web Site that doesn't exist (puts a Guid part in the site name).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_CheckCreateWebSite_WithNewSite()
        {
            using (var client = ManagementClient.CreateWebSiteClient())
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
            using (var client = ManagementClient.CreateWebSiteClient())
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

        #endregion

    }
}
