using BuildingManagement.Models;
using System.Collections.Generic;

namespace BuildingManagement.DAL
{
    public interface ISpaceTypeRepository : IGenericRepository<SpaceType>
    {
        IEnumerable<SpaceType> GetFilteredSpaceTypes(string searchString);
        IEnumerable<SpaceType> OrderSpaceTypes(IEnumerable<SpaceType> spaceTypes, string sortOrder);
    }
}