namespace DevOpsFlex.Azure.Management.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Core;
    using Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.Network;
    using Microsoft.WindowsAzure.Management.Network.Models;

    /// <summary>
    /// Contains tests that target the <see cref="NetworkManagementClientExtensions"/> extension class
    /// for the <see cref="NetworkManagementClient"/>.
    /// </summary>
    [TestClass]
    public class NetworkManagementClientExtensionsTest
    {
        /// <summary>
        /// Tests CheckCreateReservedIp with the creation of a new Reserved IP that doesn't exist in the subscription.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_CheckCreateReservedIp_WithNewReservedIp()
        {
            var location = SystemLocation.NorthEurope.GetEnumDescription();

            using (var client = ManagementClient.CreateNetworkClient())
            {
                var ipName = "fct-" + Guid.NewGuid().ToString().Split('-').Last() + "-rip";

                try
                {
                    await client.CheckCreateReservedIp(ipName, location);

                    var reservedIp = await client.ReservedIPs.GetAsync(ipName, new CancellationToken());
                    Assert.IsNotNull(reservedIp);
                }
                finally
                {
                    client.ReservedIPs.Delete(ipName);
                }
            }
        }

        /// <summary>
        /// Tests CheckCreateReservedIp with the creation of a new Reserved IP that already exists in the subscription.
        /// </summary>
        [TestMethod, TestCategory("Integration")]
        public async Task Test_CheckCreateReservedIp_WithExistingReservedIp()
        {
            var location = SystemLocation.NorthEurope.GetEnumDescription();

            using (var client = ManagementClient.CreateNetworkClient())
            {
                var ipName = "fct-" + Guid.NewGuid().ToString().Split('-').Last() + "-rip";

                try
                {
                    await client.ReservedIPs.CreateAsync(
                        new NetworkReservedIPCreateParameters
                        {
                            Name = ipName,
                            Location = location
                        });

                    await client.ReservedIPs.GetAsync(ipName, new CancellationToken());
                }
                finally
                {
                    client.ReservedIPs.Delete(ipName);
                }
            }
        }
    }
}
