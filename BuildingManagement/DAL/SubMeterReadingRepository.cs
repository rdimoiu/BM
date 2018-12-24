using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public class SubMeterReadingRepository : GenericRepository<SubMeterReading>, ISubMeterReadingRepository
    {
        public SubMeterReadingRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public SubMeterReading GetSubMeterReadingIncludingSubMeterAndMeterType(int id)
        {
            return MainContext.SubMeterReadings
                .Include(smr => smr.SubMeter)
                .Include(smr => smr.MeterType)
                .SingleOrDefault(smr => smr.ID == id);
        }

        public IEnumerable<SubMeterReading> GetAllSubMeterReadingsIncludingSubMeterAndMeterType()
        {
            return MainContext.SubMeterReadings
                .Include(smr => smr.SubMeter)
                .Include(smr => smr.MeterType);
        }

        public IEnumerable<SubMeterReading> GetFilteredSubMeterReadingsIncludingSubMeterAndMeterType(string searchString)
        {
            return MainContext.SubMeterReadings
                .Include(smr => smr.SubMeter)
                .Include(smr => smr.MeterType)
                .Where(smr =>
                    smr.Index.ToString().ToLower().Contains(searchString) ||
                    smr.Date.ToString().ToLower().Contains(searchString) ||
                    smr.SubMeter.Code.ToLower().Contains(searchString) ||
                    smr.MeterType.Type.ToLower().Contains(searchString));
        }

        public IEnumerable<SubMeterReading> OrderSubMeterReadings(IEnumerable<SubMeterReading> subMeterReadings, string sortOrder)
        {
            switch (sortOrder)
            {
                case "index_desc":
                    subMeterReadings = subMeterReadings.OrderByDescending(smr => smr.Index);
                    break;
                case "Date":
                    subMeterReadings = subMeterReadings.OrderBy(smr => smr.Date);
                    break;
                case "date_desc":
                    subMeterReadings = subMeterReadings.OrderByDescending(smr => smr.Date);
                    break;
                case "Meter":
                    subMeterReadings = subMeterReadings.OrderBy(smr => smr.SubMeter.Code);
                    break;
                case "meter_desc":
                    subMeterReadings = subMeterReadings.OrderByDescending(smr => smr.SubMeter.Code);
                    break;
                case "MeterType":
                    subMeterReadings = subMeterReadings.OrderBy(smr => smr.MeterType.Type);
                    break;
                case "meterType_desc":
                    subMeterReadings = subMeterReadings.OrderByDescending(smr => smr.MeterType.Type);
                    break;
                default: // Index ascending 
                    subMeterReadings = subMeterReadings.OrderBy(smr => smr.Index);
                    break;
            }
            return subMeterReadings;
        }
    }
}