using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using BuildingManagement.ViewModels;

namespace BuildingManagement.Controllers
{
    public class InvoiceDistributionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceDistributionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: InvoiceDistribution
        public ActionResult Index(DateTime? discountMonth, int? clientId, int? providerId)
        {
            InvoiceDistributionIndexData invoiceDistributionIndexData = new InvoiceDistributionIndexData
            {
                //DiscountMonth = DateTime.Now,
                Client = new Client(),
                Provider = new Provider(),
                Invoices = new List<Invoice>(),
                Services = new List<Service>()
            };

            if (discountMonth != null)
            {
                invoiceDistributionIndexData.DiscountMonth = (DateTime) discountMonth;
                invoiceDistributionIndexData.Invoices = _unitOfWork.InvoiceRepository.GetAllInvoicesIncludingClientAndProviderAndInvoiceTypeAndServices().Where(i => i.DiscountMonth.Month == ((DateTime) discountMonth).Month).ToList();
            }
            if (clientId != null)
            {
                invoiceDistributionIndexData.Invoices = invoiceDistributionIndexData.Invoices.Where(i => i.ClientID == clientId);
            }

            if (discountMonth != null)
            {
                invoiceDistributionIndexData.DiscountMonth = (DateTime) discountMonth;
                if (clientId != null)
                {
                    invoiceDistributionIndexData.ClientID = (int) clientId;
                    if (providerId != null)
                    {
                        invoiceDistributionIndexData.ProviderID = (int)providerId;
                        invoiceDistributionIndexData.Invoices = _unitOfWork.InvoiceRepository.GetAllInvoicesIncludingClientAndProviderAndInvoiceTypeAndServices().Where(i => i.DiscountMonth.Month == ((DateTime)discountMonth).Month && i.ClientID == clientId && i.ProviderID == providerId).ToList();
                    }
                    else
                    {
                        invoiceDistributionIndexData.Invoices = _unitOfWork.InvoiceRepository.GetAllInvoicesIncludingClientAndProviderAndInvoiceTypeAndServices().Where(i => i.DiscountMonth.Month == ((DateTime)discountMonth).Month && i.ClientID == clientId).ToList();
                    }
                }
                else
                {
                    if (providerId != null)
                    {
                        invoiceDistributionIndexData.ProviderID = (int)providerId;
                        invoiceDistributionIndexData.Invoices = _unitOfWork.InvoiceRepository.GetAllInvoicesIncludingClientAndProviderAndInvoiceTypeAndServices().Where(i => i.DiscountMonth.Month == ((DateTime)discountMonth).Month && i.ProviderID == providerId).ToList();
                    }
                    else
                    {
                        invoiceDistributionIndexData.Invoices = _unitOfWork.InvoiceRepository.GetAllInvoicesIncludingClientAndProviderAndInvoiceTypeAndServices().Where(i => i.DiscountMonth.Month == ((DateTime)discountMonth).Month).ToList();
                    }
                }
            }
            else
            {
                if (clientId != null)
                {
                    invoiceDistributionIndexData.ClientID = (int)clientId;
                    if (providerId != null)
                    {
                        invoiceDistributionIndexData.ProviderID = (int)providerId;
                        invoiceDistributionIndexData.Invoices = _unitOfWork.InvoiceRepository.GetAllInvoicesIncludingClientAndProviderAndInvoiceTypeAndServices().Where(i => i.ClientID == clientId && i.ProviderID == providerId).ToList();
                    }
                    else
                    {
                        invoiceDistributionIndexData.Invoices = _unitOfWork.InvoiceRepository.GetAllInvoicesIncludingClientAndProviderAndInvoiceTypeAndServices().Where(i => i.ClientID == clientId).ToList();
                    }
                }
                else
                {
                    if (providerId != null)
                    {
                        invoiceDistributionIndexData.ProviderID = (int) providerId;
                        invoiceDistributionIndexData.Invoices = _unitOfWork.InvoiceRepository.GetAllInvoicesIncludingClientAndProviderAndInvoiceTypeAndServices().Where(i => i.ProviderID == providerId).ToList();
                    }
                    else
                    {
                        invoiceDistributionIndexData.Invoices = _unitOfWork.InvoiceRepository.GetAllInvoicesIncludingClientAndProviderAndInvoiceTypeAndServices().ToList();
                    }
                }
            }
            PopulateClientsDropDownList();
            PopulateProvidersDropDownList();
            return View(invoiceDistributionIndexData);
        }

        private void PopulateClientsDropDownList(object selectedClient = null)
        {
            var clientsQuery = from c in _unitOfWork.ClientRepository.GetAll().ToList() select c;
            ViewBag.ClientID = new SelectList(clientsQuery, "ID", "Name", selectedClient);
        }

        private void PopulateProvidersDropDownList(object selectedProvider = null)
        {
            var providersQuery = from p in _unitOfWork.ProviderRepository.GetAll().ToList() select p;
            ViewBag.ProviderID = new SelectList(providersQuery, "ID", "Name", selectedProvider);
        }
    }
}