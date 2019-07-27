using BuildingManagement.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingManagement.ViewModels
{
    public class ServiceDistributeData
    {
        [NotMapped]
        public int ServiceID { get; set; }
        [NotMapped]
        public virtual Service Service { get; set; }

        [NotMapped]
        public virtual List<Cost> Costs { get; set; }
    }
}