using BuildingManagement.Models;
using System.Collections.Generic;

namespace BuildingManagement.DAL
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        IEnumerable<Client> GetFilteredClients(string searchString);
        IEnumerable<Client> OrderClients(IEnumerable<Client> clients, string sortOrder);
    }
}