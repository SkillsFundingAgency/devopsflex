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
        /// Specifies the relative or absolute path to the publish settings file for the target subscription.
        /// </summary>
        private const string SettingsPath = @"..\..\sfa_beta.publishsettings";

        /// <summary>
        /// Specifies the subscription Id that we want to target.
        /// This subscription needs to be defined and found in the publish settings file.
        /// </summary>
        private const string SubscriptionId = "102d951b-78c0-4e48-80d4-a9c13baca2ad";

        /// <summary>
        /// Tests the activity execution by upscaling a VM to A2 without awaiting for it to be ready.
        /// This test created a workflow and uses the <see cref="WorkflowInvoker"/> to execute it.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_Execute_UpscaleA2_LiveVM()
        {
            var inputs = new Dictionary<string, object>
            {
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

            WorkflowInvoker.Invoke(
                new AzureVmScaleActivity
                {
                    SubscriptionId = SubscriptionId,
                    SettingsPath = SettingsPath
                },
                inputs);
        }

        /// <summary>
        /// Tests the activity execution by downscaling a VM to A1 without awaiting for it to be ready.
        /// This test created a workflow and uses the <see cref="WorkflowInvoker"/> to execute it.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_Execute_DownscaleA1_LiveVM()
        {
            var inputs = new Dictionary<string, object>
            {
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

            WorkflowInvoker.Invoke(
                new AzureVmScaleActivity
                {
                    SubscriptionId = SubscriptionId,
                    SettingsPath = SettingsPath
                },
                inputs);
        }

        /// <summary>
        /// Tests the activity execution by stoping a VM without awaiting for it to be ready.
        /// This test created a workflow and uses the <see cref="WorkflowInvoker"/> to execute it.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_Execute_ShutDown_LiveVM()
        {
            var inputs = new Dictionary<string, object>
            {
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

            WorkflowInvoker.Invoke(
                new AzureVmScaleActivity
                {
                    SubscriptionId = SubscriptionId,
                    SettingsPath = SettingsPath
                },
                inputs);
        }

        #endregion
    }
}
