namespace DevOpsFlex.Activities.PublishSettings
{
    using System;
    using System.Linq;

    public class AzureSubscription
    {
        private readonly Subscription _data;

        public string ManagementCertificate { get { return _data.ManagementCertificate; } }

        public string SubscriptionName { get { return _data.Name; } }

        public AzureSubscription(string settingsPath, string subscriptionId)
        {
            _data = PublishData.FromFile(settingsPath)
                               .PublishProfiles.First()
                               .Subscriptions.SingleOrDefault(s => s.Id == subscriptionId);

            if(_data == null)
                throw new ArgumentException("Couldn't find the subscriptionId in the publish settings file", "subscriptionId");
        }
    }
}
