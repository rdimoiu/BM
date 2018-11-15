using System.Collections.Generic;

namespace BuildingManagement.Models
{
    public class Tree1D
    {
        public List<int> MeterTypeIDs{ get; set; }

        public Tree1D()
        {
            MeterTypeIDs = new List<int>();
        }
    }
}