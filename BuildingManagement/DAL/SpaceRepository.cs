using System.Collections.Generic;
using System.Linq;
using BuildingManagement.Models;
using Z.EntityFramework.Plus;

namespace BuildingManagement.DAL
{
    public class SpaceRepository : GenericRepository<Space>, ISpaceRepository
    {
        public SpaceRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public Space GetSpaceIncludingLevel(int id)
        {
            return MainContext.Spaces.IncludeOptimized(s => s.Level).SingleOrDefault(s => s.ID == id);
        }

        public IEnumerable<Space> GetAllSpacesIncludingLevelAndSpaceTypeAndSubClient(string sortOrder)
        {
            var spaces = MainContext.Spaces.IncludeOptimized(s => s.Level)
                .IncludeOptimized(s => s.SpaceType)
                .IncludeOptimized(s => s.SubClient);
            switch (sortOrder)
            {
                case "number_desc":
                    spaces = spaces.OrderByDescending(s => s.Number);
                    break;
                case "Surface":
                    spaces = spaces.OrderBy(s => s.Surface);
                    break;
                case "surface_desc":
                    spaces = spaces.OrderByDescending(s => s.Surface);
                    break;
                case "People":
                    spaces = spaces.OrderBy(s => s.People);
                    break;
                case "people_desc":
                    spaces = spaces.OrderByDescending(s => s.People);
                    break;
                case "Inhabited":
                    spaces = spaces.OrderBy(s => s.Inhabited);
                    break;
                case "inhabited_desc":
                    spaces = spaces.OrderByDescending(s => s.Inhabited);
                    break;
                case "Level":
                    spaces = spaces.OrderBy(s => s.Level.Number);
                    break;
                case "level_desc":
                    spaces = spaces.OrderByDescending(s => s.Level.Number);
                    break;
                case "SpaceType":
                    spaces = spaces.OrderBy(s => s.SpaceType.Type);
                    break;
                case "spaceType_desc":
                    spaces = spaces.OrderByDescending(s => s.SpaceType.Type);
                    break;
                case "SubClient":
                    spaces = spaces.OrderBy(s => s.SubClient.Name);
                    break;
                case "subClient_desc":
                    spaces = spaces.OrderByDescending(s => s.SubClient.Name);
                    break;
                default: // Number ascending 
                    spaces = spaces.OrderBy(s => s.Number);
                    break;
            }
            return spaces;
        }

        public IEnumerable<Space> GetFilteredSpacesIncludingLevelAndSpaceTypeAndSubClient(string searchString, string sortOrder)
        {
            var spaces = MainContext.Spaces.IncludeOptimized(s => s.Level)
                .IncludeOptimized(s => s.SpaceType)
                .IncludeOptimized(s => s.SubClient)
                    .Where(
                        s =>
                            s.Number.ToLower().Contains(searchString) ||
                            s.Surface.ToString().ToLower().Contains(searchString) ||
                            s.People.ToString().ToLower().Contains(searchString) ||
                            s.Inhabited.ToString().ToLower().Contains(searchString) ||
                            s.Level.Number.ToLower().Contains(searchString) ||
                            s.SpaceType.Type.ToLower().Contains(searchString) ||
                            s.SubClient.Name.ToLower().Contains(searchString));
            switch (sortOrder)
            {
                case "number_desc":
                    spaces = spaces.OrderByDescending(s => s.Number);
                    break;
                case "Surface":
                    spaces = spaces.OrderBy(s => s.Surface);
                    break;
                case "surface_desc":
                    spaces = spaces.OrderByDescending(s => s.Surface);
                    break;
                case "People":
                    spaces = spaces.OrderBy(s => s.People);
                    break;
                case "people_desc":
                    spaces = spaces.OrderByDescending(s => s.People);
                    break;
                case "Inhabited":
                    spaces = spaces.OrderBy(s => s.Inhabited);
                    break;
                case "inhabited_desc":
                    spaces = spaces.OrderByDescending(s => s.Inhabited);
                    break;
                case "Level":
                    spaces = spaces.OrderBy(s => s.Level.Number);
                    break;
                case "level_desc":
                    spaces = spaces.OrderByDescending(s => s.Level.Number);
                    break;
                case "SpaceType":
                    spaces = spaces.OrderBy(s => s.SpaceType.Type);
                    break;
                case "spaceType_desc":
                    spaces = spaces.OrderByDescending(s => s.SpaceType.Type);
                    break;
                case "SubClient":
                    spaces = spaces.OrderBy(s => s.SubClient.Name);
                    break;
                case "subClient_desc":
                    spaces = spaces.OrderByDescending(s => s.SubClient.Name);
                    break;
                default: // Number ascending 
                    spaces = spaces.OrderBy(s => s.Number);
                    break;
            }
            return spaces;
        }

        public IEnumerable<Space> GetSpacesByLevel(int levelId)
        {
            return MainContext.Spaces.Where(s => s.LevelID == levelId);
        } 

        // to be deleted
        public IEnumerable<Space> OrderSpaces(IEnumerable<Space> spaces, string sortOrder)
        {
            switch (sortOrder)
            {
                case "number_desc":
                    spaces = spaces.OrderByDescending(s => s.Number);
                    break;
                case "Surface":
                    spaces = spaces.OrderBy(s => s.Surface);
                    break;
                case "surface_desc":
                    spaces = spaces.OrderByDescending(s => s.Surface);
                    break;
                case "People":
                    spaces = spaces.OrderBy(s => s.People);
                    break;
                case "people_desc":
                    spaces = spaces.OrderByDescending(s => s.People);
                    break;
                case "Inhabited":
                    spaces = spaces.OrderBy(s => s.Inhabited);
                    break;
                case "inhabited_desc":
                    spaces = spaces.OrderByDescending(s => s.Inhabited);
                    break;
                case "Level":
                    spaces = spaces.OrderBy(s => s.Level.Number);
                    break;
                case "level_desc":
                    spaces = spaces.OrderByDescending(s => s.Level.Number);
                    break;
                case "SpaceType":
                    spaces = spaces.OrderBy(s => s.SpaceType.Type);
                    break;
                case "spaceType_desc":
                    spaces = spaces.OrderByDescending(s => s.SpaceType.Type);
                    break;
                case "SubClient":
                    spaces = spaces.OrderBy(s => s.SubClient.Name);
                    break;
                case "subClient_desc":
                    spaces = spaces.OrderByDescending(s => s.SubClient.Name);
                    break;
                default: // Number ascending 
                    spaces = spaces.OrderBy(s => s.Number);
                    break;
            }
            return spaces;
        }
    }
}