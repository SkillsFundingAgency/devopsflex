namespace DevOpsFlex.Activities.Tests
{
    using System.Activities;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AzureVmScaleActivityTest
    {
        private const string VmName = "wjn-sql2008";

        [TestMethod, TestCategory("Integration")]
        public void Test_Execute_UpscaleA2_LiveVM()
        {
            var inputs = new Dictionary<string, object>
            {
                {"SubscriptionId", AzureSubscription.SubscriptionId},
                {"ManagementCertificate", AzureSubscription.ManagementCertificate},
                {"VirtualMachines", new[] {VmName}},
                {"VirtualMachineSize", VirtualMachineSize.Medium},
            };

            WorkflowInvoker.Invoke(new AzureVmScaleActivity(), inputs);
        }

        [TestMethod, TestCategory("Integration")]
        public void Test_Execute_DownscaleA0_LiveVM()
        {
            var inputs = new Dictionary<string, object>
            {
                {"SubscriptionId", AzureSubscription.SubscriptionId},
                {"ManagementCertificate", AzureSubscription.ManagementCertificate},
                {"VirtualMachines", new[] {VmName}},
                {"VirtualMachineSize", VirtualMachineSize.ExtraSmall},
            };

            WorkflowInvoker.Invoke(new AzureVmScaleActivity(), inputs);
        }
    }
}
