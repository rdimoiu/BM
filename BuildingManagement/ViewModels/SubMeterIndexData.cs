using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.ViewModels
{
    public class SubMeterIndexData
    {
        public IEnumerable<SubMeter> SubMeters { get; set; }
        public IEnumerable<MeterType> MeterTypes { get; set; }
        public IEnumerable<Section> Sections { get; set; }
        public IEnumerable<Level> Levels { get; set; }
        public IEnumerable<Space> Spaces { get; set; }
    }
}