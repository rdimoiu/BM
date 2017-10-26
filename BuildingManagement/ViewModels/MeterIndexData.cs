using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.ViewModels
{
    public class MeterIndexData
    {
        public IEnumerable<Meter> Meters { get; set; }
        public IEnumerable<MeterType> MeterTypes { get; set; }
        public IEnumerable<Space> Spaces { get; set; }
    }
}