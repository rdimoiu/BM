using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface IDistributionModeRepository : IGenericRepository<DistributionMode>
    {
        IEnumerable<DistributionMode> GetFilteredDistributionModes(string searchString);
        IEnumerable<DistributionMode> OrderDistributionModes(IEnumerable<DistributionMode> distributionModes, string sortOrder);
    }
}