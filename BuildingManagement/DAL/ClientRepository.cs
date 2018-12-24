using System.Collections.Generic;
using System.Linq;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        public ClientRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public IEnumerable<Client> GetFilteredClients(string searchString)
        {
            return MainContext.Clients
                .Where(c =>
                    c.Name.ToLower().Contains(searchString) ||
                    c.Phone.ToLower().Contains(searchString) ||
                    c.Address.ToLower().Contains(searchString) ||
                    c.Contact.ToLower().Contains(searchString) ||
                    c.Email.ToLower().Contains(searchString));
        }

        public IEnumerable<Client> OrderClients(IEnumerable<Client> clients, string sortOrder)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    clients = clients.OrderByDescending(c => c.Name);
                    break;
                case "Phone":
                    clients = clients.OrderBy(c => c.Phone);
                    break;
                case "phone_desc":
                    clients = clients.OrderByDescending(c => c.Phone);
                    break;
                case "Address":
                    clients = clients.OrderBy(c => c.Address);
                    break;
                case "address_desc":
                    clients = clients.OrderByDescending(c => c.Address);
                    break;
                case "Contact":
                    clients = clients.OrderBy(c => c.Contact);
                    break;
                case "contact_desc":
                    clients = clients.OrderByDescending(c => c.Contact);
                    break;
                case "Email":
                    clients = clients.OrderBy(c => c.Email);
                    break;
                case "email_desc":
                    clients = clients.OrderByDescending(c => c.Email);
                    break;
                default: // Name ascending 
                    clients = clients.OrderBy(c => c.Name);
                    break;
            }
            return clients;
        }
    }
}