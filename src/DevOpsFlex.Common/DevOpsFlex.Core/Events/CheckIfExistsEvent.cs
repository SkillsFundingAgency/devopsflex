namespace DevOpsFlex.Core.Events
{
    public class CheckIfExistsEvent : BuildEvent
    {
        public AzureResource ResourceType { get; private set; }

        public string ResourceName { get; private set; }

        public CheckIfExistsEvent(AzureResource resourceType, string resourceName)
            : base(BuildEventType.Information, BuildEventImportance.Low, $"Checking if the {resourceType.GetEnumDescription()} {resourceName} exists in the subscription.")
        {
            ResourceType = resourceType;
            ResourceName = resourceName;
        }
    }
}
