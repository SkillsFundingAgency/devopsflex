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
                    Name = "FCT"
                };

            context.Systems.AddOrUpdate(s => s.Name, fctSystem);

            context.Components.AddOrUpdate(
                c => c.LogicalName,
                new AzureCloudService
                {
                    System = fctSystem,
                    Label = "Main NServiceBus role with most of the endpoints",
                    LogicalName = "ServiceBus",
                    Name = "CrossDomain.Integration NServiceBus role",
                    PublishProjectTfsPath = "$/FCT/Main/CrossDomain.Integration/CrossDomain.Integration.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/CrossDomain.Integration/CrossDomain.Integration.sln"
                },
                new AzureCloudService
                {
                    System = fctSystem,
                    Label = "Organisation service",
                    LogicalName = "OrgService",
                    Name = "Organisation service",
                    PublishProjectTfsPath = "$/FCT/Main/OrganisationDomain/OrganisationDomain.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/OrganisationDomain/OrganisationDomain.sln"
                },
                new AzureCloudService
                {
                    System = fctSystem,
                    Label = "Funding Stream Config service",
                    LogicalName = "FundingStreamConfig",
                    Name = "Funding Stream Config service",
                    PublishProjectTfsPath = "$/FCT/Main/FundingStreamConfigDomain/FundingStreamConfigDomain.publish.proj",
                    SolutionTfsPath = "$/FCT/Main/FundingStreamConfigDomain/FundingStreamConfigDomain.sln"
                },
                new SqlAzureDb
                {
                    System = fctSystem,
                    LogicalName = "ServiceBus",
                    Name = "NServiceBus persistency database",
                    Edition = SqlAzureEdition.Business,
                    DTUs = 10,
                    MaximumDatabaseSizeInGB = 10,
                    CollationName = "SQL_Latin1_General_CP1_CI_AS"
                },
                new SqlAzureDb
                {
                    System = fctSystem,
                    LogicalName = "Organisation",
                    Name = "Organisation service persistency database",
                    Edition = SqlAzureEdition.Business,
                    DTUs = 10,
                    MaximumDatabaseSizeInGB = 10,
                    CollationName = "SQL_Latin1_General_CP1_CI_AS"
                });


        }
    }
}
