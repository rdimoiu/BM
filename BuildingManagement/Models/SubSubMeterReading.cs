using System;

namespace BuildingManagement.Models
{
    public class SubSubMeterReading : AbstractMeterReading
    {
        public int SubSubMeterID { get; set; }
        public virtual SubSubMeter SubSubMeter { get; set; }

        public SubSubMeterReading()
        {
            Date = DateTime.Today;
            DiscountMonth = new DateTime();
        }
    }
}