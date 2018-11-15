using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface ISectionRepository : IGenericRepository<Section>
    {
        Section GetSectionIncludingClient(int id);
        IEnumerable<Section> GetAllSectionsIncludingClient();
        IEnumerable<Section> GetFilteredSectionsIncludingClient(string searchString);
        IEnumerable<Section> GetSectionsByClient(int clientId);
        IEnumerable<Section> OrderSections(IEnumerable<Section> sections, string sortOrder);
    }
}