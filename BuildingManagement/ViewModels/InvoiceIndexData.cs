using System.Collections.Generic;
using BuildingManagement.Models;

namespace BuildingManagement.ViewModels
{
    public class InvoiceIndexData
    {
        public IEnumerable<Invoice> Invoices;
        public IEnumerable<Service> Services;
    }
}