using System.Collections.Generic;
using System.Linq;
using BuildingManagement.Models;
using Z.EntityFramework.Plus;

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
            return
                MainContext.SubMeters.IncludeOptimized(sm => sm.MeterTypes)
                    .IncludeOptimized(sm => sm.DistributionMode)
                    .IncludeOptimized(sm => sm.Meter)
                    .IncludeOptimized(sm => sm.Sections)
                    .IncludeOptimized(sm => sm.Levels)
                    .IncludeOptimized(sm => sm.Spaces)
                    .SingleOrDefault(sm => sm.ID == id);
        }

        public SubMeter GetSubMeterIncludingMeterTypes(int id)
        {
            return
                MainContext.SubMeters.IncludeOptimized(sm => sm.MeterTypes)
                    .SingleOrDefault(sm => sm.ID == id);
        }

        public SubMeter GetSubMeterIncludingSectionsAndLevelsAndSpaces(int id)
        {
            return
                MainContext.SubMeters.IncludeOptimized(sm => sm.Sections)
                    .IncludeOptimized(sm => sm.Levels)
                    .IncludeOptimized(sm => sm.Spaces)
                    .SingleOrDefault(sm => sm.ID == id);
        }

        public IEnumerable<SubMeter> GetAllSubMetersIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces(string sortOrder)
        {
            var subMeters = MainContext.SubMeters.IncludeOptimized(sm => sm.MeterTypes)
                    .IncludeOptimized(sm => sm.DistributionMode)
                    .IncludeOptimized(sm => sm.Meter)
                    .IncludeOptimized(sm => sm.Sections)
                    .IncludeOptimized(sm => sm.Levels)
                    .IncludeOptimized(sm => sm.Spaces);
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
                case "InitialIndex":
                    subMeters = subMeters.OrderBy(sm => sm.InitialIndex);
                    break;
                case "initialIndex_desc":
                    subMeters = subMeters.OrderByDescending(sm => sm.InitialIndex);
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
            var subMeters = MainContext.SubMeters.IncludeOptimized(sm => sm.MeterTypes)
                    .IncludeOptimized(sm => sm.DistributionMode)
                    .IncludeOptimized(sm => sm.Meter)
                    .IncludeOptimized(sm => sm.Sections)
                    .IncludeOptimized(sm => sm.Levels)
                    .IncludeOptimized(sm => sm.Spaces)
                    .Where(
                        sm =>
                            sm.Code.ToLower().Contains(searchString) ||
                            sm.Details.ToLower().Contains(searchString) ||
                            sm.InitialIndex.ToString().ToLower().Contains(searchString) ||
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
                case "InitialIndex":
                    subMeters = subMeters.OrderBy(sm => sm.InitialIndex);
                    break;
                case "initialIndex_desc":
                    subMeters = subMeters.OrderByDescending(sm => sm.InitialIndex);
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
                case "InitialIndex":
                    subMeters = subMeters.OrderBy(sm => sm.InitialIndex);
                    break;
                case "initialIndex_desc":
                    subMeters = subMeters.OrderByDescending(sm => sm.InitialIndex);
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