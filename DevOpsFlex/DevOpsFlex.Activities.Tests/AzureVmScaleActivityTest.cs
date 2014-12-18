namespace DevOpsFlex.Activities.Tests
{
    using System.Activities;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Contains tests that target the workflow AzureVmScaleActivity directly.
    /// </summary>
    [TestClass]
    public class AzureVmScaleActivityTest
    {
        #region Integration Tests

        /// <summary>
        /// Specifies the name of the VM that we want to target in this set of Integration Tests.
        /// </summary>
        private const string VmName = "devopsflex-test";

        /// <summary>
        /// Tests the activity execution by upscaling a VM to A2 without awaiting for it to be ready.
        /// This test created a workflow and uses the <see cref="WorkflowInvoker"/> to execute it.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_Execute_UpscaleA2_NoWait_LiveVM()
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

        /// <summary>
        /// Tests the activity execution by downscaling a VM to A1 without awaiting for it to be ready.
        /// This test created a workflow and uses the <see cref="WorkflowInvoker"/> to execute it.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_Execute_DownscaleA1_NoWait_LiveVM()
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

        /// <summary>
        /// Tests the activity execution by stoping a VM without awaiting for it to be ready.
        /// This test created a workflow and uses the <see cref="WorkflowInvoker"/> to execute it.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_Execute_ShutDown_NoWait_LiveVM()
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

        #endregion
    }
}
