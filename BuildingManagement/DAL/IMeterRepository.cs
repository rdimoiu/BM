using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface IMeterRepository : IGenericRepository<Meter>
    {
        Meter GetMeterIncludingMeterTypesAndDistributionModeAndClientAndSectionsAndLevelsAndSpaces(int id);
        Meter GetMeterIncludingMeterTypes(int id);
        Meter GetMeterIncludingSectionsAndLevelsAndSpaces(int id);
        IEnumerable<Meter> GetAllMetersIncludingMeterTypesAndDistributionModeAndClientAndSectionsAndLevelsAndSpaces();
        IEnumerable<Meter> GetFilteredMetersIncludingMeterTypesAndDistributionModeAndClientAndSectionsAndLevelsAndSpaces(string searchString);
        IEnumerable<Meter> OrderMeters(IEnumerable<Meter> meters, string sortOrder);
    }
}