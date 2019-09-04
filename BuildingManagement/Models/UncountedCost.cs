using System.ComponentModel.DataAnnotations;

namespace BuildingManagement.Models
{
    public class UncountedCost
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
        public virtual Service Service { get; set; }

        [Required]
        public int SpaceID { get; set; }
        public virtual Space Space { get; set; }
    }
}