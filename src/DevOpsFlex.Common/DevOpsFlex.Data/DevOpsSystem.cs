namespace DevOpsFlex.Data
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.WindowsAzure.Management.Models;

    public class DevOpsSystem
    {
        [Key]
        public int Id { get; set; }

        public virtual ICollection<DevOpsComponent> Components { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required, MaxLength(3)]
        public string LogicalName { get; set; }

        public SystemLocation Location { get; set; }

        [MaxLength(32)]
        public string AfinityGroup { get; set; }
    }

    public enum SystemLocation : short
    {
        [Description(LocationNames.WestEurope)]     WestEurope  = 1,
        [Description(LocationNames.NorthEurope)]    NorthEurope = 2
    }
}
