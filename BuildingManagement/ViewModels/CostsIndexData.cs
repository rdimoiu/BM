using BuildingManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BuildingManagement.ViewModels
{
    public class CostsIndexData
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
        public int SectionID { get; set; }
        [NotMapped]
        public virtual Section Section { get; set; }

        [NotMapped]
        public List<Space> Spaces { get; set; }

        [NotMapped]
        public virtual IEnumerable<Cost> Costs { get; set; }

        [NotMapped]
        public virtual IEnumerable<Invoice> Invoices { get; set; }

        [NotMapped]
        public List<Service> Services { get; set; }

        [NotMapped]
        public Dictionary<string, Dictionary<string, string>> Rows;

        [NotMapped]
        public Dictionary<string, string> Cols;
    }
}