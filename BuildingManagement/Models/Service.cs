using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingManagement.Models
{
    public class Service
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [Range(0, 9999999999999999.99)]
        public decimal Quantity { get; set; }

        [Required]
        [StringLength(10)]
        public string Unit { get; set; }

        [Required]
        [Range(0, 9999999999999999.99)]
        public decimal Price { get; set; }

        [Required]
        public decimal QuotaTVA { get; set; }

        public bool Fixed { get; set; }

        public bool Counted { get; set; }

        public bool Inhabited { get; set; }

        public bool Distributed { get; set; }

        public bool Rest { get; set; }

        public int InvoiceID { get; set; }
        public virtual Invoice Invoice { get; set; }

        public int? DistributionModeID { get; set; }

        public int? MeterTypeID { get; set; }
        public MeterType MeterType { get; set; }

        public virtual ICollection<Space> Spaces { get; set; }

        public virtual ICollection<Level> Levels { get; set; }

        public virtual ICollection<Section> Sections { get; set; }

        public virtual ICollection<Cost> Costs { get; set; }

        [NotMapped]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal ValueWithoutTVA
        {
            get
            {
                return Quantity * Price;
            }
        }

        [NotMapped]
        public decimal TVA
        {
            get
            {
                return ValueWithoutTVA * QuotaTVA;
            }
        }

        [NotMapped]
        public List<string> ServiceSLSSelected { get; set; }

        [NotMapped]
        public string PreviousPage { get; set; }

        public Service()
        {
            Inhabited = true;
        }
    }
}