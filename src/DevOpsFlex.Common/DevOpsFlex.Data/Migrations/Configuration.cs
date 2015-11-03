namespace DevOpsFlex.Data.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.WindowsAzure.Storage.Blob;

    internal sealed class Configuration : DbMigrationsConfiguration<DevOpsFlexDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DevOpsFlexDbContext context)
        {
            var fctSystem = SeedSystem(context);
            SeedComponents(context, fctSystem);
            SeedFirewallRules(context, fctSystem);
        }

        private static DevOpsSystem SeedSystem(DevOpsFlexDbContext context)
        {
            var system =
                new DevOpsSystem
                {
                    AfinityGroup = "FCTWest",
                    Location = SystemLocation.WestEurope,
                    WebSpace = SystemWebSpace.WestEurope,
                    StorageType = StorageType.LocalRedundant,
                    Name = "FCT",
                    LogicalName = "FCT"
                };

            context.Systems.AddOrUpdate(s => s.LogicalName, system);

            return context.Systems.Single(s => s.LogicalName == system.LogicalName);
        }

        private static void SeedComponents(DevOpsFlexDbContext context, DevOpsSystem system)
        {
            context.Components.AddOrUpdate(
                c => c.LogicalName,
                new AzureCloudService
                {
                    System = system,
                    SystemId = system.Id,
                    LogicalName = "ServiceBus",
                    Name = "CrossDomain.Integration NServiceBus role",
                    Label = "Main NServiceBus role with most of the endpoints",
                    ReserveIp = true,
                    PublishProjectTfsPath = "$/FCT/Main/CrossDomain.Integration/CrossDomain.Integration.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/CrossDomain.Integration/CrossDomain.Integration.sln"
                },
                new AzureCloudService
                {
                    System = system,
                    SystemId = system.Id,
                    LogicalName = "OrgService",
                    Name = "Organisation service",
                    Label = "Organisation service",
                    ReserveIp = true,
                    PublishProjectTfsPath = "$/FCT/Main/OrganisationDomain/OrganisationDomain.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/OrganisationDomain/OrganisationDomain.sln"
                },
                new AzureCloudService
                {
                    System = system,
                    SystemId = system.Id,
                    LogicalName = "FSC",
                    Name = "Funding Stream Config service",
                    Label = "Funding Stream Config service",
                    ReserveIp = true,
                    PublishProjectTfsPath = "$/FCT/Main/FundingStreamConfigDomain/FundingStreamConfigDomain.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/FundingStreamConfigDomain/FundingStreamConfigDomain.sln"
                },
                new AzureCloudService
                {
                    System = system,
                    SystemId = system.Id,
                    LogicalName = "ContractService",
                    Name = "Contract Management service",
                    Label = "Contract Management service",
                    ReserveIp = true,
                    PublishProjectTfsPath = "$/FCT/Main/ContractManagementDomain/ContractManagementDomain.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/ContractManagementDomain/ContractManagementDomain.sln"
                },
                new SqlAzureDb
                {
                    System = system,
                    SystemId = system.Id,
                    LogicalName = "ServiceBusDb",
                    Name = "NServiceBus persistency database",
                    Edition = SqlAzureEdition.Standard,
                    ServiceObjective = SqlServiceObjectives.S1,
                    MaximumDatabaseSizeInGB = 10,
                    CollationName = "SQL_Latin1_General_CP1_CI_AS",
                    CreateAppUser = false
                },
                new SqlAzureDb
                {
                    System = system,
                    SystemId = system.Id,
                    LogicalName = "OrganisationDb",
                    Name = "Organisation service persistency database",
                    Edition = SqlAzureEdition.Standard,
                    ServiceObjective = SqlServiceObjectives.S1,
                    MaximumDatabaseSizeInGB = 10,
                    CollationName = "SQL_Latin1_General_CP1_CI_AS",
                    CreateAppUser = false
                },
                new AzureWebSite
                {
                    System = system,
                    SystemId = system.Id,
                    LogicalName = "Mocks",
                    Name = "Mocks services (Stubs)",
                    PublishProjectTfsPath = "$/FCT/Main/Mocks/FctServices/FctServices.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/Mocks/FctServices/FctServices.sln"
                },
                new AzureWebSite
                {
                    System = system,
                    SystemId = system.Id,
                    LogicalName = "WebJobs",
                    Name = "Web Jobs hosting",
                    PublishProjectTfsPath = "$/FCT/Main/CrossDomain.Integration/CrossDomain.Integration.Azure.WebJobsHostSite.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/CrossDomain.Integration/CrossDomain.Integration.sln"
                },
                new AzureServiceBusNamespace
                {
                    System = system,
                    SystemId = system.Id,
                    LogicalName = "IntegrationSb",
                    Name = "NServiceBus main namespace",
                    Region = ServiceBusRegions.NorthEurope
                },
                new AzureStorageContainer
                {
                    System = system,
                    SystemId = system.Id,
                    LogicalName = "DataBus",
                    Name = "NServiceBus DataBus storage",
                    PublicAccess = BlobContainerPublicAccessType.Off,
                    Acl = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List
                },
                new AzureStorageContainer
                {
                    System = system,
                    SystemId = system.Id,
                    LogicalName = "DocTemplates",
                    Name = "Document Templates for the document domain",
                    PublicAccess = BlobContainerPublicAccessType.Off,
                    Acl = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List
                });
        }

        private static void SeedFirewallRules(DevOpsFlexDbContext context, DevOpsSystem system)
        {
            var sfaRule =
                new SqlFirewallRule
                {
                    System = system,
                    SystemId = system.Id,
                    Name = "SFA LAN",
                    StartIp = "193.240.137.225",
                    EndIp = "193.240.137.254"
                };

            var capRule =
                new SqlFirewallRule
                {
                    System = system,
                    SystemId = system.Id,
                    Name = "Cap UK",
                    StartIp = "212.167.5.1",
                    EndIp = "212.167.5.255"
                };

            context.SqlFirewallRules.AddOrUpdate(
                r => r.Name,
                sfaRule,
                capRule);
        }
    }
}
