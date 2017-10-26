using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingManagement.Models
{
    public class Invoice
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
        public decimal CheckTotalValueWithoutTVA { get; set; }

        [Required]
        public decimal CheckTotalTVA { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }

        [Range(0, 9999999999999999.99)]
        public decimal Quantity { get; set; }

        [Range(0, 9999999999999999.99)]
        public decimal CheckQuantity { get; set; }

        [StringLength(10)]
        public string Unit { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DiscountMonth { get; set; }

        public int InvoiceTypeID { get; set; }
        public virtual InvoiceType InvoiceType { get; set; }

        public int ProviderID { get; set; }
        public virtual Provider Provider { get; set; }

        public int ClientID { get; set; }
        public virtual Client Client { get; set; }

        public virtual ICollection<Service> Services { get; set; }

        [NotMapped]
        public string PreviousPage { get; set; }

        public bool Closed { get; set; }

        public Invoice()
        {
            Date = DateTime.Now;
            DueDate = DateTime.Now;
            DiscountMonth = DateTime.Now;
        }
    }
}