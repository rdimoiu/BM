using System.Collections.Generic;
using System.Linq;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public class SpaceTypeRepository : GenericRepository<SpaceType>, ISpaceTypeRepository
    {
        public SpaceTypeRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public IEnumerable<SpaceType> GetFilteredSpaceTypes(string searchString)
        {
            return
                MainContext.SpaceTypes.Where(st => st.Type.ToLower().Contains(searchString));
        }

        public IEnumerable<SpaceType> OrderSpaceTypes(IEnumerable<SpaceType> spaceTypes,
            string sortOrder)
        {
            switch (sortOrder)
            {
                case "type_desc":
                    spaceTypes = spaceTypes.OrderByDescending(st => st.Type);
                    break;
                default: // Type ascending 
                    spaceTypes = spaceTypes.OrderBy(st => st.Type);
                    break;
            }
            return spaceTypes;
        }
    }
}