using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingManagement.Models
{
    public class Meter
    {
        public int ID { get; set; }

        public string Code { get; set; }

        public string Details { get; set; }

        public bool Defect { get; set; }

        public int DistributionModeID { get; set; }
        public virtual DistributionMode DistributionMode { get; set; }

        public int ClientID { get; set; }
        public virtual Client Client { get; set; }

        public virtual ICollection<MeterType> MeterTypes { get; set; }

        public virtual ICollection<Space> Spaces { get; set; }

        public virtual ICollection<Level> Levels { get; set; }

        public virtual ICollection<Section> Sections { get; set; }

        [NotMapped]
        public List<string> MeterTypesSelected { get; set; }

        [NotMapped]
        public List<string> MeterSLSSelected { get; set; }
    }
}