﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

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

        public int InvoiceID { get; set; }
        public virtual Invoice Invoice { get; set; }

        public int? DistributionModeID { get; set; }
        public DistributionMode DistributionMode { get; set; }

        public virtual ICollection<Space> Spaces { get; set; }

        public virtual ICollection<Level> Levels { get; set; }

        public virtual ICollection<Section> Sections { get; set; }

        [NotMapped]
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