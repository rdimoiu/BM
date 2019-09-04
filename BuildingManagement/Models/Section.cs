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

        [Range(0.00, 9999999999999999.99)]
        public decimal Surface { get; set; }

        public int People { get; set; }

        public int ClientID { get; set; }

        public virtual Client Client { get; set; }

        public virtual ICollection<AbstractMeter> AbstractMeters { get; set; }

        public virtual ICollection<Service> Services { get; set; }
    }
}