using BuildingManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingManagement.ViewModels
{
    public class InvoiceDistributionIndexData
    {
        [NotMapped]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DiscountMonth { get; set; }

        [NotMapped]
        public int ClientID { get; set; }
        [NotMapped]
        public virtual Client Client { get; set; }

        [NotMapped]
        public int ProviderID { get; set; }
        [NotMapped]
        public virtual Provider Provider { get; set; }

        [NotMapped]
        public virtual IEnumerable<Invoice> Invoices { get; set; }

        [NotMapped]
        public virtual IEnumerable<Service> Services { get; set; }
    }
}