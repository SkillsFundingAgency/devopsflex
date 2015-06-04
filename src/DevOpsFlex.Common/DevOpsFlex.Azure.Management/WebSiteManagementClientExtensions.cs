﻿namespace DevOpsFlex.Azure.Management
{
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using Core;
    using Data;
    using Data.Events;
    using Hyak.Common;
    using Microsoft.WindowsAzure.Management.WebSites;
    using Microsoft.WindowsAzure.Management.WebSites.Models;

    /// <summary>
    /// Extends the <see cref="WebSiteManagementClient"/> with usefull extensions that the devopsflex
    /// activities and commandlets need in order to achieve their execution.
    /// </summary>
    public static class WebSiteManagementClientExtensions
    {
        /// <summary>
        /// Gate object for the hosting plan choosing process.
        /// </summary>
        private static readonly object HostingPlanGate = new object();

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
            FlexStreams.BuildEventsObserver.OnNext(new CheckIfExistsEvent(AzureResource.WebSite, parameters.Name));

            try
            {
                service = await client.WebSites.GetAsync(webSpace, parameters.Name, null);
            }
            catch (CloudException cex)
            {
                if (cex.Error.Code != "NotFound") throw;
            }

            if (service != null)
            {
                FlexStreams.BuildEventsObserver.OnNext(new FoundExistingEvent(AzureResource.WebSite, parameters.Name));
                return;
            }

            await client.WebSites.CreateAsync(webSpace, parameters);
            FlexStreams.BuildEventsObserver.OnNext(new ProvisionEvent(AzureResource.WebSite, parameters.Name));
        }

        /// <summary>
        /// Checks for the existence of a specific Azure Web Site, if it doesn't exist it will create it.
        /// </summary>
        /// <param name="client">The <see cref="WebSiteManagementClient"/> that is performing the operation.</param>
        /// <param name="model">The DevOpsFlex rich model object that contains everything there is to know about this web site spec.</param>
        /// <returns>The async <see cref="Task"/> wrapper.</returns>
        public static async Task CreateWebSiteIfNotExistsAsync(this WebSiteManagementClient client, AzureWebSite model)
        {
            Contract.Requires(client != null);
            Contract.Requires(model != null);

            var webSpace = model.System.WebSpace.GetEnumDescription();
            string webPlan;

            lock (HostingPlanGate)
            {
                FlexStreams.BuildEventsObserver.OnNext(new CheckForParentResourceEvent(AzureResource.HostingPlan, AzureResource.WebSite, model.Name));
                webPlan = FlexConfiguration.WebPlanChooser.Choose(client, webSpace).Result;

                if (webPlan == null)
                {
                    var response = client.WebHostingPlans.Create(webSpace,
                        new WebHostingPlanCreateParameters
                        {
                            Name = model.System.LogicalName + "-" + webSpace,
                            NumberOfWorkers = 1,
                            SKU = SkuOptions.Standard,
                            WorkerSize = WorkerSizeOptions.Medium
                        });

                    webPlan = response.WebHostingPlan.Name;
                    FlexStreams.BuildEventsObserver.OnNext(new ProvisionEvent(AzureResource.HostingPlan, webPlan));
                }
                else
                {
                    FlexStreams.BuildEventsObserver.OnNext(new FoundExistingEvent(AzureResource.HostingPlan, webPlan));
                }
            }

            await CreateWebSiteIfNotExistsAsync(
                client,
                webSpace,
                new WebSiteCreateParameters
                {
                    Name = FlexDataConfiguration.GetNaming<AzureWebSite>().GetSlotName(model, FlexDataConfiguration.Branch, FlexDataConfiguration.Configuration),
                    ServerFarm = webPlan
                });
        }
    }
}
