namespace DevOpsFlex.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CodeBranches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        SlotPart = c.String(nullable: false, maxLength: 10),
                        TfsPath = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RelComponentExclusions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ComponentId = c.Int(nullable: false),
                        ConfigurationId = c.Int(nullable: false),
                        BranchId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CodeBranches", t => t.BranchId)
                .ForeignKey("dbo.DevOpsComponents", t => t.ComponentId, cascadeDelete: true)
                .ForeignKey("dbo.BuildConfigurations", t => t.ConfigurationId, cascadeDelete: true)
                .Index(t => t.ComponentId)
                .Index(t => t.ConfigurationId)
                .Index(t => t.BranchId);
            
            CreateTable(
                "dbo.DevOpsComponents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SystemId = c.Int(nullable: false),
                        DependantId = c.Int(),
                        Name = c.String(nullable: false, maxLength: 200),
                        LogicalName = c.String(nullable: false, maxLength: 100),
                        Label = c.String(maxLength: 200),
                        ReserveIp = c.Boolean(),
                        PublishProjectTfsPath = c.String(maxLength: 500),
                        SolutionTfsPath = c.String(maxLength: 500),
                        Region = c.Int(),
                        PublicAccess = c.Int(),
                        Acl = c.Int(),
                        PublishProjectTfsPath1 = c.String(maxLength: 500),
                        SolutionTfsPath1 = c.String(maxLength: 500),
                        Edition = c.Short(),
                        MaximumDatabaseSizeInGB = c.Int(),
                        CollationName = c.String(maxLength: 200),
                        ServiceObjective = c.Int(),
                        CreateAppUser = c.Boolean(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DevOpsComponents", t => t.DependantId)
                .ForeignKey("dbo.DevOpsSystems", t => t.SystemId, cascadeDelete: true)
                .Index(t => t.SystemId)
                .Index(t => t.DependantId)
                .Index(t => t.LogicalName, unique: true);
            
            CreateTable(
                "dbo.DevOpsSystems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                        LogicalName = c.String(nullable: false, maxLength: 3),
                        Location = c.Short(nullable: false),
                        WebSpace = c.Short(nullable: false),
                        StorageType = c.Int(nullable: false),
                        AfinityGroup = c.String(maxLength: 32),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.LogicalName, unique: true);
            
            CreateTable(
                "dbo.SqlFirewallRules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SystemId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        StartIp = c.String(nullable: false, maxLength: 15),
                        EndIp = c.String(nullable: false, maxLength: 15),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DevOpsSystems", t => t.SystemId, cascadeDelete: true)
                .Index(t => t.SystemId);
            
            CreateTable(
                "dbo.RelFirewallRuleExclusions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirewallRuleId = c.Int(nullable: false),
                        ConfigurationId = c.Int(nullable: false),
                        BranchId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CodeBranches", t => t.BranchId)
                .ForeignKey("dbo.BuildConfigurations", t => t.ConfigurationId, cascadeDelete: true)
                .ForeignKey("dbo.SqlFirewallRules", t => t.FirewallRuleId, cascadeDelete: true)
                .Index(t => t.FirewallRuleId)
                .Index(t => t.ConfigurationId)
                .Index(t => t.BranchId);
            
            CreateTable(
                "dbo.BuildConfigurations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Configuration = c.String(nullable: false, maxLength: 20),
                        Platform = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BuildEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Short(nullable: false),
                        Importance = c.Short(nullable: false),
                        Message = c.String(nullable: false, maxLength: 500),
                        ResourceType = c.Int(),
                        ResourceName = c.String(),
                        ResourceType1 = c.Int(),
                        ResourceName1 = c.String(),
                        ResourceType2 = c.Int(),
                        ResourceName2 = c.String(),
                        Account = c.String(maxLength: 100),
                        Container = c.String(maxLength: 100),
                        Key = c.String(maxLength: 100),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RelComponentExclusions", "ConfigurationId", "dbo.BuildConfigurations");
            DropForeignKey("dbo.RelComponentExclusions", "ComponentId", "dbo.DevOpsComponents");
            DropForeignKey("dbo.DevOpsComponents", "SystemId", "dbo.DevOpsSystems");
            DropForeignKey("dbo.SqlFirewallRules", "SystemId", "dbo.DevOpsSystems");
            DropForeignKey("dbo.RelFirewallRuleExclusions", "FirewallRuleId", "dbo.SqlFirewallRules");
            DropForeignKey("dbo.RelFirewallRuleExclusions", "ConfigurationId", "dbo.BuildConfigurations");
            DropForeignKey("dbo.RelFirewallRuleExclusions", "BranchId", "dbo.CodeBranches");
            DropForeignKey("dbo.DevOpsComponents", "DependantId", "dbo.DevOpsComponents");
            DropForeignKey("dbo.RelComponentExclusions", "BranchId", "dbo.CodeBranches");
            DropIndex("dbo.RelFirewallRuleExclusions", new[] { "BranchId" });
            DropIndex("dbo.RelFirewallRuleExclusions", new[] { "ConfigurationId" });
            DropIndex("dbo.RelFirewallRuleExclusions", new[] { "FirewallRuleId" });
            DropIndex("dbo.SqlFirewallRules", new[] { "SystemId" });
            DropIndex("dbo.DevOpsSystems", new[] { "LogicalName" });
            DropIndex("dbo.DevOpsComponents", new[] { "LogicalName" });
            DropIndex("dbo.DevOpsComponents", new[] { "DependantId" });
            DropIndex("dbo.DevOpsComponents", new[] { "SystemId" });
            DropIndex("dbo.RelComponentExclusions", new[] { "BranchId" });
            DropIndex("dbo.RelComponentExclusions", new[] { "ConfigurationId" });
            DropIndex("dbo.RelComponentExclusions", new[] { "ComponentId" });
            DropTable("dbo.BuildEvents");
            DropTable("dbo.BuildConfigurations");
            DropTable("dbo.RelFirewallRuleExclusions");
            DropTable("dbo.SqlFirewallRules");
            DropTable("dbo.DevOpsSystems");
            DropTable("dbo.DevOpsComponents");
            DropTable("dbo.RelComponentExclusions");
            DropTable("dbo.CodeBranches");
        }
    }
}
