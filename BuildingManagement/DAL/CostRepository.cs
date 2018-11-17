﻿using System.Collections.Generic;
using System.Linq;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public class CostRepository : GenericRepository<Cost>, ICostRepository
    {
        public CostRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public IEnumerable<Cost> GetCostsByService(int serviceId)
        {
            return MainContext.Costs.Where(c => c.ServiceID == serviceId);
        }
    }
}