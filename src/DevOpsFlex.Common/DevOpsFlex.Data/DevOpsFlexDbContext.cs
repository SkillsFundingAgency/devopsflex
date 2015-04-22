﻿namespace DevOpsFlex.Data
{
    using System.Data.Entity;

    public class DevOpsFlexDbContext : DbContext
    {
        public DbSet<BuildConfiguration> Configurations { get; set; }

        public DbSet<CodeBranch> Branches { get; set; }

        public DbSet<RelComponentExclusion> ComponentExclusions { get; set; }

        public DbSet<RelFirewallRuleExclusion> FirewallRulesExclusions { get; set; }

        public DbSet<DevOpsComponent> Components { get; set; }

        public DbSet<SqlFirewallRule> SqlFirewallRules { get; set; }

        public DbSet<DevOpsSystem> Systems { get; set; }
    }
}
