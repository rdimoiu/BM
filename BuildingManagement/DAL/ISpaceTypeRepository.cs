using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface ISpaceTypeRepository : IGenericRepository<SpaceType>
    {
        IEnumerable<SpaceType> GetFilteredSpaceTypes(string searchString);
        IEnumerable<SpaceType> OrderSpaceTypes(IEnumerable<SpaceType> spaceTypes, string sortOrder);
    }
}