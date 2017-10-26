using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;

namespace BuildingManagement.Controllers
{
    public class SectionController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        // GET: Section
        public ActionResult Index()
        {
            var sections = _unitOfWork.SectionRepository.Get(includeProperties: "Client");
            return View(sections);
        }

        // GET: Section/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var section = _unitOfWork.SectionRepository.GetById(id);
            if (section == null)
            {
                return HttpNotFound();
            }
            return View(section);
        }

        // GET: Section/Create
        public ActionResult Create()
        {
            PopulateClientsDropDownList();
            return View();
        }

        // POST: Section/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Number,ClientID")] Section section)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //uniqueness condition check
                    var duplicateSection = _unitOfWork.SectionRepository.Get(filter: s => s.Number == section.Number && s.ClientID == section.ClientID).FirstOrDefault();
                    if (duplicateSection != null)
                    {
                        ModelState.AddModelError("Number", "A section with this number already exists for this client.");
                        PopulateClientsDropDownList(section.ClientID);
                        return View(section);
                    }
                    section.Surface = 0m;
                    section.People = 0;
                    _unitOfWork.SectionRepository.Insert(section);
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateClientsDropDownList(section.ClientID);
            return View(section);
        }

        // GET: Section/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var section = _unitOfWork.SectionRepository.GetById(id);
            if (section == null)
            {
                return HttpNotFound();
            }
            PopulateClientsDropDownList(section.ClientID);
            return View(section);
        }

        // POST: Section/Edit/5
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
            var sectionToUpdate = _unitOfWork.SectionRepository.GetById(id);
            if (sectionToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(sectionToUpdate, "", new[] { "Number", "ClientID" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateSection = _unitOfWork.SectionRepository.Get(filter: s => s.Number == sectionToUpdate.Number && s.ClientID == sectionToUpdate.ClientID).FirstOrDefault();
                    if (duplicateSection != null && duplicateSection.ID != sectionToUpdate.ID)
                    {
                        ModelState.AddModelError("Number", "A section with this number already exists for this client.");
                        PopulateClientsDropDownList(sectionToUpdate.ClientID);
                        return View(sectionToUpdate);
                    }
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateClientsDropDownList(sectionToUpdate.ClientID);
            return View(sectionToUpdate);
        }

        // GET: Section/Delete/5
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
            var section = _unitOfWork.SectionRepository.Get(includeProperties: "Client").Single(s => s.ID == id);
            if (section == null)
            {
                return HttpNotFound();
            }
            return View(section);
        }

        // POST: Section/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _unitOfWork.SectionRepository.Delete(id);
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
