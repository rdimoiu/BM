﻿using BuildingManagement.Models;
using System.Collections.Generic;

namespace BuildingManagement.DAL
{
    public interface ISubClientRepository : IGenericRepository<SubClient>
    {
        SubClient GetSubClientIncludingClient(int id);
        IEnumerable<SubClient> GetAllSubClientsIncludingClient();
        IEnumerable<SubClient> GetFilteredSubClientsIncludingClient(string searchString);
        IEnumerable<SubClient> OrderSubClients(IEnumerable<SubClient> subClients, string sortOrder);
    }
}