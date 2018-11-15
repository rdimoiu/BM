﻿using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface IProviderRepository : IGenericRepository<Provider>
    {
        IEnumerable<Provider> GetFilteredProviders(string searchString);
        IEnumerable<Provider> OrderProviders(IEnumerable<Provider> providers, string sortOrder);
    }
}