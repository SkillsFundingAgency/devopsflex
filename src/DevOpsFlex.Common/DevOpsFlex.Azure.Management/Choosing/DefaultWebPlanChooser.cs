﻿namespace DevOpsFlex.Azure.Management.Choosing
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Hyak.Common;
    using Microsoft.WindowsAzure.Management.WebSites;
    using Microsoft.WindowsAzure.Management.WebSites.Models;

    /// <summary>
    /// Provides a default Web Plan chooser object.
    /// </summary>
    public class DefaultWebPlanChooser : IChooseWebPlan
    {
        /// <summary>
        /// Finds a valid web plan in the subscription to create web sites on.
        /// </summary>
        /// <param name="client">The <see cref="WebSiteManagementClient"/> that is performing the operation.</param>
        /// <param name="webSpace">The name of the Web Space where the site should be.</param>
        /// <returns>A suitable web plan name if one is found, null otherwise.</returns>
        public async Task<string> Choose(WebSiteManagementClient client, string webSpace)
        {
            try
            {
                return (await client.WebHostingPlans.ListAsync(webSpace, new CancellationToken()))
                    .FirstOrDefault(p => p.SKU == SkuOptions.Standard)?
                    .Name;
            }
            catch (CloudException cex)
            {
                if (cex.Error.Code != "NotFound") throw;

                return null;
            }
        }
    }
}
