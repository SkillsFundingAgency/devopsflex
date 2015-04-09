namespace DevOpsFlex.Data
{
    using System.Data.Entity;

    public class DevOpsFlexDbContext : DbContext
    {
        public DbSet<DevOpsSystem> Systems { get; set; }

        public DbSet<DevOpsComponent> Components { get; set; }
    }
}
