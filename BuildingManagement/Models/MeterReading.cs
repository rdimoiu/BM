using System;

namespace BuildingManagement.Models
{
    public class MeterReading : AbstractMeterReading
    {
        public int MeterID { get; set; }
        public virtual Meter Meter { get; set; }

        public MeterReading()
        {
            Date = DateTime.Today;
            DiscountMonth = new DateTime();
        }
    }
}