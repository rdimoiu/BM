using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface ISubMeterRepository : IGenericRepository<SubMeter>
    {
        SubMeter GetSubMeterIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces(int id);
        SubMeter GetSubMeterIncludingMeterTypes(int id);
        SubMeter GetSubMeterIncludingSectionsAndLevelsAndSpaces(int id);
        IEnumerable<SubMeter> GetAllSubMetersIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces(string sortOrder);
        IEnumerable<SubMeter> GetFilteredSubMetersIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces(string searchString, string sortOrder);
        IEnumerable<SubMeter> OrderSubMeters(IEnumerable<SubMeter> subMeters, string sortOrder);
    }
}