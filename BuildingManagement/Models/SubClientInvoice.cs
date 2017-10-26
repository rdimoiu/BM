using System;
using System.ComponentModel.DataAnnotations;

namespace BuildingManagement.Models
{
    public class SubClientInvoice
    {
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string Number { get; set; }

        [Required]
        public decimal TotalValueWithoutTVA { get; set; }

        [Required]
        public decimal TotalTVA { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }

        public int ProviderID { get; set; }
        public virtual Provider Provider { get; set; }

        public int SubClientID { get; set; }
        public virtual SubClient SubClient { get; set; }

        public int InvoiceID { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}