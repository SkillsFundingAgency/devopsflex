namespace DevOpsFlex.Azure.Management.Tests
{
    using System;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Core;
    using Data;
    using Data.PublishSettings;
    using Microsoft.Azure;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.Sql;

    [TestClass]
    public class SqlManagementClientExtensionsTest
    {

        #region Integration Tests

        /// <summary>
        /// Specifies the relative or absolute path to the publish settings file for the target subscription.
        /// </summary>
        private const string SettingsPath = @"C:\PublishSettings\sfa_beta.publishsettings";

        /// <summary>
        /// Specifies the subscription Id that we want to target.
        /// This subscription needs to be defined and found in the publish settings file.
        /// </summary>
        private const string SubscriptionId = "102d951b-78c0-4e48-80d4-a9c13baca2ad";

        [TestMethod]
        public async Task Foo()
        {
            using (var client = CreateClient())
            {
                await client.CheckCreateDatabase("acx03vg6p0", "fct-db123", SqlAzureEdition.Standard.GetEnumDescription(), "SQL_Latin1_General_CP1_CI_AS", 5);
            }
        }

        #endregion

        /// <summary>
        /// Creates a standard <see cref="SqlManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </summary>
        /// <returns>
        /// A standard <see cref="SqlManagementClient"/> that targets the Azure subscription
        /// specified in the <see cref="AzureSubscription"/> static class.
        /// </returns>
        private static SqlManagementClient CreateClient()
        {
            var azureSubscription = new AzureSubscription(SettingsPath, SubscriptionId);

            return new SqlManagementClient(
                new CertificateCloudCredentials(
                    SubscriptionId,
                    new X509Certificate2(Convert.FromBase64String(azureSubscription.ManagementCertificate))));
        }
    }
}
