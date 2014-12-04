namespace DevOpsFlex.Activities.Tests
{
    using System;
    using System.Linq;
    using PublishSettings;

    public static class AzureSubscription
    {
        private const string PublishSettingsPath = @"..\..\sfa_beta.publishsettings";
        private const string SubscriptionName = "Beta";

        private static readonly Subscription Data;

        public static string SubscriptionId
        {
            get { return Data.Id; }
        }

        public static string ManagementCertificate
        {
            get { return Data.ManagementCertificate; }
        }

        static AzureSubscription()
        {
            Data = PublishData.FromFile(PublishSettingsPath)
                              .PublishProfiles.First()
                              .Subscriptions.SingleOrDefault(s => s.Name == SubscriptionName);

            if(Data == null)
                throw new ArgumentOutOfRangeException(
                    "SubscriptionName",
                    "Invalid SubscriptionName constant in the AzureSubscription class.");
        }
    }
}
