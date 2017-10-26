using System.Collections.Generic;
using System.Linq;
using BuildingManagement.Models;

namespace BuildingManagement.Controllers
{
    public static class Utils
    {
        public static Building MapSelectedIDsToDatabaseIDs(List<string> selectedIDs)
        {
            var noDots = new List<string>();
            var oneDot = new List<string>();
            var twoDots = new List<string>();

            //split sections from levels from spaces
            foreach (var item in selectedIDs)
            {
                var dots = item.Count(d => d == '.');
                if (dots == 0)
                {
                    if (!item.Equals("root"))
                    {
                        noDots.Add(item);
                    }
                } else if (dots == 1)
                {
                    oneDot.Add(item);
                } else if (dots == 2)
                {
                    twoDots.Add(item);
                }
            }

            //remove levels and spaces if sections are selected
            for (var i = 0; i < noDots.Count; i++)
            {
                for (var j = 0; j < oneDot.Count; j++)
                {
                    if (oneDot[j].StartsWith(noDots[i]))
                    {
                        oneDot.Remove(oneDot[j]);
                        j--;
                    }
                    else
                    {
                        for (var k = 0; k < twoDots.Count; k++)
                        {
                            if (twoDots[k].StartsWith(noDots[i] + "." + oneDot[j]))
                            {
                                twoDots.Remove(twoDots[k]);
                                k--;
                            }
                        }
                    }
                }
                for (var k = 0; k < twoDots.Count; k++)
                {
                    if (twoDots[k].StartsWith(noDots[i]))
                    {
                        twoDots.Remove(twoDots[k]);
                        k--;
                    }
                }
            }

            //remove spaces if levels are selected
            for (var j = 0; j < oneDot.Count; j++)
            {
                for (var k = 0; k < twoDots.Count; k++)
                {
                    if (twoDots[k].StartsWith(oneDot[j]))
                    {
                        twoDots.Remove(twoDots[k]);
                        k--;
                    }
                }
            }

            var building = new Building();
            foreach (var sectionId in noDots)
            {
                building.SectionIDs.Add(int.Parse(sectionId));
            }
            foreach (var levelId in oneDot)
            {
                string[] parts = levelId.Split('.');
                building.LevelIDs.Add(int.Parse(parts[1]));
            }
            foreach (var spaceId in twoDots)
            {
                string[] parts = spaceId.Split('.');
                building.SpaceIDs.Add(int.Parse(parts[2]));
            }
            
            return building;
        }
    }
}