namespace DevOpsFlex.Azure.Management.Choosing
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.WebSites;
    using Microsoft.WindowsAzure.Management.WebSites.Models;

    /// <summary>
    /// Provides a default WebPlan chooser object.
    /// </summary>
    public class DefaultWebPlanChooser : IChooseWebPlan
    {
        /// <summary>
        /// Finds a valid web plan in the subscription to create web sites on.
        /// </summary>
        /// <param name="client">The <see cref="WebSiteManagementClient"/> that is performing the operation.</param>
        /// <param name="webSpace">The name of the Web Space where the site should be.</param>
        /// <returns>A suitable web plan if one is found, null otherwise.</returns>
        public async Task<string> Choose(WebSiteManagementClient client, string webSpace)
        {
            var webPlan = (await client.WebHostingPlans.ListAsync(webSpace, new CancellationToken())).FirstOrDefault(p => p.SKU == SkuOptions.Standard);

            return webPlan == null ?
                null :
                webPlan.Name;
        }
    }
}
