using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingManagement.Models
{
    public class MeterType
    {
        public int ID { get; set; }

        [Required]
        public string Type { get; set; }

        public virtual ICollection<Meter> Meters { get; set; }

        public virtual ICollection<SubMeter> SubMeters { get; set; }

        public virtual ICollection<SubSubMeter> SubSubMeters { get; set; }
    }
}