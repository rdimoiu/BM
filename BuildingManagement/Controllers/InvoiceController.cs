using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using BuildingManagement.ViewModels;

namespace BuildingManagement.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        // GET: Invoice
        public ActionResult Index()
        {
            var viewModel = new InvoiceIndexData
            {
                Invoices = _unitOfWork.InvoiceRepository.Get(includeProperties: "Provider, Client, Services")
            };
            return View(viewModel);
        }

        // GET: Invoice/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var invoice = _unitOfWork.InvoiceRepository.GetById(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // GET: Invoice/Create
        public ActionResult Create(int? clientId, int? providerId)
        {
            var model = new Invoice();
            if (clientId != null)
            {
                model.ClientID = (int) clientId;
            }
            if (providerId != null)
            {
                model.ProviderID = (int) providerId;
            }
            if (Request.UrlReferrer != null && Request.UrlReferrer.AbsoluteUri.Contains("Distribution"))
            {
                model.PreviousPage = "InvoiceDistribution";
            }
            else
            {
                model.PreviousPage = "Invoice";
            }
            PopulateInvoiceTypesDropDownList();
            PopulateClientsDropDownList();
            PopulateProvidersDropDownList();
            return View(model);
        }

        // POST: Invoice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(
                Include =
                    "Number,CheckTotalValueWithoutTVA,CheckTotalTVA,Date,DueDate,CheckQuantity,Unit,DiscountMonth,InvoiceTypeID,ProviderID,ClientID,PreviousPage"
                )] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //uniqueness condition check
                    var duplicateInvoice =
                        _unitOfWork.InvoiceRepository.Get(
                            filter:
                                i =>
                                    i.Number == invoice.Number && i.Date == invoice.Date &&
                                    i.ProviderID == invoice.ProviderID).FirstOrDefault();
                    if (duplicateInvoice != null)
                    {
                        ModelState.AddModelError("Number",
                            "An invoice with this number, on this date, from this provider, already exists.");
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
                    _unitOfWork.InvoiceRepository.Insert(invoice);
                    _unitOfWork.Save();
                    if (invoice.PreviousPage.Equals("Invoice"))
                    {
                        return RedirectToAction("Index", "Invoice");
                    }
                    return RedirectToAction("Index", "InvoiceDistribution", new {invoice.ClientID, invoice.ProviderID});
                }
                catch (DataException)
                {
                    ModelState.AddModelError("",
                        "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateInvoiceTypesDropDownList(invoice.InvoiceTypeID);
            PopulateClientsDropDownList(invoice.ClientID);
            PopulateProvidersDropDownList(invoice.ProviderID);
            return View(invoice);
        }

        // GET: Invoice/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var invoice = _unitOfWork.InvoiceRepository.GetById(id);
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var invoiceToUpdate = _unitOfWork.InvoiceRepository.GetById(id);
            if (invoiceToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(invoiceToUpdate, "",
                new[]
                {
                    "Number", "CheckTotalValueWithoutTVA", "CheckTotalTVA", "Date", "DueDate", "CheckQuantity", "Unit", "DiscountMonth",
                    "InvoiceTypeID", "ProviderID", "ClientID"
                }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateInvoice =
                        _unitOfWork.InvoiceRepository.Get(
                            filter:
                                i =>
                                    i.Number == invoiceToUpdate.Number && i.Date == invoiceToUpdate.Date &&
                                    i.ProviderID == invoiceToUpdate.ProviderID).FirstOrDefault();
                    if (duplicateInvoice != null && duplicateInvoice.ID != invoiceToUpdate.ID)
                    {
                        ModelState.AddModelError("Number",
                            "An invoice with this number, on this date, from this provider, already exists.");
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
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("",
                        "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateInvoiceTypesDropDownList(invoiceToUpdate.InvoiceTypeID);
            PopulateClientsDropDownList(invoiceToUpdate.ClientID);
            PopulateProvidersDropDownList(invoiceToUpdate.ProviderID);
            return View(invoiceToUpdate);
        }

        // GET: Invoice/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage =
                    "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var invoice =
                _unitOfWork.InvoiceRepository.Get(includeProperties: "Client, Provider").Single(s => s.ID == id);
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
                _unitOfWork.InvoiceRepository.Delete(id);
                _unitOfWork.Save();
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new {id, saveChangesError = true});
            }
            return RedirectToAction("Index");
        }

        
        public ActionResult Close(int? invoiceId)
        {
            if (invoiceId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //if (saveChangesError.GetValueOrDefault())
            //{
            //    ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            //}
            var invoice =
                _unitOfWork.InvoiceRepository.Get(includeProperties: "Client, Provider, Services")
                    .Single(s => s.ID == invoiceId);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            
            var checkTotalValueWithoutTVA = invoice.Services.Sum(service => service.ValueWithoutTVA);
            if (checkTotalValueWithoutTVA != invoice.CheckTotalValueWithoutTVA)
            {
                ModelState.AddModelError("", "Sum of values without TVA from services is wrong.");
                return RedirectToAction("Index", "InvoiceDistribution",
                    new {discountMonth = invoice.DiscountMonth, clientId = invoice.ClientID, providerId = invoice.ProviderID});
            }
            
            foreach (var service in invoice.Services)
            {
                List<Space> spaces = new List<Space>();
                List<Level> levels = new List<Level>();
                foreach (var section in service.Sections)
                {
                    var sectionLevels = _unitOfWork.LevelRepository.Get().Where(l => l.SectionID == section.ID).ToList();
                    foreach (var level in sectionLevels)
                    {
                        levels.Add(level);
                    }
                }
                levels.AddRange(service.Levels);
                foreach (var level in levels)
                {
                    var levelSpaces = _unitOfWork.SpaceRepository.Get().Where(s => s.LevelID == level.ID).ToList();
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
                                cost.Value = quota * valueWithTVA;
                                cost.ServiceID = service.ID;
                                cost.SpaceID = space.ID;
                                _unitOfWork.CostRepository.Insert(cost);
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
                                cost.Value = cota * valueWithTVA;
                                cost.ServiceID = service.ID;
                                cost.SpaceID = space.ID;
                                _unitOfWork.CostRepository.Insert(cost);
                            }
                        }
                    }
                }
            }
            invoice.Closed = true;
            _unitOfWork.Save();
            return View(invoice);
        }

        public ActionResult Open(int? invoiceId)
        {
            if (invoiceId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var invoice = _unitOfWork.InvoiceRepository.GetById(invoiceId);
            foreach (var service in invoice.Services)
            {
                var costs = _unitOfWork.CostRepository.Get().Where(c => c.ServiceID == service.ID);
                foreach (var cost in costs)
                {
                    _unitOfWork.CostRepository.Delete(cost.ID);
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
            var invoiceTypesQuery = from it in _unitOfWork.InvoiceTypeRepository.Get() select it;
            ViewBag.InvoiceTypeID = new SelectList(invoiceTypesQuery, "ID", "Type", selectedInvoiceType);
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
