using System.ComponentModel.DataAnnotations;

namespace BuildingManagement.Models
{
    public class SpaceType
    {
        public int ID { get; set; }

        [Required]
        public string Type { get; set; }
    }
}