using System;
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
                    smr.DiscountMonth.ToString().ToLower().Contains(searchString) ||
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
                case "DiscountMonth":
                    subMeterReadings = subMeterReadings.OrderBy(smr => smr.DiscountMonth);
                    break;
                case "discountMonth_desc":
                    subMeterReadings = subMeterReadings.OrderByDescending(smr => smr.DiscountMonth);
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

        public SubMeterReading GetSubMeterReadingByDiscountMonth(int subMeterID, int meterTypeID, DateTime discountMonth)
        {
            var firstDayOfMonth = new DateTime(discountMonth.Year, discountMonth.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            return MainContext.SubMeterReadings.OrderByDescending(smr => smr.DiscountMonth).FirstOrDefault(smr => smr.SubMeterID == subMeterID && smr.MeterTypeID == meterTypeID && smr.DiscountMonth >= firstDayOfMonth && smr.DiscountMonth <= lastDayOfMonth);
        }

        public SubMeterReading GetPreviousSubMeterReading(int subMeterID, int meterTypeID, DateTime discountMonth)
        {
            var firstDayOfMonth = new DateTime(discountMonth.Year, discountMonth.Month, 1);
            return MainContext.SubMeterReadings.OrderByDescending(smr => smr.DiscountMonth).FirstOrDefault(smr => smr.SubMeterID == subMeterID && smr.MeterTypeID == meterTypeID && smr.DiscountMonth < firstDayOfMonth);
        }

        public SubMeterReading GetInitialSubMeterReading(int subMeterID, int meterTypeID)
        {
            return MainContext.SubMeterReadings.FirstOrDefault(smr => smr.SubMeterID == subMeterID && smr.MeterTypeID == meterTypeID && smr.Initial);
        }
    }
}