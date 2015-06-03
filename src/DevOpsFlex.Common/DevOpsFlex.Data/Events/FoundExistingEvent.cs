namespace DevOpsFlex.Data.Events
{
    using Core;

    public class FoundExistingEvent : BuildEvent
    {
        public AzureResource ResourceType { get; private set; }

        public string ResourceName { get; private set; }

        public FoundExistingEvent(AzureResource resourceType, string resourceName)
            : base(BuildEventType.Information, BuildEventImportance.Low, $"Found an existing suitable {resourceType.GetEnumDescription()}: {resourceName}.")
        {
            ResourceType = resourceType;
            ResourceName = resourceName;
        }
    }
}
