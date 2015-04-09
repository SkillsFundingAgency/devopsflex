namespace DevOpsFlex.Data
{
    using System.ComponentModel.DataAnnotations;

    public class AzureCloudService : DevOpsComponent
    {
        [MaxLength(200)]
        public string Label { get; set; }

        [Required, MaxLength(500)]
        public string PublishProjectTfsPath { get; set; }

        [Required, MaxLength(500)]
        public string SolutionTfsPath { get; set; }
    }
}
