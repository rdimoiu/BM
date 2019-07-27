using BuildingManagement.DAL;
using BuildingManagement.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using X.PagedList;

namespace BuildingManagement.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClientController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Client
        public ActionResult Index(int? page, string currentFilter, string searchString, string sortOrder)
        {
            IEnumerable<Client> clients;
            var pageNumber = page ?? 1;
            const int pageSize = 3;
            if (searchString != null)
            {
                pageNumber = 1;
                clients = _unitOfWork.ClientRepository.GetFilteredClients(searchString).ToList();
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    clients = _unitOfWork.ClientRepository.GetFilteredClients(searchString).ToList();
                }
                else
                {
                    clients = _unitOfWork.ClientRepository.GetAll().ToList();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PhoneSortParm = sortOrder == "Phone" ? "phone_desc" : "Phone";
            ViewBag.AddressSortParm = sortOrder == "Address" ? "address_desc" : "Address";
            ViewBag.ContactSortParm = sortOrder == "Contact" ? "contact_desc" : "Contact";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";
            clients = _unitOfWork.ClientRepository.OrderClients(clients, sortOrder);
            ViewBag.OnePageOfClients = clients.ToPagedList(pageNumber, pageSize);
            return View(ViewBag.OnePageOfClients);
        }

        // GET: Client/Details/5
        public ActionResult Details(int id)
        {
            var client = _unitOfWork.ClientRepository.Get(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // GET: Client/Create
        public ActionResult Create()
        {
            var client = new Client();
            return View(client);
        }

        // POST: Client/Create
        [HttpPost]
        public ActionResult Create(Client client)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                var duplicateClient = _unitOfWork.ClientRepository.FirstOrDefault(c => c.Name == client.Name);
                if (duplicateClient != null)
                {
                    return new HttpStatusCodeResult(409, "A client with this name already exists.");
                }
                try
                {
                    _unitOfWork.ClientRepository.Add(client);
                    _unitOfWork.Save();
                    TempData["message"] = $"Client {client.Name} has been created.";
                    return Json(client.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(client);
        }

        // GET: Client/Edit/5
        public ActionResult Edit(int id)
        {
            var client = _unitOfWork.ClientRepository.Get(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Client/Edit/5
        [HttpPost]
        public ActionResult Edit(Client client)
        {
            var clientToUpdate = _unitOfWork.ClientRepository.Get(client.ID);
            if (clientToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(clientToUpdate, "", new[] { "Name", "Phone", "Address", "Contact", "Email" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateClient = _unitOfWork.ClientRepository.FirstOrDefault(c => c.ID != clientToUpdate.ID && c.Name == clientToUpdate.Name);
                    if (duplicateClient != null)
                    {
                        return new HttpStatusCodeResult(409, "A client with this name already exists.");
                    }
                    _unitOfWork.Save();
                    TempData["message"] = $"Client {clientToUpdate.Name} has been edited.";
                    return Json(clientToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(clientToUpdate);
        }

        // GET: Client/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var client = _unitOfWork.ClientRepository.Get(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var client = _unitOfWork.ClientRepository.Get(id);
                if (client == null)
                {
                    return HttpNotFound();
                }
                _unitOfWork.ClientRepository.Remove(client);
                _unitOfWork.Save();
                TempData["message"] = $"Client {client.Name} has been deleted.";
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
