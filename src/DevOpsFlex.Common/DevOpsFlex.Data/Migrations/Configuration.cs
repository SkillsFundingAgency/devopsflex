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
            var fctSystem =
                new DevOpsSystem
                {
                    AfinityGroup = "FCTWest",
                    Location = SystemLocation.WestEurope,
                    WebSpace = SystemWebSpace.WestEurope,
                    Name = "FCT",
                    LogicalName = "FCT"
                };

            context.Systems.AddOrUpdate(s => s.Name, fctSystem);

            context.Components.AddOrUpdate(
                c => c.LogicalName,
                new AzureCloudService
                {
                    System = fctSystem,
                    LogicalName = "ServiceBus",
                    Name = "CrossDomain.Integration NServiceBus role",
                    Label = "Main NServiceBus role with most of the endpoints",
                    PublishProjectTfsPath = "$/FCT/Main/CrossDomain.Integration/CrossDomain.Integration.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/CrossDomain.Integration/CrossDomain.Integration.sln"
                },
                new AzureCloudService
                {
                    System = fctSystem,
                    LogicalName = "OrgService",
                    Name = "Organisation service",
                    Label = "Organisation service",
                    PublishProjectTfsPath = "$/FCT/Main/OrganisationDomain/OrganisationDomain.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/OrganisationDomain/OrganisationDomain.sln"
                },
                new AzureCloudService
                {
                    System = fctSystem,
                    LogicalName = "FundingStreamConfig",
                    Name = "Funding Stream Config service",
                    Label = "Funding Stream Config service",
                    PublishProjectTfsPath = "$/FCT/Main/FundingStreamConfigDomain/FundingStreamConfigDomain.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/FundingStreamConfigDomain/FundingStreamConfigDomain.sln"
                },
                new SqlAzureDb
                {
                    System = fctSystem,
                    LogicalName = "ServiceBusDb",
                    Name = "NServiceBus persistency database",
                    Edition = SqlAzureEdition.Business,
                    DTUs = 10,
                    MaximumDatabaseSizeInGB = 10,
                    CollationName = "SQL_Latin1_General_CP1_CI_AS"
                },
                new SqlAzureDb
                {
                    System = fctSystem,
                    LogicalName = "OrganisationDb",
                    Name = "Organisation service persistency database",
                    Edition = SqlAzureEdition.Business,
                    DTUs = 10,
                    MaximumDatabaseSizeInGB = 10,
                    CollationName = "SQL_Latin1_General_CP1_CI_AS"
                },
                new AzureWebSite
                {
                    System = fctSystem,
                    LogicalName = "MocksSite",
                    Name = "Mocks services (Stubs)",
                    PublishProjectTfsPath = "$/FCT/Main/Mocks/FctServices/FctServices.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/Mocks/FctServices/FctServices.sln"
                },
                new AzureWebSite
                {
                    System = fctSystem,
                    LogicalName = "WebJobsSite",
                    Name = "Web Jobs hosting",
                    PublishProjectTfsPath = "$/FCT/Main/CrossDomain.Integration/CrossDomain.Integration.Azure.WebJobsHostSite.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/CrossDomain.Integration/CrossDomain.Integration.sln"
                },
                new AzureServiceBusNamespace
                {
                    System = fctSystem,
                    LogicalName = "IntegrationSb",
                    Name = "NServiceBus main namespace"
                },
                new AzureStorageContainer
                {
                    System = fctSystem,
                    LogicalName = "DataBusStorage",
                    Name = "NServiceBus DataBus storage",
                    ExternalAccess = ContainerExternalAccess.Private,
                    Acl = ContainerAcl.Read | ContainerAcl.Write
                });
        }
    }
}
