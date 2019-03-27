using System;
using System.ComponentModel.DataAnnotations;

namespace BuildingManagement.Models
{
    public class SubMeterReading
    {
        public int ID { get; set; }

        [Required]
        [Range(0, 9999999999999999.99)]
        public decimal Index { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DiscountMonth { get; set; }

        [Range(1, int.MaxValue)]
        public int SubMeterID { get; set; }
        public virtual SubMeter SubMeter { get; set; }

        [Range(1, int.MaxValue)]
        public int MeterTypeID { get; set; }
        public virtual MeterType MeterType { get; set; }

        public bool Initial { get; set; }

        public bool Estimated { get; set; }

        public SubMeterReading()
        {
            Date = DateTime.Today;
            DiscountMonth = new DateTime();
        }
    }
}