using System.Collections.Generic;

namespace BuildingManagement.Models
{
    public class Tree3D
    {
        public List<int> SectionIDs { get; set; }

        public List<int> LevelIDs { get; set; }

        public List<int> SpaceIDs { get; set; }

        public Tree3D()
        {
            SectionIDs = new List<int>();
            LevelIDs = new List<int>();
            SpaceIDs = new List<int>();
        }
    }
}