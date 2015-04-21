namespace DevOpsFlex.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
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
                        PublishProjectTfsPath = c.String(maxLength: 500),
                        SolutionTfsPath = c.String(maxLength: 500),
                        Region = c.Int(),
                        ExternalAccess = c.Short(),
                        Acl = c.Short(),
                        PublishProjectTfsPath1 = c.String(maxLength: 500),
                        SolutionTfsPath1 = c.String(maxLength: 500),
                        Edition = c.Short(),
                        MaximumDatabaseSizeInGB = c.Int(),
                        CollationName = c.String(maxLength: 200),
                        DTUs = c.Int(),
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
                        WebSpace = c.Int(nullable: false),
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
                        RawExclusions = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DevOpsSystems", t => t.SystemId, cascadeDelete: true)
                .Index(t => t.SystemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DevOpsComponents", "SystemId", "dbo.DevOpsSystems");
            DropForeignKey("dbo.SqlFirewallRules", "SystemId", "dbo.DevOpsSystems");
            DropForeignKey("dbo.DevOpsComponents", "DependantId", "dbo.DevOpsComponents");
            DropIndex("dbo.SqlFirewallRules", new[] { "SystemId" });
            DropIndex("dbo.DevOpsSystems", new[] { "LogicalName" });
            DropIndex("dbo.DevOpsComponents", new[] { "LogicalName" });
            DropIndex("dbo.DevOpsComponents", new[] { "DependantId" });
            DropIndex("dbo.DevOpsComponents", new[] { "SystemId" });
            DropTable("dbo.SqlFirewallRules");
            DropTable("dbo.DevOpsSystems");
            DropTable("dbo.DevOpsComponents");
        }
    }
}
