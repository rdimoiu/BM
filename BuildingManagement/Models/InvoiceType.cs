using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingManagement.Models
{
    public class InvoiceType
    {
        public int ID { get; set; }

        [Required]
        public string Type { get; set; }
    }
}