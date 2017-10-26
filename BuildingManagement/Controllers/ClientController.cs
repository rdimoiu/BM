using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using PagedList;

namespace BuildingManagement.Controllers
{
    public class ClientController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        // GET: Client
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PhoneSortParm = sortOrder == "Phone" ? "phone_desc" : "Phone";
            ViewBag.AddressSortParm = sortOrder == "Address" ? "address_desc" : "Address";
            ViewBag.ContactSortParm = sortOrder == "Contact" ? "contact_desc" : "Contact";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var clients = _unitOfWork.ClientRepository.Get();
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.ToLower();
                clients =
                    clients.Where(
                        c =>
                            c.Name.ToLower().Contains(searchString) ||
                            c.Phone.ToLower().Contains(searchString) ||
                            c.Address.ToLower().Contains(searchString) ||
                            c.Contact.ToLower().Contains(searchString) ||
                            c.Email.ToLower().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    clients = clients.OrderByDescending(s => s.Name);
                    break;
                case "Phone":
                    clients = clients.OrderBy(s => s.Phone);
                    break;
                case "phone_desc":
                    clients = clients.OrderByDescending(s => s.Phone);
                    break;
                case "Address":
                    clients = clients.OrderBy(s => s.Address);
                    break;
                case "address_desc":
                    clients = clients.OrderByDescending(s => s.Address);
                    break;
                case "Contact":
                    clients = clients.OrderBy(s => s.Contact);
                    break;
                case "contact_desc":
                    clients = clients.OrderByDescending(s => s.Contact);
                    break;
                case "Email":
                    clients = clients.OrderBy(s => s.Email);
                    break;
                case "email_desc":
                    clients = clients.OrderByDescending(s => s.Email);
                    break;
                default: // Name ascending 
                    clients = clients.OrderBy(s => s.Name);
                    break;
            }
            int pageSize = 3;
            int pageNumber = page ?? 1;
            return View(clients.ToPagedList(pageNumber, pageSize));
        }

        // GET: Client/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var client = _unitOfWork.ClientRepository.GetById(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // GET: Client/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Client/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Phone,Address,Contact,Email")] Client client)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.ClientRepository.Insert(client);
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("",
                        "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(client);
        }

        // GET: Client/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var client = _unitOfWork.ClientRepository.GetById(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Client/Edit/5
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
            var clientToUpdate = _unitOfWork.ClientRepository.GetById(id);
            if (clientToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(clientToUpdate, "", new[] {"Name", "Phone", "Address", "Contact", "Email"}))
            {
                try
                {
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("",
                        "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(clientToUpdate);
        }

        // GET: Client/Delete/5
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
            var client = _unitOfWork.ClientRepository.Get().Single(c => c.ID == id);
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
                _unitOfWork.ClientRepository.Delete(id);
                _unitOfWork.Save();
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new {id, saveChangesError = true});
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
