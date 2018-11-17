﻿using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface ILevelRepository : IGenericRepository<Level>
    {
        Level GetLevelIncludingSection(int id);
        IEnumerable<Level> GetAllLevelsIncludingSection();
        IEnumerable<Level> GetFilteredLevelsIncludingSection(string searchString);
        IEnumerable<Level> GetLevelsBySection(int sectionId); 
        IEnumerable<Level> OrderLevels(IEnumerable<Level> levels, string sortOrder);
    }
}