using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public class SubSubMeterReadingRepository : GenericRepository<SubSubMeterReading>, ISubSubMeterReadingRepository
    {
        public SubSubMeterReadingRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public SubSubMeterReading GetSubSubMeterReadingIncludingSubSubMeterAndMeterType(int id)
        {
            return MainContext.SubSubMeterReadings
                .Include(ssmr => ssmr.SubSubMeter)
                .Include(ssmr => ssmr.MeterType)
                .SingleOrDefault(ssmr => ssmr.ID == id);
        }

        public IEnumerable<SubSubMeterReading> GetAllSubSubMeterReadingsIncludingSubSubMeterAndMeterType()
        {
            return MainContext.SubSubMeterReadings
                .Include(ssmr => ssmr.SubSubMeter)
                .Include(ssmr => ssmr.MeterType);
        }

        public IEnumerable<SubSubMeterReading> GetFilteredSubSubMeterReadingsIncludingSubSubMeterAndMeterType(string searchString)
        {
            return MainContext.SubSubMeterReadings
                .Include(ssmr => ssmr.SubSubMeter)
                .Include(ssmr => ssmr.MeterType)
                .Where(ssmr =>
                    ssmr.Index.ToString().ToLower().Contains(searchString) ||
                    ssmr.Date.ToString().ToLower().Contains(searchString) ||
                    ssmr.DiscountMonth.ToString().ToLower().Contains(searchString) ||
                    ssmr.SubSubMeter.Code.ToLower().Contains(searchString) ||
                    ssmr.MeterType.Type.ToLower().Contains(searchString));
        }

        public IEnumerable<SubSubMeterReading> OrderSubSubMeterReadings(IEnumerable<SubSubMeterReading> subSubMeterReadings, string sortOrder)
        {
            switch (sortOrder)
            {
                case "index_desc":
                    subSubMeterReadings = subSubMeterReadings.OrderByDescending(ssmr => ssmr.Index);
                    break;
                case "Date":
                    subSubMeterReadings = subSubMeterReadings.OrderBy(ssmr => ssmr.Date);
                    break;
                case "date_desc":
                    subSubMeterReadings = subSubMeterReadings.OrderByDescending(ssmr => ssmr.Date);
                    break;
                case "DiscountMonth":
                    subSubMeterReadings = subSubMeterReadings.OrderBy(ssmr => ssmr.DiscountMonth);
                    break;
                case "discountMonth_desc":
                    subSubMeterReadings = subSubMeterReadings.OrderByDescending(ssmr => ssmr.DiscountMonth);
                    break;
                case "Meter":
                    subSubMeterReadings = subSubMeterReadings.OrderBy(ssmr => ssmr.SubSubMeter.Code);
                    break;
                case "meter_desc":
                    subSubMeterReadings = subSubMeterReadings.OrderByDescending(ssmr => ssmr.SubSubMeter.Code);
                    break;
                case "MeterType":
                    subSubMeterReadings = subSubMeterReadings.OrderBy(ssmr => ssmr.MeterType.Type);
                    break;
                case "meterType_desc":
                    subSubMeterReadings = subSubMeterReadings.OrderByDescending(ssmr => ssmr.MeterType.Type);
                    break;
                default: // Index ascending 
                    subSubMeterReadings = subSubMeterReadings.OrderBy(ssmr => ssmr.Index);
                    break;
            }
            return subSubMeterReadings;
        }

        public SubSubMeterReading GetSubSubMeterReadingByDiscountMonth(int subSubMeterID, int meterTypeID, DateTime discountMonth)
        {
            var firstDayOfMonth = new DateTime(discountMonth.Year, discountMonth.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            return MainContext.SubSubMeterReadings.OrderByDescending(ssmr => ssmr.DiscountMonth).FirstOrDefault(ssmr => ssmr.SubSubMeterID == subSubMeterID && ssmr.MeterTypeID == meterTypeID && ssmr.DiscountMonth >= firstDayOfMonth && ssmr.DiscountMonth <= lastDayOfMonth);
        }

        public SubSubMeterReading GetPreviousSubSubMeterReading(int subSubMeterID, int meterTypeID, DateTime discountMonth)
        {
            var firstDayOfMonth = new DateTime(discountMonth.Year, discountMonth.Month, 1);
            return MainContext.SubSubMeterReadings.OrderByDescending(ssmr => ssmr.DiscountMonth).FirstOrDefault(ssmr => ssmr.SubSubMeterID == subSubMeterID && ssmr.MeterTypeID == meterTypeID && ssmr.DiscountMonth < firstDayOfMonth);
        }

        public SubSubMeterReading GetInitialSubSubMeterReading(int subSubMeterID, int meterTypeID)
        {
            return MainContext.SubSubMeterReadings.FirstOrDefault(ssmr => ssmr.SubSubMeterID == subSubMeterID && ssmr.MeterTypeID == meterTypeID && ssmr.Initial);
        }
    }
}