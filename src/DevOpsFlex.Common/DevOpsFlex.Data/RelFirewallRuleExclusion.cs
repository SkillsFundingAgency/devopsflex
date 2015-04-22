namespace DevOpsFlex.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class RelFirewallRuleExclusion
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("FirewallRule")]
        public int FirewallRuleId { get; set; }
        public SqlFirewallRule FirewallRule { get; set; }

        [Required, ForeignKey("Configuration")]
        public int ConfigurationId { get; set; }
        public BuildConfiguration Configuration { get; set; }

        [ForeignKey("Branch")]
        public int? BranchId { get; set; }
        public CodeBranch Branch { get; set; }
    }
}
