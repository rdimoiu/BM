using System.Collections.Generic;

namespace BuildingManagement.Models
{
    public class Meter : AbstractMeter
    {
        public int ClientID { get; set; }
        public virtual Client Client { get; set; }

        public virtual ICollection<MeterReading> MeterReadings { get; set; }
    }
}