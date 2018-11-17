using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using X.PagedList;

namespace BuildingManagement.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Invoice
        public ActionResult Index(int? page, string currentFilter, string searchString, string sortOrder)
        {
            IEnumerable<Invoice> invoices;
            var pageNumber = page ?? 1;
            const int pageSize = 3;
            if (searchString != null)
            {
                pageNumber = 1;
                invoices = _unitOfWork.InvoiceRepository.GetFilteredInvoicesIncludingClientAndProviderAndInvoiceTypeAndServices(searchString);
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    invoices = _unitOfWork.InvoiceRepository.GetFilteredInvoicesIncludingClientAndProviderAndInvoiceTypeAndServices(searchString);
                }
                else
                {
                    invoices = _unitOfWork.InvoiceRepository.GetAllInvoicesIncludingClientAndProviderAndInvoiceTypeAndServices();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClientSortParm = string.IsNullOrEmpty(sortOrder) ? "client_desc" : "";
            ViewBag.ProviderSortParm = sortOrder == "Provider" ? "provider_desc" : "Provider";
            ViewBag.InvoiceTypeSortParm = sortOrder == "InvoiceType" ? "invoiceType_desc" : "InvoiceType";
            ViewBag.NumberSortParm = sortOrder == "Number" ? "number_desc" : "Number";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.DueDateSortParm = sortOrder == "DueDate" ? "dueDate_desc" : "DueDate";
            ViewBag.QuantitySortParm = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            ViewBag.CheckQuantitySortParm = sortOrder == "CheckQuantity" ? "checkQuantity_desc" : "CheckQuantity";
            ViewBag.TotalValueWithoutTVASortParm = sortOrder == "TotalValueWithoutTVA" ? "totalValueWithoutTVA_desc" : "TotalValueWithoutTVA";
            ViewBag.CheckTotalValueWithoutTVASortParm = sortOrder == "CheckTotalValueWithoutTVA" ? "checkTotalValueWithoutTVA_desc" : "CheckTotalValueWithoutTVA";
            ViewBag.TotalTVASortParm = sortOrder == "TotalTVA" ? "totalTVA_desc" : "TotalTVA";
            ViewBag.CheckTotalTVASortParm = sortOrder == "CheckTotalTVA" ? "checkTotalTVA_desc" : "CheckTotalTVA";
            ViewBag.DiscountMonthSortParm = sortOrder == "DiscountMonth" ? "discountMonth_desc" : "DiscountMonth";
            invoices = _unitOfWork.InvoiceRepository.OrderInvoices(invoices, sortOrder);
            ViewBag.OnePageOfInvoices = invoices.ToPagedList(pageNumber, pageSize);
            return View(ViewBag.OnePageOfInvoices);
        }

        // GET: Invoice/Details/5
        public ActionResult Details(int id)
        {
            var invoice = _unitOfWork.InvoiceRepository.Get(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // GET: Invoice/Create
        public ActionResult Create(int? clientId, int? providerId)
        {
            var invoice = new Invoice();
            if (clientId != null)
            {
                invoice.ClientID = (int) clientId;
            }
            if (providerId != null)
            {
                invoice.ProviderID = (int) providerId;
            }
            if (Request.UrlReferrer != null && Request.UrlReferrer.AbsoluteUri.Contains("Distribution"))
            {
                invoice.PreviousPage = "InvoiceDistribution";
            }
            else
            {
                invoice.PreviousPage = "Invoice";
            }
            PopulateInvoiceTypesDropDownList();
            PopulateClientsDropDownList();
            PopulateProvidersDropDownList();
            return View(invoice);
        }

        // POST: Invoice/Create
        [HttpPost]
        public ActionResult Create(Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                var duplicateInvoice = _unitOfWork.InvoiceRepository.SingleOrDefault(i => i.Number == invoice.Number && i.Date == invoice.Date && i.ProviderID == invoice.ProviderID);
                if (duplicateInvoice != null)
                {
                    ModelState.AddModelError("Number", "An invoice with this number, on this date, from this provider, already exists.");
                    PopulateInvoiceTypesDropDownList(invoice.InvoiceTypeID);
                    PopulateClientsDropDownList(invoice.ClientID);
                    PopulateProvidersDropDownList(invoice.ProviderID);
                    return View(invoice);
                }
                if (invoice.Date > invoice.DueDate)
                {
                    ModelState.AddModelError("DueDate", "DueDate must be greater than Date.");
                    PopulateInvoiceTypesDropDownList(invoice.InvoiceTypeID);
                    PopulateClientsDropDownList(invoice.ClientID);
                    PopulateProvidersDropDownList(invoice.ProviderID);
                    return View(invoice);
                }
                invoice.Quantity = 0.0m;
                invoice.TotalValueWithoutTVA = 0.0m;
                invoice.TotalTVA = 0.0m;
                try
                {
                    _unitOfWork.InvoiceRepository.Add(invoice);
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Invoice {0} has been created.", invoice.Number);
                    if (invoice.PreviousPage.Equals("Invoice"))
                    {
                        return Json(invoice.ID);
                    }
                    return RedirectToAction("Index", "InvoiceDistribution", new {invoice.ClientID, invoice.ProviderID});
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateInvoiceTypesDropDownList(invoice.InvoiceTypeID);
            PopulateClientsDropDownList(invoice.ClientID);
            PopulateProvidersDropDownList(invoice.ProviderID);
            return View(invoice);
        }

        // GET: Invoice/Edit/5
        public ActionResult Edit(int id)
        {
            var invoice = _unitOfWork.InvoiceRepository.Get(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            PopulateInvoiceTypesDropDownList(invoice.InvoiceTypeID);
            PopulateClientsDropDownList(invoice.ClientID);
            PopulateProvidersDropDownList(invoice.ProviderID);
            return View(invoice);
        }

        // POST: Invoice/Edit/5
        [HttpPost]
        public ActionResult Edit(Invoice invoice)
        {
            var invoiceToUpdate = _unitOfWork.InvoiceRepository.GetInvoiceIncludingClientAndProviderAndInvoiceTypeAndServices(invoice.ID);
            if (invoiceToUpdate == null)
            {
                return HttpNotFound();
            }
            //uniqueness condition check
            var duplicateInvoice = _unitOfWork.InvoiceRepository.SingleOrDefault(i => i.Number == invoiceToUpdate.Number && i.Date == invoiceToUpdate.Date && i.ProviderID == invoiceToUpdate.ProviderID);
            if (duplicateInvoice != null && duplicateInvoice.ID != invoiceToUpdate.ID)
            {
                ModelState.AddModelError("Number", "An invoice with this number, on this date, from this provider, already exists.");
                PopulateInvoiceTypesDropDownList(invoiceToUpdate.InvoiceTypeID);
                PopulateClientsDropDownList(invoiceToUpdate.ClientID);
                PopulateProvidersDropDownList(invoiceToUpdate.ProviderID);
                return View(invoiceToUpdate);
            }
            if (invoiceToUpdate.Date > invoiceToUpdate.DueDate)
            {
                ModelState.AddModelError("DueDate", "DueDate must be greater than Date.");
                PopulateInvoiceTypesDropDownList(invoiceToUpdate.InvoiceTypeID);
                PopulateClientsDropDownList(invoiceToUpdate.ClientID);
                PopulateProvidersDropDownList(invoiceToUpdate.ProviderID);
                return View(invoiceToUpdate);
            }
            if (TryUpdateModel(invoiceToUpdate, "", new[]{"Number", "CheckTotalValueWithoutTVA", "CheckTotalTVA", "Date", "DueDate", "CheckQuantity", "DiscountMonth", "InvoiceTypeID", "ProviderID", "ClientID"}))
            {
                try
                {
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Invoice {0} has been edited.", invoiceToUpdate.Number);
                    return Json(invoiceToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateInvoiceTypesDropDownList(invoiceToUpdate.InvoiceTypeID);
            PopulateClientsDropDownList(invoiceToUpdate.ClientID);
            PopulateProvidersDropDownList(invoiceToUpdate.ProviderID);
            return View(invoiceToUpdate);
        }

        // GET: Invoice/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var invoice = _unitOfWork.InvoiceRepository.GetInvoiceIncludingClientAndProviderAndInvoiceTypeAndServices(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // POST: Invoice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var invoice = _unitOfWork.InvoiceRepository.Get(id);
                if (invoice == null)
                {
                    return HttpNotFound();
                }
                _unitOfWork.InvoiceRepository.Remove(invoice);
                _unitOfWork.Save();
                TempData["message"] = string.Format("Invoice {0} has been deleted.", invoice.Number);
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new {id, saveChangesError = true});
            }
            return RedirectToAction("Index");
        }


        public ActionResult Close(int id)
        {
            var invoice = _unitOfWork.InvoiceRepository.SingleOrDefault(s => s.ID == id);
            if (invoice == null)
            {
                return HttpNotFound();
            }

            var checkTotalValueWithoutTVA = invoice.Services.Sum(service => service.ValueWithoutTVA);
            if (checkTotalValueWithoutTVA != invoice.CheckTotalValueWithoutTVA)
            {
                ModelState.AddModelError("", "Sum of values without TVA from services is wrong.");
                return RedirectToAction("Index", "InvoiceDistribution",
                    new
                    {
                        discountMonth = invoice.DiscountMonth,
                        clientId = invoice.ClientID,
                        providerId = invoice.ProviderID
                    });
            }

            foreach (var service in invoice.Services)
            {
                List<Space> spaces = new List<Space>();
                List<Level> levels = new List<Level>();
                foreach (var section in service.Sections)
                {
                    var sectionLevels = _unitOfWork.LevelRepository.GetLevelsBySection(section.ID).ToList();
                    foreach (var level in sectionLevels)
                    {
                        levels.Add(level);
                    }
                }
                levels.AddRange(service.Levels);
                foreach (var level in levels)
                {
                    var levelSpaces = _unitOfWork.SpaceRepository.GetSpacesByLevel(level.ID).ToList();
                    foreach (var space in levelSpaces)
                    {
                        spaces.Add(space);
                    }
                }
                spaces.AddRange(service.Spaces);
                decimal totalSurface = 0;
                decimal totalPeople = 0;
                foreach (var space in spaces)
                {
                    if (!space.Inhabited)
                    {
                        totalSurface = totalSurface + space.Surface;
                        totalPeople = totalPeople + space.People;
                    }
                }
                var valueWithTVA = service.TVA*service.QuotaTVA*service.ValueWithoutTVA;
                if (service.DistributionModeID == 1)
                {
                    if (totalSurface > 0)
                    {
                        foreach (var space in spaces)
                        {
                            var cost = new Cost();
                            if (!space.Inhabited)
                            {
                                var quota = space.Surface/totalSurface;
                                cost.Value = quota*valueWithTVA;
                                cost.ServiceID = service.ID;
                                cost.SpaceID = space.ID;
                                _unitOfWork.CostRepository.Add(cost);
                            }
                        }
                    }
                }
                else
                {
                    if (totalPeople > 0)
                    {
                        foreach (var space in spaces)
                        {
                            var cost = new Cost();
                            if (!space.Inhabited)
                            {
                                var cota = space.People/totalPeople;
                                cost.Value = cota*valueWithTVA;
                                cost.ServiceID = service.ID;
                                cost.SpaceID = space.ID;
                                _unitOfWork.CostRepository.Add(cost);
                            }
                        }
                    }
                }
            }
            invoice.Closed = true;
            _unitOfWork.Save();
            return View(invoice);
        }

        public ActionResult Open(int invoiceId)
        {
            var invoice = _unitOfWork.InvoiceRepository.Get(invoiceId);
            foreach (var service in invoice.Services)
            {
                var costs = _unitOfWork.CostRepository.GetCostsByService(service.ID);
                foreach (var cost in costs)
                {
                    _unitOfWork.CostRepository.Remove(cost);
                }
            }
            invoice.Closed = false;
            _unitOfWork.Save();
            return View(invoice);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        private void PopulateInvoiceTypesDropDownList(object selectedInvoiceType = null)
        {
            var invoiceTypesQuery = from it in _unitOfWork.InvoiceTypeRepository.GetAll() select it;
            ViewBag.InvoiceTypeID = new SelectList(invoiceTypesQuery, "ID", "Type", selectedInvoiceType);
        }

        private void PopulateClientsDropDownList(object selectedClient = null)
        {
            var clientsQuery = from c in _unitOfWork.ClientRepository.GetAll() select c;
            ViewBag.ClientID = new SelectList(clientsQuery, "ID", "Name", selectedClient);
        }

        private void PopulateProvidersDropDownList(object selectedProvider = null)
        {
            var providersQuery = from p in _unitOfWork.ProviderRepository.GetAll() select p;
            ViewBag.ProviderID = new SelectList(providersQuery, "ID", "Name", selectedProvider);
        }
    }
}
