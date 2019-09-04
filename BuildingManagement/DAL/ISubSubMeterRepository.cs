using BuildingManagement.Models;
using System.Collections.Generic;

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
        IEnumerable<int> GetSubSubMeterIDsBySubMeterIDNoDefect(int subMeterID);
        IEnumerable<SubSubMeter> GetAllNoDefect();
    }
}