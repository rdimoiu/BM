using System.Collections.Generic;
using System.Linq;
using BuildingManagement.DAL;
using BuildingManagement.Models;

namespace BuildingManagement.Utils
{
    public class TreeHelper
    {
        private readonly IUnitOfWork _unitOfWork;

        public TreeHelper(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public static Tree1D MapSelectedIDsToDatabaseIDsForMeterTypes(List<string> selectedIDs)
        {
            var noDots = new List<string>();
            if (selectedIDs != null)
            {
                foreach (var item in selectedIDs)
                {
                    var dots = item.Count(d => d == '.');
                    if (dots == 0)
                    {
                        if (!item.Equals("root"))
                        {
                            noDots.Add(item);
                        }
                    }
                }
            }

            var tree1D = new Tree1D();
            foreach (var sectionId in noDots)
            {
                tree1D.MeterTypeIDs.Add(int.Parse(sectionId));
            }

            return tree1D;
        }

        public static Tree3D MapSelectedIDsToDatabaseIDsForSpaces(List<string> selectedIDs)
        {
            var noDots = new List<string>();
            var oneDot = new List<string>();
            var twoDots = new List<string>();

            //split sections from levels from spaces
            if (selectedIDs != null)
            {
                foreach (var item in selectedIDs)
                {
                    var dots = item.Count(d => d == '.');
                    if (dots == 0)
                    {
                        if (!item.Equals("root"))
                        {
                            noDots.Add(item);
                        }
                    }
                    else if (dots == 1)
                    {
                        oneDot.Add(item);
                    }
                    else if (dots == 2)
                    {
                        twoDots.Add(item);
                    }
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

            var tree3D = new Tree3D();
            foreach (var sectionId in noDots)
            {
                tree3D.SectionIDs.Add(int.Parse(sectionId));
            }
            foreach (var levelId in oneDot)
            {
                string[] parts = levelId.Split('.');
                tree3D.LevelIDs.Add(int.Parse(parts[1]));
            }
            foreach (var spaceId in twoDots)
            {
                string[] parts = spaceId.Split('.');
                tree3D.SpaceIDs.Add(int.Parse(parts[2]));
            }

            return tree3D;
        }

        public TreeNode GetSectionsLevelsSpacesByClient(TreeNode root, int clientID, HashSet<int> selectedSectionsIDs, HashSet<int> selectedLevelsIDs, HashSet<int> selectedSpacesIDs, bool disabled)
        {
            var sections = _unitOfWork.SectionRepository.GetSectionsByClient(clientID).ToList();
            if (sections.Any())
            {
                foreach (var section in sections)
                {
                    var sectionNode = new TreeNode();
                    sectionNode.id = section.ID.ToString();
                    sectionNode.text = section.Number;
                    sectionNode.state = new TreeNodeState();
                    sectionNode.state.opened = true;
                    sectionNode.state.disabled = disabled;
                    if (selectedSectionsIDs.Count > 0 && selectedSectionsIDs.Contains(section.ID))
                    {
                        sectionNode.state.selected = true;
                    }
                    root.children.Add(sectionNode);
                    var levels = _unitOfWork.LevelRepository.GetLevelsBySection(section.ID).ToList();
                    if (levels.Any())
                    {
                        foreach (var level in levels)
                        {
                            var levelNode = new TreeNode();
                            levelNode.id = section.ID + "." + level.ID;
                            levelNode.text = level.Number;
                            levelNode.state = new TreeNodeState();
                            levelNode.state.opened = true;
                            levelNode.state.disabled = disabled;
                            if (selectedLevelsIDs.Count > 0 && selectedLevelsIDs.Contains(level.ID))
                            {
                                levelNode.state.selected = true;
                            }
                            sectionNode.children.Add(levelNode);
                            var spaces = _unitOfWork.SpaceRepository.GetSpacesByLevel(level.ID).ToList();
                            if (spaces.Any())
                            {
                                foreach (var space in spaces)
                                {
                                    var spaceNode = new TreeNode();
                                    spaceNode.id = section.ID + "." + level.ID + "." + space.ID;
                                    spaceNode.text = space.Number;
                                    spaceNode.state = new TreeNodeState();
                                    spaceNode.state.disabled = disabled;
                                    if (selectedSpacesIDs.Count > 0 && selectedSpacesIDs.Contains(space.ID))
                                    {
                                        spaceNode.state.selected = true;
                                    }
                                    levelNode.children.Add(spaceNode);
                                }
                            }
                        }
                    }
                }
            }
            return root;
        }

        public TreeNode GetSectionsLevelsSpacesByParent(TreeNode root, int clientID, ICollection<Section> parentSections, ICollection<Level> parentLevels, ICollection<Space> parentSpaces, HashSet<int> selectedSectionsIDs, HashSet<int> selectedLevelsIDs, HashSet<int> selectedSpacesIDs)
        {
            var allSections = _unitOfWork.SectionRepository.GetSectionsByClient(clientID).ToList();
            if (allSections.Any())
            {
                foreach (var section in allSections)
                {
                    var sectionNode = new TreeNode();
                    sectionNode.id = section.ID.ToString();
                    sectionNode.text = section.Number;
                    sectionNode.state = new TreeNodeState();
                    sectionNode.state.opened = true;
                    sectionNode.state.disabled = true;
                    foreach (var parentSection in parentSections)
                    {
                        if (parentSection.ID == section.ID)
                        {
                            sectionNode.state.disabled = false;
                        }
                    }
                    if (selectedSectionsIDs.Count > 0 && selectedSectionsIDs.Contains(section.ID) && !sectionNode.state.disabled)
                    {
                        sectionNode.state.selected = true;
                    }
                    root.children.Add(sectionNode);
                    var allLevels = _unitOfWork.LevelRepository.GetLevelsBySection(section.ID).ToList();
                    if (allLevels.Any())
                    {
                        foreach (var level in allLevels)
                        {
                            var levelNode = new TreeNode();
                            levelNode.id = section.ID + "." + level.ID;
                            levelNode.text = level.Number;
                            levelNode.state = new TreeNodeState();
                            levelNode.state.opened = true;
                            levelNode.state.disabled = sectionNode.state.disabled;
                            foreach (var parentLevel in parentLevels)
                            {
                                if (parentLevel.ID == level.ID)
                                {
                                    levelNode.state.disabled = false;
                                }
                            }
                            if (selectedLevelsIDs.Count > 0 && selectedLevelsIDs.Contains(level.ID) && !levelNode.state.disabled)
                            {
                                levelNode.state.selected = true;
                            }
                            sectionNode.children.Add(levelNode);
                            var allSpaces = _unitOfWork.SpaceRepository.GetSpacesByLevel(level.ID).ToList();
                            if (allSpaces.Any())
                            {
                                foreach (var space in allSpaces)
                                {
                                    var spaceNode = new TreeNode();
                                    spaceNode.id = section.ID + "." + level.ID + "." + space.ID;
                                    spaceNode.text = space.Number;
                                    spaceNode.state = new TreeNodeState();
                                    spaceNode.state.disabled = levelNode.state.disabled;
                                    foreach (var parentSpace in parentSpaces)
                                    {
                                        if (parentSpace.ID == space.ID)
                                        {
                                            spaceNode.state.disabled = false;
                                        }
                                    }
                                    if (selectedSpacesIDs.Count > 0 && selectedSpacesIDs.Contains(space.ID) && !spaceNode.state.disabled)
                                    {
                                        spaceNode.state.selected = true;
                                    }
                                    levelNode.children.Add(spaceNode);
                                }
                            }
                        }
                    }
                }
            }
            return root;
        }
    }
}