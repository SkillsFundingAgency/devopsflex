namespace DevOpsFlex.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class RelComponentExclusion
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Component")]
        public int ComponentId { get; set; }
        public DevOpsComponent Component { get; set; }

        [Required, ForeignKey("Configuration")]
        public int ConfigurationId { get; set; }
        public BuildConfiguration Configuration { get; set; }

        [ForeignKey("Branch")]
        public int? BranchId { get; set; }
        public CodeBranch Branch { get; set; }
    }
}
