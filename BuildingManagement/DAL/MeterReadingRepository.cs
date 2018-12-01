using System.Collections.Generic;
using System.Linq;
using BuildingManagement.Models;
using System.Data.Entity;

namespace BuildingManagement.DAL
{
    public class MeterReadingRepository : GenericRepository<MeterReading>, IMeterReadingRepository
    {
        public MeterReadingRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public MeterReading GetMeterReadingIncludingMeterAndMeterType(int id)
        {
            return
                MainContext.MeterReadings.Include(mr => mr.Meter)
                    .Include(mr => mr.MeterType)
                    .SingleOrDefault(mr => mr.ID == id);
        }

        public IEnumerable<MeterReading> GetAllMeterReadingsIncludingMeterAndMeterType()
        {
            return
                MainContext.MeterReadings.Include(mr => mr.Meter).Include(mr => mr.MeterType);
        }

        public IEnumerable<MeterReading> GetFilteredMeterReadingsIncludingMeterAndMeterType(string searchString)
        {
            return
                MainContext.MeterReadings.Include(mr => mr.Meter).Include(mr => mr.MeterType)
                    .Where(
                        mr =>
                            mr.Index.ToString().ToLower().Contains(searchString) ||
                            mr.Date.ToString().ToLower().Contains(searchString) ||
                            mr.Meter.Code.ToLower().Contains(searchString) ||
                            mr.MeterType.Type.ToLower().Contains(searchString));
        }

        public IEnumerable<MeterReading> OrderMeterReadings(IEnumerable<MeterReading> meterReadings, string sortOrder)
        {
            switch (sortOrder)
            {
                case "index_desc":
                    meterReadings = meterReadings.OrderByDescending(mr => mr.Index);
                    break;
                case "Date":
                    meterReadings = meterReadings.OrderBy(mr => mr.Date);
                    break;
                case "date_desc":
                    meterReadings = meterReadings.OrderByDescending(mr => mr.Date);
                    break;
                case "Meter":
                    meterReadings = meterReadings.OrderBy(mr => mr.Meter.Code);
                    break;
                case "meter_desc":
                    meterReadings = meterReadings.OrderByDescending(mr => mr.Meter.Code);
                    break;
                case "MeterType":
                    meterReadings = meterReadings.OrderBy(mr => mr.MeterType.Type);
                    break;
                case "meterType_desc":
                    meterReadings = meterReadings.OrderByDescending(mr => mr.MeterType.Type);
                    break;
                default: // Index ascending 
                    meterReadings = meterReadings.OrderBy(mr => mr.Index);
                    break;
            }
            return meterReadings;
        }
    }
}