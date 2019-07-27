using BuildingManagement.Models;
using System.Collections.Generic;

namespace BuildingManagement.DAL
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Invoice GetInvoiceIncludingClientAndProviderAndInvoiceTypeAndServices(int id);
        Invoice GetInvoiceIncludingServices(int id);
        IEnumerable<Invoice> GetAllInvoicesIncludingClientAndProviderAndInvoiceTypeAndServices();
        IEnumerable<Invoice> GetFilteredInvoicesIncludingClientAndProviderAndInvoiceTypeAndServices(string searchString);
        IEnumerable<Invoice> OrderInvoices(IEnumerable<Invoice> invoices, string sortOrder);
    }
}