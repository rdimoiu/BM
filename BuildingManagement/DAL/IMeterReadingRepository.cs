using System;
using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface IMeterReadingRepository : IGenericRepository<MeterReading>
    {
        MeterReading GetMeterReadingIncludingMeterAndMeterType(int id);
        IEnumerable<MeterReading> GetAllMeterReadingsIncludingMeterAndMeterType();
        IEnumerable<MeterReading> GetFilteredMeterReadingsIncludingMeterAndMeterType(string searchString);
        IEnumerable<MeterReading> OrderMeterReadings(IEnumerable<MeterReading> meterReadings, string sortOrder);
        MeterReading GetMeterReadingByDiscountMonth(int meterID, int meterTypeID, DateTime discountMonth);
        MeterReading GetPreviousMeterReading(int meterID, int meterTypeID, DateTime discountMonth);
        MeterReading GetInitialMeterReading(int meterID, int meterTypeID);
    }
}