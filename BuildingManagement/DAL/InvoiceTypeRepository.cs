using BuildingManagement.Models;
using System.Collections.Generic;
using System.Linq;

namespace BuildingManagement.DAL
{
    public class InvoiceTypeRepository : GenericRepository<InvoiceType>, IInvoiceTypeRepository
    {
        public InvoiceTypeRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public IEnumerable<InvoiceType> GetFilteredInvoiceTypes(string searchString)
        {
            return MainContext.InvoiceTypes
                .Where(it => it.Type.ToLower().Contains(searchString));
        }

        public IEnumerable<InvoiceType> OrderInvoiceTypes(IEnumerable<InvoiceType> invoiceTypes, string sortOrder)
        {
            switch (sortOrder)
            {
                case "type_desc":
                    invoiceTypes = invoiceTypes.OrderByDescending(it => it.Type);
                    break;
                default: // Type ascending 
                    invoiceTypes = invoiceTypes.OrderBy(it => it.Type);
                    break;
            }
            return invoiceTypes;
        }
    }
}