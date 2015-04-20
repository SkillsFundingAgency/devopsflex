namespace DevOpsFlex.Data
{
    using System.Data.Entity;

    public class DevOpsFlexDbContext : DbContext
    {
        public DbSet<DevOpsComponent> Components { get; set; }

        public DbSet<SqlFirewallRule> SqlFirewallRules { get; set; }

        public DbSet<DevOpsSystem> Systems { get; set; }
    }
}
