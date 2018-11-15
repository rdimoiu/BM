using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface IMeterTypeRepository : IGenericRepository<MeterType>
    {
        IEnumerable<MeterType> GetFilteredMeterTypes(string searchString);
        IEnumerable<MeterType> OrderMeterTypes(IEnumerable<MeterType> meterTypes, string sortOrder);
    }
}