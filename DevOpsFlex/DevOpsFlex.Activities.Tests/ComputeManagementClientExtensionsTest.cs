namespace DevOpsFlex.Activities.Tests
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Management.Compute;

    /// <summary>
    /// Contains tests that target the set of extensions written for the <see cref="ComputeManagementClient"/>.
    /// </summary>
    [TestClass]
    public class ComputeManagementClientExtensionsTest
    {

        #region Integration Tests

        /// <summary>
        /// Specifies the name of the VM that we want to target in this set of Integration Tests.
        /// </summary>
        private const string VmName = "devopsflex-test";

        /// <summary>
        /// Tests the ResizeVm extension by upscaling a VM to A2.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_ResizeVm_UpscaleA2_LiveVM()
        {
            using (var client = CreateClient())
            {
                client.ResizeVm(VmName, VirtualMachineSize.Medium.ToAzureString());
            }
        }

        /// <summary>
        /// Tests the ResizeVm extension by downscaling a VM to A1.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_ResizeVm_DownscaleA1_LiveVM()
        {
            using (var client = CreateClient())
            {
                client.ResizeVm(VmName, VirtualMachineSize.Small.ToAzureString());
            }
        }

        /// <summary>
        /// Tests the DeallocateVm extension by stopping a VM in deallocated mode.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public void Test_DeallocateVm_LiveVM()
        {
            using (var client = CreateClient())
            {
                client.DeallocateVm(VmName);
            }
        }

        #endregion

        /// <summary>
        /// Creates a standard <see cref="ComputeManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </summary>
        /// <returns>
        /// A standard <see cref="ComputeManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </returns>
        private static ComputeManagementClient CreateClient()
        {
            return new ComputeManagementClient(new CertificateCloudCredentials(
                AzureSubscription.SubscriptionId,
                new X509Certificate2(Convert.FromBase64String(AzureSubscription.ManagementCertificate))));
        }
    }
}
