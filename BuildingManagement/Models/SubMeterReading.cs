using System;

namespace BuildingManagement.Models
{
    public class SubMeterReading : AbstractMeterReading
    {
        public int SubMeterID { get; set; }
        public virtual SubMeter SubMeter { get; set; }

        public SubMeterReading()
        {
            Date = DateTime.Today;
            DiscountMonth = new DateTime();
        }
    }
}