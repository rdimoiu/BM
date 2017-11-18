using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingManagement.Models
{
    public class Meter
    {
        public int ID { get; set; }

        public string Code { get; set; }

        public string Details { get; set; }

        [Range(0, 9999999999999999.99)]
        public decimal InitialIndex { get; set; }

        public bool Defect { get; set; }
        //consumption

        public int ClientID { get; set; }
        public virtual Client Client { get; set; }

        public int DistributionModeID { get; set; }
        public virtual DistributionMode DistributionMode { get; set; }

        [NotMapped]
        public List<string> MeterTypesSelected { get; set; }

        public virtual ICollection<MeterType> MeterTypes { get; set; }

        public virtual ICollection<Space> Spaces { get; set; }
        
        public virtual ICollection<Level> Levels { get; set; }
        
        public virtual ICollection<Section> Sections { get; set; }

        [NotMapped]
        public List<string> MeterSLSSelected { get; set; }
    }
}