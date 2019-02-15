using System.Collections.Generic;
using System.Linq;
using BuildingManagement.Models;
using System.Data.Entity;

namespace BuildingManagement.DAL
{
    public class LevelRepository : GenericRepository<Level>, ILevelRepository
    {
        public LevelRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public Level GetLevelIncludingSection(int id)
        {
            return MainContext.Levels
                .Include(l => l.Section)
                .SingleOrDefault(l => l.ID == id);
        }

        public IEnumerable<Level> GetAllLevelsIncludingSection()
        {
            return MainContext.Levels
                .Include(l => l.Section);
        }

        public IEnumerable<Level> GetFilteredLevelsIncludingSection(string searchString)
        {
            return MainContext.Levels
                .Include(l => l.Section)
                .Where(l =>
                    l.Section.Number.ToLower().Contains(searchString) ||
                    l.Number.ToLower().Contains(searchString) ||
                    l.Surface.ToString().ToLower().Contains(searchString) ||
                    l.People.ToString().ToLower().Contains(searchString));
        }

        public IEnumerable<Level> GetLevelsBySection(int sectionId)
        {
            return MainContext.Levels
                .Where(l => l.SectionID == sectionId);
        }

        public IEnumerable<Level> GetLevelsIncludingServicesBySection(int sectionId)
        {
            return MainContext.Levels
                .Include(l => l.Services)
                .Where(l => l.SectionID == sectionId);
        }

        public IEnumerable<Level> OrderLevels(IEnumerable<Level> levels, string sortOrder)
        {
            switch (sortOrder)
            {
                case "section_desc":
                    levels = levels.OrderByDescending(l => l.Section.Number);
                    break;
                case "Number":
                    levels = levels.OrderBy(l => l.Number);
                    break;
                case "number_desc":
                    levels = levels.OrderByDescending(l => l.Number);
                    break;
                case "Surface":
                    levels = levels.OrderBy(l => l.Surface);
                    break;
                case "surface_desc":
                    levels = levels.OrderByDescending(l => l.Surface);
                    break;
                case "People":
                    levels = levels.OrderBy(l => l.People);
                    break;
                case "people_desc":
                    levels = levels.OrderByDescending(l => l.People);
                    break;
                default: // Section ascending 
                    levels = levels.OrderBy(l => l.Section.Number);
                    break;
            }
            return levels;
        }
    }
}