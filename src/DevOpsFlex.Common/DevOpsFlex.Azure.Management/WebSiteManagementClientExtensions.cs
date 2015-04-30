namespace DevOpsFlex.Azure.Management
{
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Management.WebSites;
    using Microsoft.WindowsAzure.Management.WebSites.Models;

    /// <summary>
    /// Extends the <see cref="WebSiteManagementClient"/> with usefull extensions that the devopsflex
    /// activities and commandlets need in order to achieve their execution.
    /// </summary>
    public static class WebSiteManagementClientExtensions
    {
        /// <summary>
        /// Checks for the existence of a specific Azure Web Site, if it doesn't exist it will create it.
        /// </summary>
        /// <param name="client">The <see cref="WebSiteManagementClient"/> that is performing the operation.</param>
        /// <param name="webSpace">The name of the Web Space where the site should be.</param>
        /// <param name="parameters">The <see cref="WebSiteCreateParameters"/> that define the site we want to create.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CreateWebSiteIfNotExistsAsync(this WebSiteManagementClient client, string webSpace, WebSiteCreateParameters parameters)
        {
            Contract.Requires(client != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(webSpace));
            Contract.Requires(parameters != null);

            WebSiteGetResponse service = null;

            try
            {
                service = await client.WebSites.GetAsync(webSpace, parameters.Name, null);
            }
            catch (CloudException cex)
            {
                if (cex.ErrorCode != "NotFound") throw;
            }

            if (service != null) return;

            await client.WebSites.CreateAsync(webSpace, parameters);
        }
    }
}
