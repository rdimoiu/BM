using BuildingManagement.Models;
using System.Collections.Generic;

namespace BuildingManagement.DAL
{
    public interface IDistributionModeRepository : IGenericRepository<DistributionMode>
    {
        IEnumerable<DistributionMode> GetFilteredDistributionModes(string searchString);
        IEnumerable<DistributionMode> OrderDistributionModes(IEnumerable<DistributionMode> distributionModes, string sortOrder);
    }
}