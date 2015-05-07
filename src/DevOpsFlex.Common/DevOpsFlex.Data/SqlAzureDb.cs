namespace DevOpsFlex.Data
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.WindowsAzure.Management.Sql.Models;

    public class SqlAzureDb : DevOpsComponent
    {
        public SqlAzureEdition Edition { get; set; }

        [Range(1, 250)]
        public int MaximumDatabaseSizeInGB { get; set; }

        [Required, MaxLength(200)]
        public string CollationName { get; set; }

        public SqlServiceObjectives ServiceObjective { get; set; }
    }

    public enum SqlAzureEdition : short
    {
        [Description(DatabaseEditions.Basic)]       Basic       = 1,
        [Description(DatabaseEditions.Standard)]    Standard    = 2,
        [Description(DatabaseEditions.Premium)]     Premium     = 3
    }
}
