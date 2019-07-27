using BuildingManagement.Models;
using System.Collections.Generic;

namespace BuildingManagement.DAL
{
    public interface ISpaceRepository : IGenericRepository<Space>
    {
        Space GetSpaceIncludingLevel(int id);
        IEnumerable<Space> GetAllSpacesIncludingLevelAndSpaceTypeAndSubClient(string sortOrder);
        IEnumerable<Space> GetFilteredSpacesIncludingLevelAndSpaceTypeAndSubClient(string searchString, string sortOrder);
        IEnumerable<Space> GetSpacesByLevel(int levelId);
        IEnumerable<Space> GetSpacesIncludingServicesByLevel(int levelId);
        IEnumerable<Space> OrderSpaces(IEnumerable<Space> spaces, string sortOrder);
    }
}