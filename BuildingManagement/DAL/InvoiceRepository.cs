using System.Collections.Generic;
using System.Linq;
using BuildingManagement.Models;
using System.Data.Entity;

namespace BuildingManagement.DAL
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(MainContext context)
            : base(context)
        {
        }

        public MainContext MainContext => Context as MainContext;

        public Invoice GetInvoiceIncludingClientAndProviderAndInvoiceTypeAndServices(int id)
        {
            return MainContext.Invoices
                .Include(i => i.Client)
                .Include(i => i.Provider)
                .Include(i => i.InvoiceType)
                .Include(i => i.Services)
                .SingleOrDefault(i => i.ID == id);
        }

        public IEnumerable<Invoice> GetAllInvoicesIncludingClientAndProviderAndInvoiceTypeAndServices()
        {
            return MainContext.Invoices
                .Include(i => i.Client)
                .Include(i => i.Provider)
                .Include(i => i.InvoiceType)
                .Include(i => i.Services);
        }

        public IEnumerable<Invoice> GetFilteredInvoicesIncludingClientAndProviderAndInvoiceTypeAndServices(string searchString)
        {
            return MainContext.Invoices
                .Include(i => i.Client)
                .Include(i => i.Provider)
                .Include(i => i.InvoiceType)
                .Include(i => i.Services)
                .Where(i =>
                    i.Client.Name.ToLower().Contains(searchString) ||
                    i.Provider.Name.ToLower().Contains(searchString) ||
                    i.InvoiceType.Type.ToLower().Contains(searchString) ||
                    i.Number.ToLower().Contains(searchString) ||
                    i.Date.ToString().ToLower().Contains(searchString) ||
                    i.DueDate.ToString().ToLower().Contains(searchString) ||
                    i.PaidDate.ToString().ToLower().Contains(searchString) ||
                    i.Quantity.ToString().ToLower().Contains(searchString) ||
                    i.CheckQuantity.ToString().ToLower().Contains(searchString) ||
                    i.TotalValueWithoutTVA.ToString().ToLower().Contains(searchString) ||
                    i.CheckTotalValueWithoutTVA.ToString().ToLower().Contains(searchString) ||
                    i.TotalTVA.ToString().ToLower().Contains(searchString) ||
                    i.CheckTotalTVA.ToString().ToLower().Contains(searchString) ||
                    i.DiscountMonth.ToString().ToLower().Contains(searchString));
        }

        public IEnumerable<Invoice> OrderInvoices(IEnumerable<Invoice> invoices, string sortOrder)
        {
            switch (sortOrder)
            {
                case "client_desc":
                    invoices = invoices.OrderByDescending(i => i.Client.Name);
                    break;
                case "Provider":
                    invoices = invoices.OrderBy(i => i.Provider.Name);
                    break;
                case "provider_desc":
                    invoices = invoices.OrderByDescending(i => i.Provider.Name);
                    break;
                case "InvoiceType":
                    invoices = invoices.OrderBy(i => i.InvoiceType.Type);
                    break;
                case "invoiceType_desc":
                    invoices = invoices.OrderByDescending(i => i.InvoiceType.Type);
                    break;
                case "Number":
                    invoices = invoices.OrderBy(i => i.Number);
                    break;
                case "number_desc":
                    invoices = invoices.OrderByDescending(i => i.Number);
                    break;
                case "Date":
                    invoices = invoices.OrderBy(i => i.Date);
                    break;
                case "date_desc":
                    invoices = invoices.OrderByDescending(i => i.Date);
                    break;
                case "DueDate":
                    invoices = invoices.OrderBy(i => i.DueDate);
                    break;
                case "dueDate_desc":
                    invoices = invoices.OrderByDescending(i => i.DueDate);
                    break;
                case "PaidDate":
                    invoices = invoices.OrderBy(i => i.PaidDate);
                    break;
                case "paidDate_desc":
                    invoices = invoices.OrderByDescending(i => i.PaidDate);
                    break;
                case "Quantity":
                    invoices = invoices.OrderBy(i => i.Quantity);
                    break;
                case "quantity_desc":
                    invoices = invoices.OrderByDescending(i => i.Quantity);
                    break;
                case "CheckQuantity":
                    invoices = invoices.OrderBy(i => i.CheckQuantity);
                    break;
                case "checkQuantity_desc":
                    invoices = invoices.OrderByDescending(i => i.CheckQuantity);
                    break;
                case "TotalValueWithoutTVA":
                    invoices = invoices.OrderBy(i => i.TotalValueWithoutTVA);
                    break;
                case "totalValueWithoutTVA_desc":
                    invoices = invoices.OrderByDescending(i => i.TotalValueWithoutTVA);
                    break;
                case "CheckTotalValueWithoutTVA":
                    invoices = invoices.OrderBy(i => i.CheckTotalValueWithoutTVA);
                    break;
                case "checkTotalValueWithoutTVA_desc":
                    invoices = invoices.OrderByDescending(i => i.CheckTotalValueWithoutTVA);
                    break;
                case "TotalTVA":
                    invoices = invoices.OrderBy(i => i.TotalTVA);
                    break;
                case "totalTVA_desc":
                    invoices = invoices.OrderByDescending(i => i.TotalTVA);
                    break;
                case "CheckTotalTVA":
                    invoices = invoices.OrderBy(i => i.CheckTotalTVA);
                    break;
                case "checkTotalTVA_desc":
                    invoices = invoices.OrderByDescending(i => i.CheckTotalTVA);
                    break;
                case "DiscountMonth":
                    invoices = invoices.OrderBy(i => i.DiscountMonth);
                    break;
                case "DiscountMonth_desc":
                    invoices = invoices.OrderByDescending(i => i.DiscountMonth);
                    break;
                default: // Client ascending 
                    invoices = invoices.OrderBy(i => i.Client.Name);
                    break;
            }
            return invoices;
        }
    }
}