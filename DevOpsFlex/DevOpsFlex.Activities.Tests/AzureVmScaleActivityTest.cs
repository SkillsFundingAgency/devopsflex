namespace DevOpsFlex.Activities.Tests
{
    using System.Activities;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AzureVmScaleActivityTest
    {
        private const string VmName = "devopsflex-test";

        [TestMethod, TestCategory("Integration")]
        public void Test_Execute_UpscaleA2_LiveVM()
        {
            var inputs = new Dictionary<string, object>
            {
                {"SubscriptionId", AzureSubscription.SubscriptionId},
                {"ManagementCertificate", AzureSubscription.ManagementCertificate},
                {
                    "VirtualMachines",
                    new[]
                    {
                        new VmScaleDefinition
                        {
                            Name = VmName,
                            Size = VirtualMachineSize.Medium
                        }
                    }
                }
            };

            WorkflowInvoker.Invoke(new AzureVmScaleActivity(), inputs);
        }

        [TestMethod, TestCategory("Integration")]
        public void Test_Execute_DownscaleA1_LiveVM()
        {
            var inputs = new Dictionary<string, object>
            {
                {"SubscriptionId", AzureSubscription.SubscriptionId},
                {"ManagementCertificate", AzureSubscription.ManagementCertificate},
                {
                    "VirtualMachines",
                    new[]
                    {
                        new VmScaleDefinition
                        {
                            Name = VmName,
                            Size = VirtualMachineSize.Small
                        }
                    }
                }
            };

            WorkflowInvoker.Invoke(new AzureVmScaleActivity(), inputs);
        }

        [TestMethod, TestCategory("Integration")]
        public void Test_Execute_ShutDown_LiveVM()
        {
            var inputs = new Dictionary<string, object>
            {
                {"SubscriptionId", AzureSubscription.SubscriptionId},
                {"ManagementCertificate", AzureSubscription.ManagementCertificate},
                {
                    "VirtualMachines",
                    new[]
                    {
                        new VmScaleDefinition
                        {
                            Name = VmName,
                            Size = VirtualMachineSize.Stop
                        }
                    }
                }
            };

            WorkflowInvoker.Invoke(new AzureVmScaleActivity(), inputs);
        }
    }
}
