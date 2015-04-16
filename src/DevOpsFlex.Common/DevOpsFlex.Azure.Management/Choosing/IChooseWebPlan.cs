namespace DevOpsFlex.Azure.Management.Choosing
{
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.WebSites;

    /// <summary>
    /// Provides a contract to override the WebPlan choosing pipeline.
    /// </summary>
    public interface IChooseWebPlan
    {
        /// <summary>
        /// Finds a valid web plan in the subscription to create web sites on.
        /// </summary>
        /// <param name="client">The <see cref="WebSiteManagementClient"/> that is performing the operation.</param>
        /// <param name="webSpace">The name of the Web Space where the site should be.</param>
        /// <returns>A suitable web plan if one is found, null otherwise.</returns>
        Task<string> Choose(WebSiteManagementClient client, string webSpace);
    }
}
