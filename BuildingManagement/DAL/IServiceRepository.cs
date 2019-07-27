using BuildingManagement.Models;
using System.Collections.Generic;

namespace BuildingManagement.DAL
{
    public interface IServiceRepository : IGenericRepository<Service>
    {
        Service GetServiceIncludingInvoiceAndDistributionModeAndMeterTypeAndSectionsAndLevelsAndSpaces(int id);
        Service GetServiceIncludingSectionsAndLevelsAndSpaces(int id);
        Service GetServiceIncludingSpacesAndCosts(int id);
        IEnumerable<Service> GetAllServicesIncludingInvoiceAndDistributionModeAndMeterTypeAndSectionsAndLevelsAndSpaces();
        IEnumerable<Service> GetFilteredServicesIncludingInvoiceAndDistributionModeAndMeterTypeAndSectionsAndLevelsAndSpaces(string searchString);
        IEnumerable<Service> OrderServices(IEnumerable<Service> services, string sortOrder);
    }
}