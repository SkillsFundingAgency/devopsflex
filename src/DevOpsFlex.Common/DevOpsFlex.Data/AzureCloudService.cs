namespace DevOpsFlex.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Core;
    using Microsoft.WindowsAzure.Management.Compute.Models;

    public class AzureCloudService : DevOpsComponent
    {
        [MaxLength(200)]
        public string Label { get; set; }

        [Required, MaxLength(500)]
        public string PublishProjectTfsPath { get; set; }

        [Required, MaxLength(500)]
        public string SolutionTfsPath { get; set; }

        [NotMapped]
        public HostedServiceCreateParameters AzureParameters
        {
            get
            {
                return new HostedServiceCreateParameters
                {
                    Label = Label,
                    Location = System.Location.GetEnumDescription(),
                    ServiceName = DataConfiguration.GetNaming<AzureCloudService>().GetSlotName(this)
                };
            }
        }
    }
}
