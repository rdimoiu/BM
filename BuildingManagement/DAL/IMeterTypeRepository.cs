using BuildingManagement.Models;
using System.Collections.Generic;

namespace BuildingManagement.DAL
{
    public interface IMeterTypeRepository : IGenericRepository<MeterType>
    {
        IEnumerable<MeterType> GetFilteredMeterTypes(string searchString);
        IEnumerable<MeterType> OrderMeterTypes(IEnumerable<MeterType> meterTypes, string sortOrder);
    }
}