﻿using BuildingManagement.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BuildingManagement.DAL
{
    public class SubSubMeterRepository : GenericRepository<SubSubMeter>, ISubSubMeterRepository
    {
        public SubSubMeterRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public SubSubMeter GetSubSubMeterIncludingMeterTypesAndDistributionModeAndSubMeterAndSectionsAndLevelsAndSpaces(int id)
        {
            return MainContext.SubSubMeters
                .Include(ssm => ssm.MeterTypes)
                .Include(ssm => ssm.SubMeter)
                .Include(ssm => ssm.Sections)
                .Include(ssm => ssm.Levels)
                .Include(ssm => ssm.Spaces)
                .SingleOrDefault(ssm => ssm.ID == id);
        }

        public SubSubMeter GetSubSubMeterIncludingMeterTypes(int id)
        {
            return MainContext.SubSubMeters
                .Include(ssm => ssm.MeterTypes)
                .SingleOrDefault(ssm => ssm.ID == id);
        }

        public SubSubMeter GetSubSubMeterIncludingSectionsAndLevelsAndSpaces(int id)
        {
            return MainContext.SubSubMeters
                .Include(ssm => ssm.Sections)
                .Include(ssm => ssm.Levels)
                .Include(ssm => ssm.Spaces)
                .SingleOrDefault(ssm => ssm.ID == id);
        }

        public IEnumerable<SubSubMeter> GetAllSubSubMetersIncludingMeterTypesAndDistributionModeAndSubMeterAndSectionsAndLevelsAndSpaces()
        {
            return MainContext.SubSubMeters
                .Include(ssm => ssm.MeterTypes)
                .Include(ssm => ssm.SubMeter)
                .Include(ssm => ssm.Sections)
                .Include(ssm => ssm.Levels)
                .Include(ssm => ssm.Spaces);
        }

        public IEnumerable<SubSubMeter> GetFilteredSubSubMetersIncludingMeterTypesAndDistributionModeAndSubMeterAndSectionsAndLevelsAndSpaces(string searchString)
        {
            return MainContext.SubSubMeters
                .Include(ssm => ssm.MeterTypes)
                .Include(ssm => ssm.SubMeter)
                .Include(ssm => ssm.Sections)
                .Include(ssm => ssm.Levels)
                .Include(ssm => ssm.Spaces)
                .Where(ssm =>
                    ssm.Code.ToLower().Contains(searchString) ||
                    ssm.Details.ToLower().Contains(searchString) ||
                    ssm.Defect.ToString().ToLower().Contains(searchString) ||
                    ssm.SubMeter.Code.ToLower().Contains(searchString));
        }

        public IEnumerable<SubSubMeter> OrderSubSubMeters(IEnumerable<SubSubMeter> subSubMeters, string sortOrder)
        {
            switch (sortOrder)
            {
                case "code_desc":
                    subSubMeters = subSubMeters.OrderByDescending(ssm => ssm.Code);
                    break;
                case "Details":
                    subSubMeters = subSubMeters.OrderBy(ssm => ssm.Details);
                    break;
                case "details_desc":
                    subSubMeters = subSubMeters.OrderByDescending(ssm => ssm.Details);
                    break;
                case "Defect":
                    subSubMeters = subSubMeters.OrderBy(ssm => ssm.Defect);
                    break;
                case "defect_desc":
                    subSubMeters = subSubMeters.OrderByDescending(ssm => ssm.Defect);
                    break;
                case "SubMeter":
                    subSubMeters = subSubMeters.OrderBy(ssm => ssm.SubMeter.Code);
                    break;
                case "subMeter_desc":
                    subSubMeters = subSubMeters.OrderByDescending(ssm => ssm.SubMeter.Code);
                    break;
                default: // Code ascending 
                    subSubMeters = subSubMeters.OrderBy(ssm => ssm.Code);
                    break;
            }
            return subSubMeters;
        }

        public IEnumerable<int> GetSubSubMeterIDsBySubMeterIDNoDefect(int subMeterID)
        {
            return MainContext.SubSubMeters.Where(ssm => ssm.SubMeterID == subMeterID && !ssm.Defect).Select(ssm => ssm.ID);
        }

        public IEnumerable<SubSubMeter> GetAllNoDefect()
        {
            return MainContext.SubSubMeters.Where(ssm => !ssm.Defect);
        }
    }
}