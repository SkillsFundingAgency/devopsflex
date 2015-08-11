namespace DevOpsFlex.CRM.Management
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Events;
    using Microsoft.Xrm.Sdk.Deployment;

    /// <summary>
    /// Contains extension methods to the <see cref="DeploymentServiceClient"/> object in the CRM SDK Deployment library.
    /// </summary>
    public static class DeploymentServiceClientExtensions
    {
        /// <summary>
        /// CRM pooling interval in miliseconds used anytime something needs to constantly pool CRM.
        /// </summary>
        private const int CrmPoolingInterval = 5000;

        /// <summary>
        /// Creates a new Organization in CRM.
        /// </summary>
        /// <param name="client">The <see cref="DeploymentServiceClient"/> that we are using to call CRM.</param>
        /// <param name="organization">The <see cref="Organization"/> model object containing all the information about the new organization.</param>
        /// <param name="eventStream">The Rx event stream used to push build events onto.</param>
        public static async Task CreateOrganizationAsync(this DeploymentServiceClient client, Organization organization, IObserver<BuildEvent> eventStream)
        {
            organization.SqlServerName = organization.SqlServerName.Replace(".cloudapp.net", ""); // TODO: FIX THIS HACK

            var operationId = await Task.Factory.StartNew(() =>
                ((BeginCreateOrganizationResponse)client.Execute(new BeginCreateOrganizationRequest { Organization = organization })).OperationId);

            eventStream.OnNext(new BuildEvent(BuildEventType.Information, BuildEventImportance.Low, $"Started the Request for Creating the Organisation : {organization.FriendlyName}"));

            var retrieveOperationStatus =
                new RetrieveRequest
                {
                    EntityType = DeploymentEntityType.DeferredOperationStatus,
                    InstanceTag = new EntityInstanceId { Id = operationId }
                };

            DeferredOperationStatus deferredOperationStatus = null;
            var progressEvent = new ProgressBuildEvent($"Trying to Create the Organisation {organization.FriendlyName} [In Progress]");
            eventStream.OnNext(progressEvent);

            do
            {
                await Task.Factory.StartNew(() => deferredOperationStatus = (DeferredOperationStatus)((RetrieveResponse)client.Execute(retrieveOperationStatus)).Entity);
                await Task.Delay(CrmPoolingInterval);

                progressEvent.Tick(5);
            } while (deferredOperationStatus.State != DeferredOperationState.Processing &&
                     deferredOperationStatus.State != DeferredOperationState.Completed);

            var retrieveReqServer =
                new RetrieveRequest
                {
                    EntityType = DeploymentEntityType.Organization,
                    InstanceTag = new EntityInstanceId { Name = organization.UniqueName }
                };

            var orgState = OrganizationState.Pending;
            progressEvent = new ProgressBuildEvent($"Checking that the Organisation {organization.FriendlyName} is Enabled [In Progress]");
            eventStream.OnNext(progressEvent);

            do
            {
                await Task.Factory.StartNew(() => orgState = ((Organization)((RetrieveResponse)client.Execute(retrieveReqServer)).Entity).State);
                await Task.Delay(CrmPoolingInterval);

                progressEvent.Tick(10);
            } while (orgState != OrganizationState.Enabled);

        }

        /// <summary>
        /// Checks if a given organization exists or not in CRM.
        /// </summary>
        /// <param name="client">The <see cref="DeploymentServiceClient"/> that we are using to call CRM.</param>
        /// <param name="organization">The name of the Organization we want to disable.</param>
        /// <returns>True if the organization exists in CRM, false otherwise.</returns>
        public static async Task<bool> OrganizationExists(this DeploymentServiceClient client, string organization)
        {
            return await Task.Factory.StartNew(() => client.RetrieveAll(DeploymentEntityType.Organization)
                                                           .Any(item => item.Name == organization));
        }

        /// <summary>
        /// Disables an Organization in CRM.
        /// </summary>
        /// <param name="client">The <see cref="DeploymentServiceClient"/> that we are using to call CRM.</param>
        /// <param name="organization">The name of the Organization we want to disable.</param>
        /// <param name="eventStream">The Rx event stream used to push build events onto.</param>
        public static async Task DisableOrganizationAsync(this DeploymentServiceClient client, string organization, IObserver<BuildEvent> eventStream)
        {
            EntityInstanceId org = null;
            await Task.Factory.StartNew(() => org = client.RetrieveAll(DeploymentEntityType.Organization)
                                                          .FirstOrDefault(item => item.Name == organization));

            if (org == null)
            {
                throw new ArgumentException($"{organization} Organisation not found", nameof(organization));
            }

            var orgToRemove = (Organization)client.Retrieve(DeploymentEntityType.Organization, new EntityInstanceId { Id = org.Id });

            orgToRemove.State = OrganizationState.Disabled;
            await Task.Factory.StartNew(() => client.Update(orgToRemove));
        }

        /// <summary>
        /// Deletes an Organization in CRM.
        /// </summary>
        /// <param name="client">The <see cref="DeploymentServiceClient"/> that we are using to call CRM.</param>
        /// <param name="organization">The name of the Organization we want to delete.</param>
        /// <param name="eventStream">The Rx event stream used to push build events onto.</param>
        public static async Task DeleteOrganizationAsync(this DeploymentServiceClient client, string organization, IObserver<BuildEvent> eventStream)
        {
            EntityInstanceId org = null;
            await Task.Factory.StartNew(() => org = client.RetrieveAll(DeploymentEntityType.Organization)
                                                          .FirstOrDefault(item => item.Name == organization));

            if (org == null)
            {
                throw new ArgumentException($"{organization} Organisation not found", nameof(organization));
            }

            var orgToDisable = (Organization)client.Retrieve(DeploymentEntityType.Organization, new EntityInstanceId { Id = org.Id });

            if (orgToDisable.State != OrganizationState.Disabled)
            {
                throw new InvalidOperationException($"The Organization needs to be in the Disabled state to be deleted and currently is in the {orgToDisable.State} state");
            }

            await Task.Factory.StartNew(() => client.Delete(DeploymentEntityType.Organization, new EntityInstanceId { Id = orgToDisable.Id }));
        }
    }
}
