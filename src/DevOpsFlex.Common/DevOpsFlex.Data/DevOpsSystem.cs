namespace DevOpsFlex.Data
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.WindowsAzure.Management.Models;
    using Microsoft.WindowsAzure.Management.WebSites.Models;

    public class DevOpsSystem
    {
        [Key]
        public int Id { get; set; }

        public virtual ICollection<DevOpsComponent> Components { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required, Index(IsUnique = true), MaxLength(3)]
        public string LogicalName { get; set; }

        public SystemLocation Location { get; set; }

        public SystemWebSpace WebSpace { get; set; }

        [MaxLength(32)]
        public string AfinityGroup { get; set; }
    }

    public enum SystemLocation : short
    {
        [Description(LocationNames.WestEurope)]     WestEurope      = 1,
        [Description(LocationNames.NorthEurope)]    NorthEurope     = 2,
        [Description(LocationNames.EastUS)]         EastUS          = 3,
        [Description(LocationNames.NorthCentralUS)] NorthCentralUS  = 4,
        [Description(LocationNames.SouthCentralUS)] SouthCentralUS  = 5,
        [Description(LocationNames.WestUS)]         WestUS          = 6,
        [Description(LocationNames.EastAsia)]       EastAsia        = 7,
        [Description(LocationNames.SoutheastAsia)]  SoutheastAsia   = 8
    }

    public enum SystemWebSpace
    {
        [Description(WebSpaceNames.WestEuropeWebSpace)]     WestEurope      = 1,
        [Description(WebSpaceNames.NorthEuropeWebSpace)]    NorthEurope     = 2,
        [Description(WebSpaceNames.EastUSWebSpace)]         EastUS          = 3,
        [Description(WebSpaceNames.NorthCentralUSWebSpace)] NorthCentralUS  = 4,
        [Description(WebSpaceNames.WestUSWebSpace)]         WestUS          = 6,
        [Description(WebSpaceNames.EastAsiaWebSpace)]       EastAsia        = 7
    }
}
