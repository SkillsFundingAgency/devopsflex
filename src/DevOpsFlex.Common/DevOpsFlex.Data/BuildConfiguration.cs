namespace DevOpsFlex.Data
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class BuildConfiguration
    {
        [Key]
        public int Id { get; set; }

        public virtual ICollection<RelComponentExclusion> ComponentExclusions { get; set; }

        public virtual ICollection<RelFirewallRuleExclusion> FirewallRulesExclusions { get; set; }

        [Required, MaxLength(20)]
        public string Configuration { get; set; }

        [Required, MaxLength(20)]
        public string Platform { get; set; }
    }
}
