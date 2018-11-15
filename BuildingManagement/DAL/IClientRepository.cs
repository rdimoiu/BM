using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        IEnumerable<Client> GetFilteredClients(string searchString);
        IEnumerable<Client> OrderClients(IEnumerable<Client> clients, string sortOrder);
    }
}