namespace DevOpsFlex.Data
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CodeBranch
    {
        [Key]
        public int Id { get; set; }

        public virtual ICollection<RelComponentExclusion> Exclusions { get; set; }

        public virtual ICollection<RelFirewallRuleExclusion> FirewallRulesExclusions { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(10)]
        public string SlotPart { get; set; }

        [Required, MaxLength(200)]
        public string TfsPath { get; set; }
    }
}
