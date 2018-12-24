using System.Collections.Generic;
using System.Linq;
using BuildingManagement.Models;
using System.Data.Entity;

namespace BuildingManagement.DAL
{
    public class SubClientRepository : GenericRepository<SubClient>, ISubClientRepository
    {
        public SubClientRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public SubClient GetSubClientIncludingClient(int id)
        {
            return MainContext.SubClients
                .Include(sc => sc.Client)
                .SingleOrDefault(sc => sc.ID == id);
        }

        public IEnumerable<SubClient> GetAllSubClientsIncludingClient()
        {
            return MainContext.SubClients
                .Include(sc => sc.Client);
        }

        public IEnumerable<SubClient> GetFilteredSubClientsIncludingClient(string searchString)
        {
            return MainContext.SubClients
                .Include(sc => sc.Client)
                .Where(sc => 
                    sc.Name.ToLower().Contains(searchString) ||
                    sc.Phone.ToLower().Contains(searchString) ||
                    sc.Country.ToLower().Contains(searchString) ||
                    sc.State.ToLower().Contains(searchString) ||
                    sc.City.ToLower().Contains(searchString) ||
                    sc.Street.ToLower().Contains(searchString) ||
                    sc.Contact.ToLower().Contains(searchString) ||
                    sc.Email.ToLower().Contains(searchString) ||
                    sc.IBAN.ToLower().Contains(searchString) ||
                    sc.Bank.ToLower().Contains(searchString) ||
                    sc.CNP.ToLower().Contains(searchString) ||
                    sc.FiscalCode.ToLower().Contains(searchString) ||
                    sc.Client.Name.ToLower().Contains(searchString));
        }

        public IEnumerable<SubClient> OrderSubClients(IEnumerable<SubClient> subClients, string sortOrder)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    subClients = subClients.OrderByDescending(sc => sc.Name);
                    break;
                case "Phone":
                    subClients = subClients.OrderBy(sc => sc.Phone);
                    break;
                case "phone_desc":
                    subClients = subClients.OrderByDescending(sc => sc.Phone);
                    break;
                case "Country":
                    subClients = subClients.OrderBy(sc => sc.Country);
                    break;
                case "country_desc":
                    subClients = subClients.OrderByDescending(sc => sc.Country);
                    break;
                case "State":
                    subClients = subClients.OrderBy(sc => sc.State);
                    break;
                case "state_desc":
                    subClients = subClients.OrderByDescending(sc => sc.State);
                    break;
                case "City":
                    subClients = subClients.OrderBy(sc => sc.City);
                    break;
                case "city_desc":
                    subClients = subClients.OrderByDescending(sc => sc.City);
                    break;
                case "Street":
                    subClients = subClients.OrderBy(sc => sc.Street);
                    break;
                case "street_desc":
                    subClients = subClients.OrderByDescending(sc => sc.Street);
                    break;
                case "Contact":
                    subClients = subClients.OrderBy(sc => sc.Contact);
                    break;
                case "contact_desc":
                    subClients = subClients.OrderByDescending(sc => sc.Contact);
                    break;
                case "Email":
                    subClients = subClients.OrderBy(sc => sc.Email);
                    break;
                case "email_desc":
                    subClients = subClients.OrderByDescending(sc => sc.Email);
                    break;
                case "IBAN":
                    subClients = subClients.OrderBy(sc => sc.IBAN);
                    break;
                case "iban_desc":
                    subClients = subClients.OrderByDescending(sc => sc.IBAN);
                    break;
                case "Bank":
                    subClients = subClients.OrderBy(sc => sc.Bank);
                    break;
                case "bank_desc":
                    subClients = subClients.OrderByDescending(sc => sc.Bank);
                    break;
                case "CNP":
                    subClients = subClients.OrderBy(sc => sc.CNP);
                    break;
                case "cnp_desc":
                    subClients = subClients.OrderByDescending(sc => sc.CNP);
                    break;
                case "FiscalCode":
                    subClients = subClients.OrderBy(sc => sc.FiscalCode);
                    break;
                case "fiscalCode_desc":
                    subClients = subClients.OrderByDescending(sc => sc.FiscalCode);
                    break;
                case "Client":
                    subClients = subClients.OrderBy(sc => sc.Client.Name);
                    break;
                case "client_desc":
                    subClients = subClients.OrderByDescending(sc => sc.Client.Name);
                    break;
                default: // Name ascending 
                    subClients = subClients.OrderBy(sc => sc.Name);
                    break;
            }
            return subClients;
        }
    }
}