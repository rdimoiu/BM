using System;
using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface ISubMeterReadingRepository : IGenericRepository<SubMeterReading>
    {
        SubMeterReading GetSubMeterReadingIncludingSubMeterAndMeterType(int id);
        IEnumerable<SubMeterReading> GetAllSubMeterReadingsIncludingSubMeterAndMeterType();
        IEnumerable<SubMeterReading> GetFilteredSubMeterReadingsIncludingSubMeterAndMeterType(string searchString);
        IEnumerable<SubMeterReading> OrderSubMeterReadings(IEnumerable<SubMeterReading> subMeterReadings, string sortOrder);
        IEnumerable<SubMeterReading> GetLastSubMeterReading(int subMeterID, int meterTypeID, DateTime discountMonth);
        IEnumerable<SubMeterReading> GetPreviousSubMeterReading(int subMeterID, int meterTypeID, DateTime discountMonth);
        IEnumerable<SubMeterReading> GetInitialSubMeterReading(int subMeterID, int meterTypeID);
    }
}