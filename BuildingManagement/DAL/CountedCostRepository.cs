using System.Collections.Generic;
using System.Linq;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public class CountedCostRepository : GenericRepository<CountedCost>, ICountedCostRepository
    {
        public CountedCostRepository(MainContext context) : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public IEnumerable<CountedCost> GetCostsByService(int serviceID)
        {
            return MainContext.CountedCosts.Where(cc => cc.ServiceID == serviceID);
        }
    }
}