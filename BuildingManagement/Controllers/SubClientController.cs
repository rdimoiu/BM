using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;

namespace BuildingManagement.Controllers
{
    public class SubClientController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        // GET: SubClient
        public ActionResult Index()
        {
            var subClients = _unitOfWork.SubClientRepository.Get(includeProperties: "Client");
            return View(subClients);
        }

        // GET: SubClient/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var subClient = _unitOfWork.SubClientRepository.GetById(id);
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
                    var duplicateSubClient =
                        _unitOfWork.SubClientRepository.Get(
                            filter: sc => sc.CNP == subClient.CNP || sc.FiscalCode == subClient.FiscalCode)
                            .FirstOrDefault();
                    if (duplicateSubClient != null)
                    {
                        ModelState.AddModelError("FiscalCode", "A sub client with this CNP or FiscalCode already exists.");
                        PopulateClientsDropDownList(subClient.ClientID);
                        return View("Create");
                    }
                    _unitOfWork.SubClientRepository.Insert(subClient);
                    _unitOfWork.Save();
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
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var subClient = _unitOfWork.SubClientRepository.GetById(id);
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
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var subClientToUpdate = _unitOfWork.SubClientRepository.GetById(id);
            if (subClientToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(subClientToUpdate, "", new[] { "Name", "Phone", "Country", "State", "City", "Street", "Contact", "Email", "IBAN", "Bank", "CNP", "FiscalCode", "ClientID" }))
            {
                try
                {
                    if (string.IsNullOrEmpty(subClientToUpdate.CNP) || string.IsNullOrEmpty(subClientToUpdate.FiscalCode))
                    {
                        ModelState.AddModelError("CNP", "A CNP or FiscalCode is required.");
                        ModelState.AddModelError("FiscalCode", "A CNP or FiscalCode is required.");
                        PopulateClientsDropDownList(subClientToUpdate.ClientID);
                        return View("Create");
                    }
                    //uniqueness condition check
                    var duplicateSubClient =
                        _unitOfWork.SubClientRepository.Get(
                            filter: sc => sc.CNP == subClientToUpdate.CNP || sc.FiscalCode == subClientToUpdate.FiscalCode)
                            .FirstOrDefault();
                    if (duplicateSubClient != null && duplicateSubClient.ID != subClientToUpdate.ID)
                    {
                        ModelState.AddModelError("Code", "A sub client with this CNP or FiscalCode already exists.");
                        PopulateClientsDropDownList(subClientToUpdate.ClientID);
                        return View("Create");
                    }
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
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
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var subClient = _unitOfWork.SubClientRepository.Get(includeProperties: "Client").Single(c => c.ID == id);
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
                _unitOfWork.SubClientRepository.Delete(id);
                _unitOfWork.Save();
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
            var clientsQuery = from c in _unitOfWork.ClientRepository.Get() select c;
            ViewBag.ClientID = new SelectList(clientsQuery, "ID", "Name", selectedClient);
        }
    }
}
