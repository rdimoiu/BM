using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.ViewModels
{
    public class SpaceIndexData
    {
        public IEnumerable<Space> Spaces { get; set; }
        public IEnumerable<Level> Levels { get; set; } 
        public IEnumerable<SpaceType> SpaceTypes { get; set; }
    }
}