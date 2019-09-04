using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingManagement.Models
{
    public class Level
    {
        public int ID { get; set; }

        [Required]
        [StringLength(30)]
        public string Number { get; set; }

        [Range(0.00, 9999999999999999.99)]
        public decimal Surface { get; set; }

        public int People { get; set; }

        public int SectionID { get; set; }

        public Section Section { get; set; }

        public virtual ICollection<AbstractMeter> AbstractMeters { get; set; }

        public virtual ICollection<Service> Services { get; set; }

        [NotMapped]
        public int ClientID { get; set; }

        [NotMapped]
        public Client Client { get; set; }
    }
}