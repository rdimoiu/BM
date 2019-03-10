﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
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
            ViewBag.PaidDateSortParm = sortOrder == "PaidDate" ? "paidDate_desc" : "PaidDate";
            ViewBag.QuantitySortParm = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            ViewBag.CheckQuantitySortParm = sortOrder == "CheckQuantity" ? "checkQuantity_desc" : "CheckQuantity";
            ViewBag.TotalValueWithoutTVASortParm = sortOrder == "TotalValueWithoutTVA" ? "totalValueWithoutTVA_desc" : "TotalValueWithoutTVA";
            ViewBag.CheckTotalValueWithoutTVASortParm = sortOrder == "CheckTotalValueWithoutTVA" ? "checkTotalValueWithoutTVA_desc" : "CheckTotalValueWithoutTVA";
            ViewBag.TotalTVASortParm = sortOrder == "TotalTVA" ? "totalTVA_desc" : "TotalTVA";
            ViewBag.CheckTotalTVASortParm = sortOrder == "CheckTotalTVA" ? "checkTotalTVA_desc" : "CheckTotalTVA";
            ViewBag.DiscountMonthSortParm = sortOrder == "DiscountMonth" ? "discountMonth_desc" : "DiscountMonth";
            ViewBag.DateTimeNow = DateTime.Now;
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
        public ActionResult Create(string discountMonth, int? clientId, int? providerId)
        {
            var invoice = new Invoice();
            if (discountMonth != null)
            {
                invoice.DiscountMonth = DateTime.ParseExact(discountMonth, "MM/dd/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
            }
            if (clientId != null)
            {
                invoice.ClientID = (int) clientId;
            }
            if (providerId != null)
            {
                invoice.ProviderID = (int) providerId;
            }
            if (Request.UrlReferrer != null)
            {
                invoice.PreviousPage = Request.UrlReferrer.AbsolutePath;
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
                    PopulateInvoiceTypesDropDownList(invoice.InvoiceTypeID);
                    PopulateClientsDropDownList(invoice.ClientID);
                    PopulateProvidersDropDownList(invoice.ProviderID);
                    return new HttpStatusCodeResult(409, "An invoice with this number, on this date, from this provider, already exists.");
                }
                try
                {
                    _unitOfWork.InvoiceRepository.Add(invoice);
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Invoice {0} has been created.", invoice.Number);
                    return Json(invoice.ID);
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
            if (Request.UrlReferrer != null)
            {
                invoice.PreviousPage = Request.UrlReferrer.AbsolutePath;
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
            if (TryUpdateModel(invoiceToUpdate, "", new[]{"Number", "CheckTotalValueWithoutTVA", "CheckTotalTVA", "Date", "DueDate", "CheckQuantity", "DiscountMonth", "InvoiceTypeID", "ProviderID", "ClientID"}))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateInvoice = _unitOfWork.InvoiceRepository.SingleOrDefault(i => i.Number == invoiceToUpdate.Number && i.Date == invoiceToUpdate.Date && i.ProviderID == invoiceToUpdate.ProviderID);
                    if (duplicateInvoice != null && duplicateInvoice.ID != invoiceToUpdate.ID)
                    {
                        PopulateInvoiceTypesDropDownList(invoiceToUpdate.InvoiceTypeID);
                        PopulateClientsDropDownList(invoiceToUpdate.ClientID);
                        PopulateProvidersDropDownList(invoiceToUpdate.ProviderID);
                        return new HttpStatusCodeResult(409, "An invoice with this number, on this date, from this provider, already exists.");
                    }
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
            if (Request.UrlReferrer != null)
            {
                invoice.PreviousPage = Request.UrlReferrer.AbsolutePath;
            }
            return View(invoice);
        }

        // POST: Invoice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string PreviousPage)
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
            if (PreviousPage.Equals("/Invoice/Index"))
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "InvoiceDistribution");
        }

        // POST: Invoice/Close/5
        public ActionResult Close(int id)
        {
            var invoice = _unitOfWork.InvoiceRepository.GetInvoiceIncludingServices(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            var totalCost = 0.0m;
            foreach (var service in invoice.Services)
            {
                //tbd
            }
            invoice.Closed = true;
            invoice.PaidDate = DateTime.Now;
            if (totalCost == invoice.TotalValueWithoutTVA + invoice.TotalTVA)
            {
                try
                {
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Invoice {0} has been closed.", invoice.Number);
                }
                catch (DataException)
                {
                    TempData["message"] = string.Format("Unexpected error occurred. Invoice {0} can not be closed.", invoice.Number);
                    return RedirectToAction("Close", new { id, saveChangesError = true });
                }
            }
            else
            {
                TempData["message"] = string.Format("Invoice {0} can not be closed. Invoice value {1} is different from total costs value {2}.", invoice.Number, invoice.TotalValueWithoutTVA + invoice.TotalTVA, totalCost);
            }
            if (Request.UrlReferrer.AbsolutePath.Equals("/Invoice/Index"))
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "InvoiceDistribution");
        }

        // POST: Invoice/Open/5
        public ActionResult Open(int id)
        {
            var invoice = _unitOfWork.InvoiceRepository.Get(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            invoice.PaidDate = invoice.Date.Subtract(TimeSpan.FromDays(1));
            invoice.Closed = false;
            try
            {
                _unitOfWork.Save();
                TempData["message"] = string.Format("Invoice {0} has been opened.", invoice.Number);
            }
            catch (DataException)
            {
                return RedirectToAction("Open", new { id, saveChangesError = true });
            }
            if (Request.UrlReferrer.AbsolutePath.Equals("/Invoice/Index"))
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "InvoiceDistribution");
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
            var invoiceTypesQuery = _unitOfWork.InvoiceTypeRepository.GetAll();
            ViewBag.InvoiceTypeID = new SelectList(invoiceTypesQuery, "ID", "Type", selectedInvoiceType);
        }

        private void PopulateClientsDropDownList(object selectedClient = null)
        {
            var clientsQuery = _unitOfWork.ClientRepository.GetAll();
            ViewBag.ClientID = new SelectList(clientsQuery, "ID", "Name", selectedClient);
        }

        private void PopulateProvidersDropDownList(object selectedProvider = null)
        {
            var providersQuery = _unitOfWork.ProviderRepository.GetAll();
            ViewBag.ProviderID = new SelectList(providersQuery, "ID", "Name", selectedProvider);
        }
    }
}
