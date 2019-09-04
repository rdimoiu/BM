using BuildingManagement.Models;
using System.Collections.Generic;

namespace BuildingManagement.DAL
{
    public interface IUncountedCostRepository : IGenericRepository<UncountedCost>
    {
        IEnumerable<UncountedCost> GetCostsByService(int serviceId);
    }
}