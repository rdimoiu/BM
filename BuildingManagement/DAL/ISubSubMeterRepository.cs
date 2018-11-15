using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface ISubSubMeterRepository : IGenericRepository<SubSubMeter>
    {
        SubSubMeter GetSubSubMeterIncludingMeterTypesAndDistributionModeAndSubMeterAndSectionsAndLevelsAndSpaces(int id);
        SubSubMeter GetSubSubMeterIncludingMeterTypes(int id);
        SubSubMeter GetSubSubMeterIncludingSectionsAndLevelsAndSpaces(int id);
        IEnumerable<SubSubMeter> GetAllSubSubMetersIncludingMeterTypesAndDistributionModeAndSubMeterAndSectionsAndLevelsAndSpaces();
        IEnumerable<SubSubMeter> GetFilteredSubSubMetersIncludingMeterTypesAndDistributionModeAndSubMeterAndSectionsAndLevelsAndSpaces(string searchString);
        IEnumerable<SubSubMeter> OrderSubSubMeters(IEnumerable<SubSubMeter> subSubMeters, string sortOrder);
    }
}