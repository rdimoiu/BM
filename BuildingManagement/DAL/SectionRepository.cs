using BuildingManagement.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BuildingManagement.DAL
{
    public class SectionRepository : GenericRepository<Section>, ISectionRepository
    {
        public SectionRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public Section GetSectionIncludingClient(int id)
        {
            return MainContext.Sections
                .Include(s => s.Client)
                .SingleOrDefault(s => s.ID == id);
        }

        public Section GetSectionIncludingClientAndServices(int id)
        {
            return MainContext.Sections
                .Include(s => s.Client)
                .Include(s => s.Services)
                .SingleOrDefault(s => s.ID == id);
        }

        public IEnumerable<Section> GetAllSectionsIncludingClient()
        {
            return MainContext.Sections
                .Include(s => s.Client);
        }

        public IEnumerable<Section> GetFilteredSectionsIncludingClient(string searchString)
        {
            return MainContext.Sections
                .Include(s => s.Client)
                .Where(s =>
                    s.Client.Name.ToLower().Contains(searchString) ||
                    s.Number.ToLower().Contains(searchString) ||
                    s.Surface.ToString().ToLower().Contains(searchString) ||
                    s.People.ToString().ToLower().Contains(searchString));
        }

        public IEnumerable<Section> GetSectionsByClient(int clientId)
        {
            return MainContext.Sections
                .Where(s => s.ClientID == clientId);
        }

        public IEnumerable<Section> OrderSections(IEnumerable<Section> sections, string sortOrder)
        {
            switch (sortOrder)
            {
                case "client_desc":
                    sections = sections.OrderByDescending(s => s.Client.Name);
                    break;
                case "Number":
                    sections = sections.OrderBy(s => s.Number);
                    break;
                case "number_desc":
                    sections = sections.OrderByDescending(s => s.Number);
                    break;
                case "Surface":
                    sections = sections.OrderBy(s => s.Surface);
                    break;
                case "surface_desc":
                    sections = sections.OrderByDescending(s => s.Surface);
                    break;
                case "People":
                    sections = sections.OrderBy(s => s.People);
                    break;
                case "people_desc":
                    sections = sections.OrderByDescending(s => s.People);
                    break;
                default: // Client ascending 
                    sections = sections.OrderBy(s => s.Client.Name);
                    break;
            }
            return sections;
        }
    }
}