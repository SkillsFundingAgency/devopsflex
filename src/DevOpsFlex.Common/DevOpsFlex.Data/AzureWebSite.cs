namespace DevOpsFlex.Data
{
    using System.ComponentModel.DataAnnotations;

    public class AzureWebSite : DevOpsComponent
    {
        [Required, MaxLength(500)]
        public string PublishProjectTfsPath { get; set; }

        [Required, MaxLength(500)]
        public string SolutionTfsPath { get; set; }
    }
}
