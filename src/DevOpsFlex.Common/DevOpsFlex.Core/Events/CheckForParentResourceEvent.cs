namespace DevOpsFlex.Core.Events
{
    public class CheckForParentResourceEvent : BuildEvent
    {
        public AzureResource ResourceType { get; private set; }

        public string ResourceName { get; private set; }

        public CheckForParentResourceEvent(AzureResource parentResourceType, AzureResource resourceType, string resourceName)
            : base(
                  BuildEventType.Information,
                  BuildEventImportance.Low,
                  $"Checking if a suitable {parentResourceType.GetEnumDescription()} that we can use exists in the subscription for the {resourceType.GetEnumDescription()} {resourceName}.")
        {
            ResourceType = resourceType;
            ResourceName = resourceName;
        }
    }
}
