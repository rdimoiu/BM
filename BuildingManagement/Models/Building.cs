using System.Collections.Generic;

namespace BuildingManagement.Models
{
    public class Building
    {
        public List<int> SectionIDs { get; set; }

        public List<int> LevelIDs { get; set; }

        public List<int> SpaceIDs { get; set; }

        public Building()
        {
            SectionIDs = new List<int>();
            LevelIDs = new List<int>();
            SpaceIDs = new List<int>();
        }
    }
}