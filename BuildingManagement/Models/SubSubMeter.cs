using System.Collections.Generic;

namespace BuildingManagement.Models
{
    public class SubSubMeter : AbstractMeter
    {
        public int SubMeterID { get; set; }
        public SubMeter SubMeter { get; set; }

        public virtual ICollection<SubSubMeterReading> SubSubMeterReadings { get; set; }
    }
}