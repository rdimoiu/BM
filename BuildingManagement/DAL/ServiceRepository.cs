using BuildingManagement.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BuildingManagement.DAL
{
    public class ServiceRepository : GenericRepository<Service>, IServiceRepository
    {
        public ServiceRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public Service GetServiceIncludingInvoiceAndDistributionModeAndMeterTypeAndSectionsAndLevelsAndSpaces(int id)
        {
            return MainContext.Services
                .Include(s => s.Invoice)
                .Include(s => s.DistributionMode)
                .Include(s => s.MeterType)
                .Include(s => s.Sections)
                .Include(s => s.Levels)
                .Include(s => s.Spaces)
                .SingleOrDefault(s => s.ID == id);
        }

        public Service GetServiceIncludingSectionsAndLevelsAndSpaces(int id)
        {
            return MainContext.Services
                .Include(s => s.Sections)
                .Include(s => s.Levels)
                .Include(s => s.Spaces)
                .SingleOrDefault(s => s.ID == id);
        }

        public Service GetServiceIncludingSpacesAndCosts(int id)
        {
            return MainContext.Services
                .Include(s => s.Spaces)
                .Include(s => s.Costs)
                .SingleOrDefault(s => s.ID == id);
        }

        public IEnumerable<Service> GetAllServicesIncludingInvoiceAndDistributionModeAndMeterTypeAndSectionsAndLevelsAndSpaces()
        {
            return MainContext.Services
                .Include(s => s.Invoice)
                .Include(s => s.DistributionMode)
                .Include(s => s.MeterType)
                .Include(s => s.Sections)
                .Include(s => s.Levels)
                .Include(s => s.Spaces);
        }

        public IEnumerable<Service> GetFilteredServicesIncludingInvoiceAndDistributionModeAndMeterTypeAndSectionsAndLevelsAndSpaces(string searchString)
        {
            return MainContext.Services
                .Include(s => s.Invoice)
                .Include(s => s.DistributionMode)
                .Include(s => s.MeterType)
                .Include(s => s.Sections)
                .Include(s => s.Levels)
                .Include(s => s.Spaces)
                .Where(s =>
                    s.Name.ToLower().Contains(searchString) ||
                    s.Quantity.ToString().ToLower().Contains(searchString) ||
                    s.Unit.ToLower().Contains(searchString) ||
                    s.Price.ToString().ToLower().Contains(searchString) ||
                    s.QuotaTVA.ToString().ToLower().Contains(searchString) ||
                    s.Invoice.Number.ToLower().Contains(searchString));
        }

        public IEnumerable<Service> OrderServices(IEnumerable<Service> services, string sortOrder)
        {
            switch (sortOrder)
            {
                case "invoice_desc":
                    services = services.OrderByDescending(s => s.Invoice.Number);
                    break;
                case "Name":
                    services = services.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    services = services.OrderByDescending(s => s.Name);
                    break;
                case "Quantity":
                    services = services.OrderBy(s => s.Quantity);
                    break;
                case "quantity_desc":
                    services = services.OrderByDescending(s => s.Quantity);
                    break;
                case "Unit":
                    services = services.OrderBy(s => s.Unit);
                    break;
                case "unit_desc":
                    services = services.OrderByDescending(s => s.Unit);
                    break;
                case "Price":
                    services = services.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    services = services.OrderByDescending(s => s.Price);
                    break;
                case "QuotaTVA":
                    services = services.OrderBy(s => s.QuotaTVA);
                    break;
                case "quotaTVA_desc":
                    services = services.OrderByDescending(s => s.QuotaTVA);
                    break;
                case "Fixed":
                    services = services.OrderBy(s => s.Fixed);
                    break;
                case "fixed_desc":
                    services = services.OrderByDescending(s => s.Fixed);
                    break;
                case "Counted":
                    services = services.OrderBy(s => s.Counted);
                    break;
                case "counted_desc":
                    services = services.OrderByDescending(s => s.Counted);
                    break;
                case "Inhabited":
                    services = services.OrderBy(s => s.Inhabited);
                    break;
                case "inhabited_desc":
                    services = services.OrderByDescending(s => s.Inhabited);
                    break;
                default: // Invoice ascending 
                    services = services.OrderBy(s => s.Invoice.Number);
                    break;
            }
            return services;
        }
    }
}