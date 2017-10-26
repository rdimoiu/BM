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
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        // GET: InvoiceDistribution
        public ActionResult Index(DateTime? discountMonth, int? clientId, int? providerId)
        {
            InvoiceDistributionIndexData invoiceDistributionIndexData = new InvoiceDistributionIndexData
            {
                DiscountMonth = DateTime.Now,
                Client = new Client(),
                Provider = new Provider(),
                Invoices = new List<Invoice>(),
                Services = new List<Service>()
            };

            if (discountMonth != null)
            {
                invoiceDistributionIndexData.DiscountMonth = (DateTime) discountMonth;
                if (clientId != null)
                {
                    invoiceDistributionIndexData.ClientID = (int) clientId;
                    if (providerId != null)
                    {
                        invoiceDistributionIndexData.ProviderID = (int) providerId;
                        invoiceDistributionIndexData.Invoices =
                            _unitOfWork.InvoiceRepository.Get()
                                .Where(i => i.DiscountMonth.Month == ((DateTime)discountMonth).Month && i.ClientID == clientId && i.ProviderID == providerId);
                    }
                    else
                    {
                        invoiceDistributionIndexData.Invoices =
                            _unitOfWork.InvoiceRepository.Get().Where(i => i.DiscountMonth.Month == ((DateTime)discountMonth).Month && i.ClientID == clientId);
                    }
                }
                else
                {
                    if (providerId != null)
                    {
                        invoiceDistributionIndexData.ProviderID = (int) providerId;
                        invoiceDistributionIndexData.Invoices =
                            _unitOfWork.InvoiceRepository.Get()
                                .Where(
                                    i =>
                                        i.DiscountMonth.Month == ((DateTime) discountMonth).Month &&
                                        i.ProviderID == providerId);
                    }
                    else
                    {
                        invoiceDistributionIndexData.Invoices = _unitOfWork.InvoiceRepository.Get().Where(i => i.DiscountMonth.Month == ((DateTime)discountMonth).Month);
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
                        invoiceDistributionIndexData.Invoices =
                            _unitOfWork.InvoiceRepository.Get()
                                .Where(i => i.ClientID == clientId && i.ProviderID == providerId);
                    }
                    else
                    {
                        invoiceDistributionIndexData.Invoices =
                            _unitOfWork.InvoiceRepository.Get().Where(i => i.ClientID == clientId);
                    }
                }
                else
                {
                    if (providerId != null)
                    {
                        invoiceDistributionIndexData.ProviderID = (int)providerId;
                        invoiceDistributionIndexData.Invoices =
                            _unitOfWork.InvoiceRepository.Get().Where(i => i.ProviderID == providerId);
                    }
                }
            }

            PopulateClientsDropDownList();
            PopulateProvidersDropDownList();
            return View(invoiceDistributionIndexData);
        }

        private void PopulateClientsDropDownList(object selectedClient = null)
        {
            var clientsQuery = from c in _unitOfWork.ClientRepository.Get() select c;
            ViewBag.ClientID = new SelectList(clientsQuery, "ID", "Name", selectedClient);
        }

        private void PopulateProvidersDropDownList(object selectedProvider = null)
        {
            var providersQuery = from p in _unitOfWork.ProviderRepository.Get() select p;
            ViewBag.ProviderID = new SelectList(providersQuery, "ID", "Name", selectedProvider);
        }
    }
}