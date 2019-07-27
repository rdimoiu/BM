using BuildingManagement.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BuildingManagement.DAL
{
    public class SubMeterRepository : GenericRepository<SubMeter>, ISubMeterRepository
    {
        public SubMeterRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public SubMeter GetSubMeterIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces(int id)
        {
            return MainContext.SubMeters
                .Include(sm => sm.MeterTypes)
                .Include(sm => sm.DistributionMode)
                .Include(sm => sm.Meter)
                .Include(sm => sm.Sections)
                .Include(sm => sm.Levels)
                .Include(sm => sm.Spaces)
                .SingleOrDefault(sm => sm.ID == id);
        }

        public SubMeter GetSubMeterIncludingMeterTypes(int id)
        {
            return MainContext.SubMeters
                .Include(sm => sm.MeterTypes)
                .SingleOrDefault(sm => sm.ID == id);
        }

        public SubMeter GetSubMeterIncludingSectionsAndLevelsAndSpaces(int id)
        {
            return MainContext.SubMeters
                .Include(sm => sm.Sections)
                .Include(sm => sm.Levels)
                .Include(sm => sm.Spaces)
                .SingleOrDefault(sm => sm.ID == id);
        }

        public IEnumerable<SubMeter> GetAllSubMetersIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces(string sortOrder)
        {
            var subMeters = MainContext.SubMeters
                .Include(sm => sm.MeterTypes)
                .Include(sm => sm.DistributionMode)
                .Include(sm => sm.Meter)
                .Include(sm => sm.Sections)
                .Include(sm => sm.Levels)
                .Include(sm => sm.Spaces);
            switch (sortOrder)
            {
                case "code_desc":
                    subMeters = subMeters.OrderByDescending(sm => sm.Code);
                    break;
                case "Details":
                    subMeters = subMeters.OrderBy(sm => sm.Details);
                    break;
                case "details_desc":
                    subMeters = subMeters.OrderByDescending(sm => sm.Details);
                    break;
                case "Defect":
                    subMeters = subMeters.OrderBy(sm => sm.Defect);
                    break;
                case "defect_desc":
                    subMeters = subMeters.OrderByDescending(sm => sm.Defect);
                    break;
                case "DistributionMode":
                    subMeters = subMeters.OrderBy(sm => sm.DistributionMode.Mode);
                    break;
                case "distributionMode_desc":
                    subMeters = subMeters.OrderByDescending(sm => sm.DistributionMode.Mode);
                    break;
                case "Meter":
                    subMeters = subMeters.OrderBy(sm => sm.Meter.Code);
                    break;
                case "meter_desc":
                    subMeters = subMeters.OrderByDescending(sm => sm.Meter.Code);
                    break;
                default: // Code ascending 
                    subMeters = subMeters.OrderBy(sm => sm.Code);
                    break;
            }
            return subMeters;
        }

        public IEnumerable<SubMeter> GetFilteredSubMetersIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces(string searchString, string sortOrder)
        {
            var subMeters = MainContext.SubMeters
                .Include(sm => sm.MeterTypes)
                .Include(sm => sm.DistributionMode)
                .Include(sm => sm.Meter)
                .Include(sm => sm.Sections)
                .Include(sm => sm.Levels)
                .Include(sm => sm.Spaces)
                .Where(sm =>
                    sm.Code.ToLower().Contains(searchString) ||
                    sm.Details.ToLower().Contains(searchString) ||
                    sm.Defect.ToString().ToLower().Contains(searchString) ||
                    sm.DistributionMode.Mode.ToLower().Contains(searchString) ||
                    sm.Meter.Code.ToLower().Contains(searchString));
            switch (sortOrder)
            {
                case "code_desc":
                    subMeters = subMeters.OrderByDescending(sm => sm.Code);
                    break;
                case "Details":
                    subMeters = subMeters.OrderBy(sm => sm.Details);
                    break;
                case "details_desc":
                    subMeters = subMeters.OrderByDescending(sm => sm.Details);
                    break;
                case "Defect":
                    subMeters = subMeters.OrderBy(sm => sm.Defect);
                    break;
                case "defect_desc":
                    subMeters = subMeters.OrderByDescending(sm => sm.Defect);
                    break;
                case "DistributionMode":
                    subMeters = subMeters.OrderBy(sm => sm.DistributionMode.Mode);
                    break;
                case "distributionMode_desc":
                    subMeters = subMeters.OrderByDescending(sm => sm.DistributionMode.Mode);
                    break;
                case "Meter":
                    subMeters = subMeters.OrderBy(sm => sm.Meter.Code);
                    break;
                case "meter_desc":
                    subMeters = subMeters.OrderByDescending(sm => sm.Meter.Code);
                    break;
                default: // Code ascending 
                    subMeters = subMeters.OrderBy(sm => sm.Code);
                    break;
            }
            return subMeters;
        }

        //to be deleted
        public IEnumerable<SubMeter> OrderSubMeters(IEnumerable<SubMeter> subMeters, string sortOrder)
        {
            switch (sortOrder)
            {
                case "code_desc":
                    subMeters = subMeters.OrderByDescending(sm => sm.Code);
                    break;
                case "Details":
                    subMeters = subMeters.OrderBy(sm => sm.Details);
                    break;
                case "details_desc":
                    subMeters = subMeters.OrderByDescending(sm => sm.Details);
                    break;
                case "Defect":
                    subMeters = subMeters.OrderBy(sm => sm.Defect);
                    break;
                case "defect_desc":
                    subMeters = subMeters.OrderByDescending(sm => sm.Defect);
                    break;
                case "DistributionMode":
                    subMeters = subMeters.OrderBy(sm => sm.DistributionMode.Mode);
                    break;
                case "distributionMode_desc":
                    subMeters = subMeters.OrderByDescending(sm => sm.DistributionMode.Mode);
                    break;
                case "Meter":
                    subMeters = subMeters.OrderBy(sm => sm.Meter.Code);
                    break;
                case "meter_desc":
                    subMeters = subMeters.OrderByDescending(sm => sm.Meter.Code);
                    break;
                default: // Code ascending 
                    subMeters = subMeters.OrderBy(sm => sm.Code);
                    break;
            }
            return subMeters;
        }
    }
}