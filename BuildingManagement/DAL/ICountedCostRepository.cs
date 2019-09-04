using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface ICountedCostRepository : IGenericRepository<CountedCost>
    {
        IEnumerable<CountedCost> GetCostsByService(int serviceID);
    }
}