using System.ComponentModel.DataAnnotations;

namespace BuildingManagement.Models
{
    public class Cost
    {
        public int ID { get; set; }

        [Required]
        [Range(0, 9999999999999999.99)]
        public decimal Value { get; set; }

        public int ServiceID { get; set; }
        public virtual Service Service { get; set; }

        public int SpaceID { get; set; }
        public virtual Space Space { get; set; }
    }
}