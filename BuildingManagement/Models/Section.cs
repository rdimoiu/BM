using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingManagement.Models
{
    public class Section
    {
        public int ID { get; set; }

        [Required]
        [StringLength(30)]
        public string Number { get; set; }

        [Range(0, 9999999999999999.99)]
        public decimal Surface { get; set; }

        public int People { get; set; }

        public int ClientID { get; set; }
        
        public virtual Client Client { get; set; }

        public virtual ICollection<Meter> Meters { get; set; }

        public virtual ICollection<SubMeter> SubMeters { get; set; }

        public virtual ICollection<SubSubMeter> SubSubMeters { get; set; }

        public virtual ICollection<Service> Services { get; set; }
    }
}