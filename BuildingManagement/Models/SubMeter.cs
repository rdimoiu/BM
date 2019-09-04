using System.Collections.Generic;

namespace BuildingManagement.Models
{
    public class SubMeter : AbstractMeter
    {
        public int MeterID { get; set; }
        public Meter Meter { get; set; }

        public virtual ICollection<SubMeterReading> SubMeterReadings { get; set; }
    }
}