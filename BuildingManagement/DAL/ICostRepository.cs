using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface ICostRepository : IGenericRepository<Cost>
    {
        IEnumerable<Cost> GetCostsByService(int serviceId);
        Cost GetCostsByServiceAndSpace(int serviceId, int spaceId);
    }
}