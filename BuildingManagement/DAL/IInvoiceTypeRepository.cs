using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface IInvoiceTypeRepository : IGenericRepository<InvoiceType>
    {
        IEnumerable<InvoiceType> GetFilteredInvoiceTypes(string searchString);
        IEnumerable<InvoiceType> OrderInvoiceTypes(IEnumerable<InvoiceType> invoiceTypes, string sortOrder);
    }
}