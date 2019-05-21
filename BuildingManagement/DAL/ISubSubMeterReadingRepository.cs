using System;
using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface ISubSubMeterReadingRepository : IGenericRepository<SubSubMeterReading>
    {
        SubSubMeterReading GetSubSubMeterReadingIncludingSubSubMeterAndMeterType(int id);
        IEnumerable<SubSubMeterReading> GetAllSubSubMeterReadingsIncludingSubSubMeterAndMeterType();
        IEnumerable<SubSubMeterReading> GetFilteredSubSubMeterReadingsIncludingSubSubMeterAndMeterType(string searchString);
        IEnumerable<SubSubMeterReading> OrderSubSubMeterReadings(IEnumerable<SubSubMeterReading> subSubMeterReadings, string sortOrder);
        IEnumerable<SubSubMeterReading> GetLastSubSubMeterReading(int subSubMeterID, int meterTypeID, DateTime discountMonth);
        IEnumerable<SubSubMeterReading> GetPreviousSubSubMeterReading(int subSubMeterID, int meterTypeID, DateTime discountMonth);
        IEnumerable<SubSubMeterReading> GetInitialSubSubMeterReading(int subSubMeterID, int meterTypeID);
    }
}