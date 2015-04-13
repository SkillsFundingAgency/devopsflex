namespace DevOpsFlex.Data.Naming
{
    using System;
    using Core;

    class CloudServiceNaming : IName<AzureCloudService>
    {
        public string GetSlotName(AzureCloudService component)
        {
            return component.System.LogicalName.ToLower() + "-" + component.LogicalName.ToLower();
        }

        public string GetMinimalSlotName(AzureCloudService component)
        {
            return component.System.Name.ToLower() + "-" + component.LogicalName.GetUpperConcat();
        }
    }
}
