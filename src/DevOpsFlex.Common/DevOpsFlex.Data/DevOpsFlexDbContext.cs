namespace DevOpsFlex.Data
{
    using System.Data.Entity;
    using Core.Events;

    public class DevOpsFlexDbContext : DbContext
    {
        public DevOpsFlexDbContext()
        {
        }

        public DevOpsFlexDbContext(string connectionString)
            :base(connectionString)
        {
        }

        public DbSet<BuildConfiguration> Configurations { get; set; }

        public DbSet<BuildEvent> BuildEvents { get; set; }

        public DbSet<CodeBranch> Branches { get; set; }

        public DbSet<DevOpsComponent> Components { get; set; }

        public DbSet<DevOpsSystem> Systems { get; set; }

        public DbSet<RelComponentExclusion> ComponentExclusions { get; set; }

        public DbSet<RelFirewallRuleExclusion> FirewallRulesExclusions { get; set; }

        public DbSet<SqlFirewallRule> SqlFirewallRules { get; set; }
    }
}
