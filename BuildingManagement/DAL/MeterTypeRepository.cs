using System.Collections.Generic;
using System.Linq;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public class MeterTypeRepository : GenericRepository<MeterType>, IMeterTypeRepository
    {
        public MeterTypeRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public IEnumerable<MeterType> GetFilteredMeterTypes(string searchString)
        {
            return MainContext.MeterTypes
                .Where(mt => mt.Type.ToLower().Contains(searchString));
        }

        public IEnumerable<MeterType> OrderMeterTypes(IEnumerable<MeterType> meterTypes, string sortOrder)
        {
            switch (sortOrder)
            {
                case "type_desc":
                    meterTypes = meterTypes.OrderByDescending(mt => mt.Type);
                    break;
                default: // Type ascending 
                    meterTypes = meterTypes.OrderBy(mt => mt.Type);
                    break;
            }
            return meterTypes;
        }
    }
}