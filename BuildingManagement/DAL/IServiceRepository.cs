using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface IServiceRepository : IGenericRepository<Service>
    {
        Service GetServiceIncludingInvoiceAndDistributionModeAndSectionsAndLevelsAndSpaces(int id);
        Service GetServiceIncludingSectionsAndLevelsAndSpaces(int id);
        Service GetServiceIncludingSpacesAndCosts(int id);
        IEnumerable<Service> GetAllServicesIncludingInvoiceAndDistributionModeAndSectionsAndLevelsAndSpaces();
        IEnumerable<Service> GetFilteredServicesIncludingInvoiceAndDistributionModeAndSectionsAndLevelsAndSpaces(string searchString);
        IEnumerable<Service> OrderServices(IEnumerable<Service> services, string sortOrder);
    }
}