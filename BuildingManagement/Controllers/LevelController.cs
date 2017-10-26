using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;

namespace BuildingManagement.Controllers
{
    public class LevelController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        // GET: Level
        public ActionResult Index()
        {
            var levels = _unitOfWork.LevelRepository.Get(includeProperties: "Section, Client");
            return View(levels);
        }

        // GET: Level/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var level = _unitOfWork.LevelRepository.GetById(id);
            if (level == null)
            {
                return HttpNotFound();
            }
            return View(level);
        }

        // GET: Level/Create
        public ActionResult Create()
        {
            PopulateSectionsDropDownList();
            PopulateClientsDropDownList();
            return View();
        }

        // POST: Level/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Number,SectionID,ClientID")] Level level)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //uniqueness condition check
                    var duplicateLevel = _unitOfWork.LevelRepository.Get(filter: l => l.Number == level.Number && l.SectionID == level.SectionID).FirstOrDefault();
                    if (duplicateLevel != null)
                    {
                        ModelState.AddModelError("Number", "A level with this number already exists for this section.");
                        PopulateSectionsDropDownList(level.SectionID);
                        PopulateClientsDropDownList(level.ClientID);
                        return View(level);
                    }
                    level.Surface = 0m;
                    level.People = 0;
                    _unitOfWork.LevelRepository.Insert(level);
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateSectionsDropDownList(level.SectionID);
            PopulateClientsDropDownList(level.ClientID);
            return View(level);
        }

        // GET: Level/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var level = _unitOfWork.LevelRepository.GetById(id);
            if (level == null)
            {
                return HttpNotFound();
            }
            PopulateSectionsDropDownList(level.SectionID);
            PopulateClientsDropDownList(level.ClientID);
            return View(level);
        }

        // POST: Level/Edit/5
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
            var levelToUpdate = _unitOfWork.LevelRepository.GetById(id);
            if (levelToUpdate == null)
            {
                return HttpNotFound();
            }
            var oldSection = _unitOfWork.SectionRepository.GetById(levelToUpdate.SectionID);
            if (oldSection == null)
            {
                return HttpNotFound();
            }
            oldSection.Surface = oldSection.Surface - levelToUpdate.Surface;
            oldSection.People = oldSection.People - levelToUpdate.People;
            if (TryUpdateModel(levelToUpdate, "", new[] { "Number", "SectionID", "ClientID" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateLevel = _unitOfWork.LevelRepository.Get(filter: l => l.Number == levelToUpdate.Number && l.SectionID == levelToUpdate.SectionID).FirstOrDefault();
                    if (duplicateLevel != null && duplicateLevel.ID != levelToUpdate.ID)
                    {
                        ModelState.AddModelError("Number", "A level with this number already exists for this section.");
                        PopulateSectionsDropDownList(levelToUpdate.SectionID);
                        PopulateClientsDropDownList(levelToUpdate.ClientID);
                        return View(levelToUpdate);
                    }
                    var section = _unitOfWork.SectionRepository.GetById(levelToUpdate.SectionID);
                    if (section == null)
                    {
                        return HttpNotFound();
                    }
                    section.Surface = section.Surface + levelToUpdate.Surface;
                    section.People = section.People + levelToUpdate.People;
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateSectionsDropDownList(levelToUpdate.SectionID);
            PopulateClientsDropDownList(levelToUpdate.ClientID);
            return View(levelToUpdate);
        }

        // GET: Level/Delete/5
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
            var level = _unitOfWork.LevelRepository.Get(includeProperties: "Section, Client").Single(l => l.ID == id);
            if (level == null)
            {
                return HttpNotFound();
            }
            return View(level);
        }

        // POST: Level/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _unitOfWork.LevelRepository.Delete(id);
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

        private void PopulateSectionsDropDownList(object selectedSection = null)
        {
            var sectionsQuery = from s in _unitOfWork.SectionRepository.Get() select s;
            ViewBag.SectionID = new SelectList(sectionsQuery, "ID", "Number", selectedSection);
        }

        private void PopulateClientsDropDownList(object selectedClient = null)
        {
            var clientsQuery = from c in _unitOfWork.ClientRepository.Get() select c;
            ViewBag.ClientID = new SelectList(clientsQuery, "ID", "Name", selectedClient);
        }

        [HttpPost]
        public ActionResult GetSectionsByClient(int clientId, int? sectionId)
        {
            var list = new List<SelectListItem>();
            var sections = _unitOfWork.SectionRepository.Get().Where(s => s.ClientID == clientId).ToList();
            foreach (var section in sections)
            {
                if (sectionId != null && sectionId == section.ID)
                {
                    list.Add(new SelectListItem { Value = section.ID.ToString(), Text = section.Number, Selected = true });
                }
                else
                {
                    list.Add(new SelectListItem { Value = section.ID.ToString(), Text = section.Number });
                }
            }
            return Json(list);
        }
    }
}
