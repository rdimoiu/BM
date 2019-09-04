using BuildingManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace BuildingManagement.DAL
{
    public class UncountedCostRepository : GenericRepository<UncountedCost>, IUncountedCostRepository
    {
        public UncountedCostRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public IEnumerable<UncountedCost> GetCostsByService(int serviceId)
        {
            return MainContext.UncountedCosts.Where(uc => uc.ServiceID == serviceId);
        }
    }
}