namespace DevOpsFlex.Data.Migrations
{
    using System.Data.Entity.Migrations;

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
                    Name = "FCT",
                    LogicalName = "FCT"
                };

            context.Systems.AddOrUpdate(s => s.LogicalName, system);

            return system;
        }

        private static void SeedComponents(DevOpsFlexDbContext context, DevOpsSystem system)
        {
            context.Components.AddOrUpdate(
                c => c.LogicalName,
                new AzureCloudService
                {
                    System = system,
                    LogicalName = "ServiceBus",
                    Name = "CrossDomain.Integration NServiceBus role",
                    Label = "Main NServiceBus role with most of the endpoints",
                    PublishProjectTfsPath = "$/FCT/Main/CrossDomain.Integration/CrossDomain.Integration.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/CrossDomain.Integration/CrossDomain.Integration.sln"
                },
                new AzureCloudService
                {
                    System = system,
                    LogicalName = "OrgService",
                    Name = "Organisation service",
                    Label = "Organisation service",
                    PublishProjectTfsPath = "$/FCT/Main/OrganisationDomain/OrganisationDomain.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/OrganisationDomain/OrganisationDomain.sln"
                },
                new AzureCloudService
                {
                    System = system,
                    LogicalName = "FundingStreamConfig",
                    Name = "Funding Stream Config service",
                    Label = "Funding Stream Config service",
                    PublishProjectTfsPath = "$/FCT/Main/FundingStreamConfigDomain/FundingStreamConfigDomain.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/FundingStreamConfigDomain/FundingStreamConfigDomain.sln"
                },
                new SqlAzureDb
                {
                    System = system,
                    LogicalName = "ServiceBusDb",
                    Name = "NServiceBus persistency database",
                    Edition = SqlAzureEdition.Standard,
                    DTUs = 10,
                    MaximumDatabaseSizeInGB = 10,
                    CollationName = "SQL_Latin1_General_CP1_CI_AS"
                },
                new SqlAzureDb
                {
                    System = system,
                    LogicalName = "OrganisationDb",
                    Name = "Organisation service persistency database",
                    Edition = SqlAzureEdition.Standard,
                    DTUs = 10,
                    MaximumDatabaseSizeInGB = 10,
                    CollationName = "SQL_Latin1_General_CP1_CI_AS"
                },
                new AzureWebSite
                {
                    System = system,
                    LogicalName = "MocksSite",
                    Name = "Mocks services (Stubs)",
                    PublishProjectTfsPath = "$/FCT/Main/Mocks/FctServices/FctServices.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/Mocks/FctServices/FctServices.sln"
                },
                new AzureWebSite
                {
                    System = system,
                    LogicalName = "WebJobsSite",
                    Name = "Web Jobs hosting",
                    PublishProjectTfsPath = "$/FCT/Main/CrossDomain.Integration/CrossDomain.Integration.Azure.WebJobsHostSite.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/CrossDomain.Integration/CrossDomain.Integration.sln"
                },
                new AzureServiceBusNamespace
                {
                    System = system,
                    LogicalName = "IntegrationSb",
                    Name = "NServiceBus main namespace",
                    Region = ServiceBusRegions.NorthEurope
                },
                new AzureStorageContainer
                {
                    System = system,
                    LogicalName = "DataBusStorage",
                    Name = "NServiceBus DataBus storage",
                    ExternalAccess = ContainerExternalAccess.Private,
                    Acl = ContainerAcl.Read | ContainerAcl.Write
                },
                new AzureStorageContainer
                {
                    System = system,
                    LogicalName = "DocumentTemplates",
                    Name = "Document Templates for the document domain",
                    ExternalAccess = ContainerExternalAccess.Private,
                    Acl = ContainerAcl.Read | ContainerAcl.Write
                });
        }

        private static void SeedFirewallRules(DevOpsFlexDbContext context, DevOpsSystem system)
        {
            context.SqlFirewallRules.AddOrUpdate(
                r => r.Name,
                new SqlFirewallRule
                {
                    System = system,
                    Name = "SFA LAN",
                    StartIp = "193.240.137.225",
                    EndIp = "193.240.137.254"
                },
                new SqlFirewallRule
                {
                    System = system,
                    Name = "Cap UK",
                    StartIp = "212.167.5.1",
                    EndIp = "212.167.5.255"
                });
        }
    }
}
