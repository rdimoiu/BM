﻿using System.Collections.Generic;
using System.Linq;
using BuildingManagement.Models;
using System.Data.Entity;

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
                MainContext.SubMeters.Include(sm => sm.MeterTypes)
                    .Include(sm => sm.DistributionMode)
                    .Include(sm => sm.Meter)
                    .Include(sm => sm.Sections)
                    .Include(sm => sm.Levels)
                    .Include(sm => sm.Spaces)
                    .SingleOrDefault(sm => sm.ID == id);
        }

        public SubMeter GetSubMeterIncludingMeterTypes(int id)
        {
            return
                MainContext.SubMeters.Include(sm => sm.MeterTypes)
                    .SingleOrDefault(sm => sm.ID == id);
        }

        public SubMeter GetSubMeterIncludingSectionsAndLevelsAndSpaces(int id)
        {
            return
                MainContext.SubMeters.Include(sm => sm.Sections)
                    .Include(sm => sm.Levels)
                    .Include(sm => sm.Spaces)
                    .SingleOrDefault(sm => sm.ID == id);
        }

        public IEnumerable<SubMeter> GetAllSubMetersIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces()
        {
            return
                MainContext.SubMeters.Include(sm => sm.MeterTypes)
                    .Include(sm => sm.DistributionMode)
                    .Include(sm => sm.Meter)
                    .Include(sm => sm.Sections)
                    .Include(sm => sm.Levels)
                    .Include(sm => sm.Spaces);
        }

        public IEnumerable<SubMeter> GetFilteredSubMetersIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces(string searchString)
        {
            return
                MainContext.SubMeters.Include(sm => sm.MeterTypes)
                    .Include(sm => sm.DistributionMode)
                    .Include(sm => sm.Meter)
                    .Include(sm => sm.Sections)
                    .Include(sm => sm.Levels)
                    .Include(sm => sm.Spaces)
                    .Where(
                        sm =>
                            sm.Code.ToLower().Contains(searchString) ||
                            sm.Details.ToLower().Contains(searchString) ||
                            sm.InitialIndex.ToString().ToLower().Contains(searchString) ||
                            sm.Defect.ToString().ToLower().Contains(searchString) ||
                            sm.DistributionMode.Mode.ToLower().Contains(searchString) ||
                            sm.Meter.Code.ToLower().Contains(searchString));
        }

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