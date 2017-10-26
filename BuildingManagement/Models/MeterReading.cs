using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace BuildingManagement.Models
{
    public class MeterReading
    {
        public int ID { get; set; }

        [Required]
        [Range(0, 9999999999999999.99)]
        public decimal Index { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Range(1, int.MaxValue)]
        public int MeterID { get; set; }
        public virtual Meter Meter { get; set; }

        [Range(1, int.MaxValue)]
        public int MeterTypeID { get; set; }
        public virtual MeterType MeterType { get; set; }
    }
}