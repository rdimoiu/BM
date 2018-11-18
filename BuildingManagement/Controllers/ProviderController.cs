using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using X.PagedList;

namespace BuildingManagement.Controllers
{
    public class ProviderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProviderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Provider
        public ActionResult Index(int? page, string currentFilter, string searchString, string sortOrder)
        {
            IEnumerable<Provider> providers;
            var pageNumber = page ?? 1;
            const int pageSize = 3;
            if (searchString != null)
            {
                pageNumber = 1;
                providers = _unitOfWork.ProviderRepository.GetFilteredProviders(searchString);
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    providers = _unitOfWork.ProviderRepository.GetFilteredProviders(searchString);
                }
                else
                {
                    providers = _unitOfWork.ProviderRepository.GetAll();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.FiscalCodeSortParm = sortOrder == "FiscalCode" ? "fiscalCode_desc" : "FiscalCode";
            ViewBag.TradeRegisterSortParm = sortOrder == "TradeRegister" ? "tradeRegister_desc" : "TradeRegister";
            ViewBag.AddressSortParm = sortOrder == "Address" ? "address_desc" : "Address";
            ViewBag.PhoneSortParm = sortOrder == "Phone" ? "phone_desc" : "Phone";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";
            ViewBag.BankAccountSortParm = sortOrder == "BankAccount" ? "bankAccount_desc" : "BankAccount";
            ViewBag.BankSortParm = sortOrder == "Bank" ? "bank_desc" : "Bank";
            ViewBag.TVAPayerSortParm = sortOrder == "TVAPayer" ? "tvaPayer_desc" : "TVAPayer";
            providers = _unitOfWork.ProviderRepository.OrderProviders(providers, sortOrder);
            ViewBag.OnePageOfProviders = providers.ToPagedList(pageNumber, pageSize);
            return View(ViewBag.OnePageOfProviders);
        }

        // GET: Provider/Details/5
        public ActionResult Details(int id)
        {
            var provider = _unitOfWork.ProviderRepository.Get(id);
            if (provider == null)
            {
                return HttpNotFound();
            }
            return View(provider);
        }

        // GET: Provider/Create
        public ActionResult Create()
        {
            Provider provider = new Provider();
            return View(provider);
        }

        // POST: Provider/Create
        [HttpPost]
        public ActionResult Create(Provider provider)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                var duplicateProvider = _unitOfWork.ProviderRepository.SingleOrDefault(p => p.FiscalCode == provider.FiscalCode);
                if (duplicateProvider != null)
                {
                    return new HttpStatusCodeResult(409, "A provider with this fiscal code already exists.");
                }
                try
                {
                    _unitOfWork.ProviderRepository.Add(provider);
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Provider {0} has been created.", provider.Name);
                    return Json(provider.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(provider);
        }

        // GET: Provider/Edit/5
        public ActionResult Edit(int id)
        {
            var provider = _unitOfWork.ProviderRepository.Get(id);
            if (provider == null)
            {
                return HttpNotFound();
            }
            return View(provider);
        }

        // POST: Provider/Edit/5
        [HttpPost]
        public ActionResult Edit(Provider provider)
        {
            var providerToUpdate = _unitOfWork.ProviderRepository.Get(provider.ID);
            if (providerToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(providerToUpdate, "", new[] { "Name", "FiscalCode", "TradeRegister", "Address", "Phone", "Email", "BankAccount", "Bank", "TVAPayer" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateProvider = _unitOfWork.ProviderRepository.SingleOrDefault(p => p.FiscalCode == providerToUpdate.FiscalCode);
                    if (duplicateProvider != null && duplicateProvider.ID != providerToUpdate.ID)
                    {
                        return new HttpStatusCodeResult(409, "A provider with this fiscal code already exists.");
                    }
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Provider {0} has been edited.", providerToUpdate.Name);
                    return Json(providerToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(providerToUpdate);
        }

        // GET: Provider/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var provider = _unitOfWork.ProviderRepository.Get(id);
            if (provider == null)
            {
                return HttpNotFound();
            }
            return View(provider);
        }

        // POST: Provider/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var provider = _unitOfWork.ProviderRepository.Get(id);
                if (provider == null)
                {
                    return HttpNotFound();
                }
                _unitOfWork.ProviderRepository.Remove(provider);
                _unitOfWork.Save();
                TempData["message"] = string.Format("Provider {0} has been deleted.", provider.Name);
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
    }
}
