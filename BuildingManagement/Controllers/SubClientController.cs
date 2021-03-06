﻿using BuildingManagement.DAL;
using BuildingManagement.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using X.PagedList;

namespace BuildingManagement.Controllers
{
    public class SubClientController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubClientController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: SubClient
        public ActionResult Index(int? page, string currentFilter, string searchString, string sortOrder)
        {
            IEnumerable<SubClient> subClients;
            var pageNumber = page ?? 1;
            const int pageSize = 3;
            if (searchString != null)
            {
                pageNumber = 1;
                subClients = _unitOfWork.SubClientRepository.GetFilteredSubClientsIncludingClient(searchString).ToList();
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    subClients = _unitOfWork.SubClientRepository.GetFilteredSubClientsIncludingClient(searchString).ToList();
                }
                else
                {
                    subClients = _unitOfWork.SubClientRepository.GetAllSubClientsIncludingClient().ToList();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PhoneSortParm = sortOrder == "Phone" ? "phone_desc" : "Phone";
            ViewBag.CountrySortParm = sortOrder == "Country" ? "country_desc" : "Country";
            ViewBag.StateSortParm = sortOrder == "State" ? "state_desc" : "State";
            ViewBag.CitySortParm = sortOrder == "City" ? "city_desc" : "City";
            ViewBag.StreetSortParm = sortOrder == "Street" ? "street_desc" : "Street";
            ViewBag.ContactSortParm = sortOrder == "Contact" ? "contact_desc" : "Contact";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";
            ViewBag.IBANSortParm = sortOrder == "IBAN" ? "iban_desc" : "IBAN";
            ViewBag.BankSortParm = sortOrder == "Bank" ? "bank_desc" : "Bank";
            ViewBag.CNPSortParm = sortOrder == "CNP" ? "cnp_desc" : "CNP";
            ViewBag.FiscalCodeSortParm = sortOrder == "FiscalCode" ? "fiscalCode_desc" : "FiscalCode";
            ViewBag.ClientSortParm = sortOrder == "Client" ? "client_desc" : "Client";
            subClients = _unitOfWork.SubClientRepository.OrderSubClients(subClients, sortOrder).ToList();
            ViewBag.OnePageOfSubClients = subClients.ToPagedList(pageNumber, pageSize);
            return View(ViewBag.OnePageOfSubClients);
        }

        // GET: SubClient/Details/5
        public ActionResult Details(int id)
        {
            var subClient = _unitOfWork.SubClientRepository.Get(id);
            if (subClient == null)
            {
                return HttpNotFound();
            }
            return View(subClient);
        }

        // GET: SubClient/Create
        public ActionResult Create()
        {
            var subClient = new SubClient();
            PopulateClientsDropDownList();
            return View(subClient);
        }

        // POST: SubClient/Create
        [HttpPost]
        public ActionResult Create(SubClient subClient)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                var duplicateSubClient = _unitOfWork.SubClientRepository.FirstOrDefault(sc => sc.CNP == subClient.CNP || sc.FiscalCode == subClient.FiscalCode);
                if (duplicateSubClient != null)
                {
                    PopulateClientsDropDownList(subClient.ClientID);
                    return new HttpStatusCodeResult(409, "A sub client with this CNP or FiscalCode already exists.");
                }
                try
                {
                    _unitOfWork.SubClientRepository.Add(subClient);
                    _unitOfWork.Save();
                    TempData["message"] = $"Sub client {subClient.Name} has been created.";
                    return Json(subClient.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateClientsDropDownList(subClient.ClientID);
            return View(subClient);
        }

        // GET: Client/Edit/5
        public ActionResult Edit(int id)
        {
            var subClient = _unitOfWork.SubClientRepository.Get(id);
            if (subClient == null)
            {
                return HttpNotFound();
            }
            PopulateClientsDropDownList(subClient.ClientID);
            return View(subClient);
        }

        // POST: SubClient/Edit/5
        [HttpPost]
        public ActionResult Edit(SubClient subClient)
        {
            var subClientToUpdate = _unitOfWork.SubClientRepository.Get(subClient.ID);
            if (subClientToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(subClientToUpdate, "", new[] { "Name", "Phone", "Country", "State", "City", "Street", "Contact", "Email", "IBAN", "Bank", "CNP", "FiscalCode", "ClientID" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateSubClient = _unitOfWork.SubClientRepository.FirstOrDefault(sc => sc.ID != subClientToUpdate.ID && sc.CNP == subClientToUpdate.CNP || sc.FiscalCode == subClientToUpdate.FiscalCode);
                    if (duplicateSubClient != null)
                    {
                        PopulateClientsDropDownList(subClientToUpdate.ClientID);
                        return new HttpStatusCodeResult(409, "A sub client with this CNP or FiscalCode already exists.");
                    }
                    _unitOfWork.Save();
                    TempData["message"] = $"Sub client {subClientToUpdate.Name} has been edited.";
                    return Json(subClientToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateClientsDropDownList(subClientToUpdate.ClientID);
            return View(subClientToUpdate);
        }

        // GET: SubClient/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var subClient = _unitOfWork.SubClientRepository.GetSubClientIncludingClient(id);
            if (subClient == null)
            {
                return HttpNotFound();
            }
            return View(subClient);
        }

        // POST: SubClient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var subClient = _unitOfWork.SubClientRepository.Get(id);
                if (subClient == null)
                {
                    return HttpNotFound();
                }
                _unitOfWork.SubClientRepository.Remove(subClient);
                _unitOfWork.Save();
                TempData["message"] = $"Sub client {subClient.Name} has been deleted.";
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new { id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        private void PopulateClientsDropDownList(object selectedClient = null)
        {
            var clientsQuery = _unitOfWork.ClientRepository.GetAll().ToList();
            ViewBag.ClientID = new SelectList(clientsQuery, "ID", "Name", selectedClient);
        }
    }
}
