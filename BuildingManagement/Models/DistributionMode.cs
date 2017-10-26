using System.ComponentModel.DataAnnotations;

namespace BuildingManagement.Models
{
    public class DistributionMode
    {
        public int ID { get; set; }

        [Required]
        [StringLength(15)]
        public string Mode { get; set; }
    }
}