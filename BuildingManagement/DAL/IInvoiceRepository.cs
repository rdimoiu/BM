using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Invoice GetInvoiceIncludingClientAndProviderAndInvoiceTypeAndServices(int id);
        IEnumerable<Invoice> GetAllInvoicesIncludingClientAndProviderAndInvoiceTypeAndServices();
        IEnumerable<Invoice> GetFilteredInvoicesIncludingClientAndProviderAndInvoiceTypeAndServices(string searchString);
        IEnumerable<Invoice> OrderInvoices(IEnumerable<Invoice> invoices, string sortOrder);
    }
}