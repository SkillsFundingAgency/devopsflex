﻿namespace DevOpsFlex.Azure.Management.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.Compute;
    using Microsoft.WindowsAzure.Management.Compute.Models;
    using Microsoft.WindowsAzure.Management.Models;

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
        /// Tests the ResizeVmAsync extension by upscaling a VM to A2.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_ResizeVm_UpscaleA2_LiveVM()
        {
            using (var client = ManagementClient.CreateComputeClient())
            {
                await client.ResizeVmAsync(VmName, VirtualMachineSize.Medium.GetEnumDescription());
            }
        }

        /// <summary>
        /// Tests the ResizeVmAsync extension by downscaling a VM to A1.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_ResizeVm_DownscaleA1_LiveVM()
        {
            using (var client = ManagementClient.CreateComputeClient())
            {
                await client.ResizeVmAsync(VmName, VirtualMachineSize.Small.GetEnumDescription());
            }
        }

        /// <summary>
        /// Tests the DeallocateVmAsync extension by stopping a VM in deallocated mode.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_DeallocateVm_LiveVM()
        {
            using (var client = ManagementClient.CreateComputeClient())
            {
                await client.DeallocateVmAsync(VmName);
            }
        }

        /// <summary>
        /// Tests the ListVmsAsync against a subscription that needs VMs on it by asserting it returns at least one.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_ListVmsAsync_LiveSubscription()
        {
            using (var client = ManagementClient.CreateComputeClient())
            {
                var vms = (await client.ListVmsAsync()).ToList();

                Assert.IsTrue(vms.Any());
            }
        }

        /// <summary>
        /// Tests the creation of a Cloud Service that doesn't exist (puts a Guid part in the service name).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_CheckCreateCloudService_WithNewService()
        {
            using (var client = ManagementClient.CreateComputeClient())
            {
                var parameters =
                    new HostedServiceCreateParameters
                    {
                        Label = "Integration Test",
                        Location = LocationNames.NorthEurope,
                        ServiceName = "fct-" + Guid.NewGuid().ToString().Split('-').Last()
                    };

                try
                {
                    await client.CreateServiceIfNotExistsAsync(parameters);

                    var service = await client.HostedServices.GetAsync(parameters.ServiceName);
                    Assert.IsNotNull(service);
                }
                finally
                {
                    client.HostedServices.Delete(parameters.ServiceName);
                }
            }
        }

        /// <summary>
        /// Tests the creation of a Cloud Service that doesn't exist (puts a Guid part in the service name).
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_CheckCreateCloudService_WithExistingService()
        {
            using (var client = ManagementClient.CreateComputeClient())
            {
                var parameters =
                    new HostedServiceCreateParameters
                    {
                        Label = "Integration Test",
                        Location = LocationNames.NorthEurope,
                        ServiceName = "fct-" + Guid.NewGuid().ToString().Split('-').Last()
                    };

                try
                {
                    await client.HostedServices.CreateAsync(parameters);
                    await client.CreateServiceIfNotExistsAsync(parameters);
                }
                finally
                {
                    client.HostedServices.Delete(parameters.ServiceName);
                }
            }
        }

        #endregion

    }
}
