using System.Collections.Generic;
using System.Linq;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public class DistributionModeRepository : GenericRepository<DistributionMode>, IDistributionModeRepository
    {
        public DistributionModeRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public IEnumerable<DistributionMode> GetFilteredDistributionModes(string searchString)
        {
            return MainContext.DistributionModes
                .Where(dm => dm.Mode.ToLower().Contains(searchString));
        }

        public IEnumerable<DistributionMode> OrderDistributionModes(IEnumerable<DistributionMode> distributionModes, string sortOrder)
        {
            switch (sortOrder)
            {
                case "mode_desc":
                    distributionModes = distributionModes.OrderByDescending(dm => dm.Mode);
                    break;
                default: // Mode ascending 
                    distributionModes = distributionModes.OrderBy(dm => dm.Mode);
                    break;
            }
            return distributionModes;
        }
    }
}