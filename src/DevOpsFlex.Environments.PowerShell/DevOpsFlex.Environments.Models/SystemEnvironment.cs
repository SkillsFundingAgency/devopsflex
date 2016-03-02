namespace DevOpsFlex.Environments.PowerShell
{
    using System;

    public class SystemSubscription
    {
        public string Name { get; }

        public string SystemName { get; }

        public string TemplatePath { get; }

        public Guid SubscriptionId { get; }

        public string Location { get; }

        public string RgName { get; }

        // ## USE ARRAYS, NOT IENUMERABLES AS POWERSHELL DOESN'T LIKE THEM!

        // ## NON-TEMPLATED RESOURCES

        // ## ENVIRONMENTS
    }
}
