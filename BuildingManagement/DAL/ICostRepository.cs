using BuildingManagement.Models;
using System.Collections.Generic;

namespace BuildingManagement.DAL
{
    public interface ICostRepository : IGenericRepository<Cost>
    {
        IEnumerable<Cost> GetCostsByService(int serviceId);
    }
}