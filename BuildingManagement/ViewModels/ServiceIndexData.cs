using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.ViewModels
{
    public class ServiceIndexData
    {
        public IEnumerable<Service> Services { get; set; }
        public IEnumerable<Space> Spaces { get; set; }
    }
}