using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
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
                subClients = _unitOfWork.SubClientRepository.GetFilteredSubClientsIncludingClient(searchString);
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    subClients = _unitOfWork.SubClientRepository.GetFilteredSubClientsIncludingClient(searchString);
                }
                else
                {
                    subClients = _unitOfWork.SubClientRepository.GetAllSubClientsIncludingClient();
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
            subClients = _unitOfWork.SubClientRepository.OrderSubClients(subClients, sortOrder);
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
            PopulateClientsDropDownList();
            return View();
        }

        // POST: SubClient/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Phone,Country,State,City,Street,Contact,Email,IBAN,Bank,CNP,FiscalCode,ClientID")] SubClient subClient)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(subClient.CNP) || string.IsNullOrEmpty(subClient.FiscalCode))
                    {
                        ModelState.AddModelError("", "A CNP or FiscalCode is required.");
                        ModelState.AddModelError("FiscalCode", "A CNP or FiscalCode is required.");
                        PopulateClientsDropDownList(subClient.ClientID);
                        return View("Create");
                    }
                    //uniqueness condition check
                    var duplicateSubClient = _unitOfWork.SubClientRepository.SingleOrDefault(sc => sc.CNP == subClient.CNP || sc.FiscalCode == subClient.FiscalCode);
                    if (duplicateSubClient != null)
                    {
                        ModelState.AddModelError("FiscalCode", "A sub client with this CNP or FiscalCode already exists.");
                        PopulateClientsDropDownList(subClient.ClientID);
                        return View("Create");
                    }
                    _unitOfWork.SubClientRepository.Add(subClient);
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Sub client {0} has been created.", subClient.Name);
                    return RedirectToAction("Index");
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int id)
        {
            var subClient = _unitOfWork.SubClientRepository.Get(id);
            if (subClient == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(subClient, "", new[] { "Name", "Phone", "Country", "State", "City", "Street", "Contact", "Email", "IBAN", "Bank", "CNP", "FiscalCode", "ClientID" }))
            {
                try
                {
                    if (string.IsNullOrEmpty(subClient.CNP) || string.IsNullOrEmpty(subClient.FiscalCode))
                    {
                        ModelState.AddModelError("CNP", "A CNP or FiscalCode is required.");
                        ModelState.AddModelError("FiscalCode", "A CNP or FiscalCode is required.");
                        PopulateClientsDropDownList(subClient.ClientID);
                        return View("Create");
                    }
                    //uniqueness condition check
                    var duplicateSubClient = _unitOfWork.SubClientRepository.SingleOrDefault(sc => sc.CNP == subClient.CNP || sc.FiscalCode == subClient.FiscalCode);
                    if (duplicateSubClient != null && duplicateSubClient.ID != subClient.ID)
                    {
                        ModelState.AddModelError("Code", "A sub client with this CNP or FiscalCode already exists.");
                        PopulateClientsDropDownList(subClient.ClientID);
                        return View("Create");
                    }
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Sub client {0} has been edited.", subClient.Name);
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateClientsDropDownList(subClient.ClientID);
            return View(subClient);
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
                TempData["message"] = string.Format("Sub client {0} has been deleted.", subClient.Name);
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
            var clientsQuery = from c in _unitOfWork.ClientRepository.GetAll() select c;
            ViewBag.ClientID = new SelectList(clientsQuery, "ID", "Name", selectedClient);
        }
    }
}
