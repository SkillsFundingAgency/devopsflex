namespace DevOpsFlex.Data
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public abstract class DevOpsComponent
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("System")]
        public int SystemId { get; set; }
        public virtual DevOpsSystem System { get; set; }

        [ForeignKey("Dependant")]
        public int? DependantId { get; set; }
        public virtual DevOpsComponent Dependant { get; set; }

        public virtual ICollection<DevOpsComponent> Dependencies { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required, MaxLength(100)]
        public string LogicalName { get; set; }
    }
}
