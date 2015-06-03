namespace DevOpsFlex.Data.Events
{
    using Core;

    public class ProvisionEvent : BuildEvent
    {
        public AzureResource ResourceType { get; private set; }

        public string ResourceName { get; private set; }

        public ProvisionEvent(AzureResource resourceType, string resourceName)
            : base(BuildEventType.Information, BuildEventImportance.High, $"Provisioned the {resourceType.GetEnumDescription()} {resourceName}.")
        {
            ResourceType = resourceType;
            ResourceName = resourceName;
        }
    }
}
