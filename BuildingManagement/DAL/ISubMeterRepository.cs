using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface ISubMeterRepository : IGenericRepository<SubMeter>
    {
        SubMeter GetSubMeterIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces(int id);
        SubMeter GetSubMeterIncludingMeterTypes(int id);
        SubMeter GetSubMeterIncludingSectionsAndLevelsAndSpaces(int id);
        IEnumerable<SubMeter> GetAllSubMetersIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces();
        IEnumerable<SubMeter> GetFilteredSubMetersIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces(string searchString);
        IEnumerable<SubMeter> OrderSubMeters(IEnumerable<SubMeter> subMeters, string sortOrder);
    }
}