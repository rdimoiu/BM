using System.ComponentModel.DataAnnotations;

namespace BuildingManagement.Models
{
    public class CountedCost
    {
        [Required]
        [Range(0.00, 9999999999999999.99)]
        public decimal Value { get; set; }

        [Required]
        [Range(0.00, 1.00)]
        public decimal Quota { get; set; }

        [Required]
        public decimal Consumption { get; set; }

        [Required]
        public int ServiceID { get; set; }
        public Service Service { get; set; }

        [Required]
        public int SpaceID { get; set; }
        public Space Space { get; set; }

        [Required]
        public int AbstractMeterID { get; set; }
        public virtual AbstractMeter AbstractMeter { get; set; }
    }
}