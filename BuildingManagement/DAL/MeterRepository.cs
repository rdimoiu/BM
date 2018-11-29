using System.Collections.Generic;
using System.Linq;
using BuildingManagement.Models;
using Z.EntityFramework.Plus;

namespace BuildingManagement.DAL
{
    public class MeterRepository : GenericRepository<Meter>, IMeterRepository
    {
        public MeterRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public Meter GetMeterIncludingMeterTypesAndDistributionModeAndClientAndSectionsAndLevelsAndSpaces(int id)
        {
            return
                MainContext.Meters.IncludeOptimized(m => m.MeterTypes)
                    .IncludeOptimized(m => m.DistributionMode)
                    .IncludeOptimized(m => m.Client)
                    .IncludeOptimized(m => m.Sections)
                    .IncludeOptimized(m => m.Levels)
                    .IncludeOptimized(m => m.Spaces)
                    .SingleOrDefault(m => m.ID == id);
        }

        public Meter GetMeterIncludingMeterTypes(int id)
        {
            return
                MainContext.Meters.IncludeOptimized(m => m.MeterTypes)
                    .SingleOrDefault(m => m.ID == id);
        }

        public Meter GetMeterIncludingSectionsAndLevelsAndSpaces(int id)
        {
            return
                MainContext.Meters.IncludeOptimized(m => m.Sections)
                    .IncludeOptimized(m => m.Levels)
                    .IncludeOptimized(m => m.Spaces)
                    .SingleOrDefault(m => m.ID == id);
        }

        public IEnumerable<Meter> GetAllMetersIncludingMeterTypesAndDistributionModeAndClientAndSectionsAndLevelsAndSpaces()
        {
            return
                MainContext.Meters.IncludeOptimized(m => m.MeterTypes)
                    .IncludeOptimized(m => m.DistributionMode)
                    .IncludeOptimized(m => m.Client)
                    .IncludeOptimized(m => m.Sections)
                    .IncludeOptimized(m => m.Levels)
                    .IncludeOptimized(m => m.Spaces);
        }

        public IEnumerable<Meter> GetFilteredMetersIncludingMeterTypesAndDistributionModeAndClientAndSectionsAndLevelsAndSpaces(string searchString)
        {
            return
                MainContext.Meters.IncludeOptimized(m => m.MeterTypes)
                    .IncludeOptimized(m => m.DistributionMode)
                    .IncludeOptimized(m => m.Client)
                    .IncludeOptimized(m => m.Sections)
                    .IncludeOptimized(m => m.Levels)
                    .IncludeOptimized(m => m.Spaces)
                    .Where(
                        m =>
                            m.Code.ToLower().Contains(searchString) ||
                            m.Details.ToLower().Contains(searchString) ||
                            m.InitialIndex.ToString().ToLower().Contains(searchString) ||
                            m.Defect.ToString().ToLower().Contains(searchString) ||
                            m.DistributionMode.Mode.ToLower().Contains(searchString)||
                            m.Client.Name.ToLower().Contains(searchString));
        }

        public IEnumerable<Meter> OrderMeters(IEnumerable<Meter> meters, string sortOrder)
        {
            switch (sortOrder)
            {
                case "code_desc":
                    meters = meters.OrderByDescending(m => m.Code);
                    break;
                case "Details":
                    meters = meters.OrderBy(m => m.Details);
                    break;
                case "details_desc":
                    meters = meters.OrderByDescending(m => m.Details);
                    break;
                case "InitialIndex":
                    meters = meters.OrderBy(m => m.InitialIndex);
                    break;
                case "initialIndex_desc":
                    meters = meters.OrderByDescending(m => m.InitialIndex);
                    break;
                case "Defect":
                    meters = meters.OrderBy(m => m.Defect);
                    break;
                case "defect_desc":
                    meters = meters.OrderByDescending(m => m.Defect);
                    break;
                case "DistributionMode":
                    meters = meters.OrderBy(m => m.DistributionMode.Mode);
                    break;
                case "distributionMode_desc":
                    meters = meters.OrderByDescending(m => m.DistributionMode.Mode);
                    break;
                case "Client":
                    meters = meters.OrderBy(m => m.Client.Name);
                    break;
                case "client_desc":
                    meters = meters.OrderByDescending(m => m.Client.Name);
                    break;
                default: // Code ascending 
                    meters = meters.OrderBy(m => m.Code);
                    break;
            }
            return meters;
        }
    }
}