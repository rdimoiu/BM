using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using X.PagedList;

namespace BuildingManagement.Controllers
{
    public class LevelController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LevelController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Level
        public ActionResult Index(int? page, string currentFilter, string searchString, string sortOrder)
        {
            IEnumerable<Level> levels;
            var pageNumber = page ?? 1;
            const int pageSize = 3;
            if (searchString != null)
            {
                pageNumber = 1;
                levels = _unitOfWork.LevelRepository.GetFilteredLevelsIncludingSection(searchString);
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    levels = _unitOfWork.LevelRepository.GetFilteredLevelsIncludingSection(searchString);
                }
                else
                {
                    levels = _unitOfWork.LevelRepository.GetAllLevelsIncludingSection();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClientSortParm = string.IsNullOrEmpty(sortOrder) ? "section_desc" : "";
            ViewBag.NumberSortParm = sortOrder == "Number" ? "number_desc" : "Number";
            ViewBag.SurfaceSortParm = sortOrder == "Surface" ? "surface_desc" : "Surface";
            ViewBag.PeopleSortParm = sortOrder == "People" ? "people_desc" : "People";
            levels = _unitOfWork.LevelRepository.OrderLevels(levels, sortOrder);
            ViewBag.OnePageOfLevels = levels.ToPagedList(pageNumber, pageSize);
            return View(ViewBag.OnePageOfLevels);
        }

        // GET: Level/Details/5
        public ActionResult Details(int id)
        {
            var level = _unitOfWork.LevelRepository.Get(id);
            if (level == null)
            {
                return HttpNotFound();
            }
            return View(level);
        }

        // GET: Level/Create
        public ActionResult Create()
        {
            var model = new Level();
            PopulateSectionsDropDownList();
            PopulateClientsDropDownList();
            return View(model);
        }

        // POST: Level/Create
        [HttpPost]
        public ActionResult CreateLevel(Level level)
        {
            //uniqueness condition check
            var duplicateLevel = _unitOfWork.LevelRepository.SingleOrDefault(l => l.Number == level.Number && l.SectionID == level.SectionID);
            if (duplicateLevel != null)
            {
                ModelState.AddModelError("Number", "A level with this number already exists for this section.");
                PopulateSectionsDropDownList(level.SectionID);
                PopulateClientsDropDownList(level.ClientID);
                return View("Create", level);
            }
            var section = _unitOfWork.SectionRepository.Get(level.SectionID);
            if (section != null)
            {
                level.Section = section;
            }
            level.Surface = 0m;
            level.People = 0;
            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.LevelRepository.Add(level);
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Level {0} has been created.", level.Number);
                    return Json(level.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateSectionsDropDownList(level.SectionID);
            PopulateClientsDropDownList(level.ClientID);
            return View("Create", level);
        }

        // GET: Level/Edit/5
        public ActionResult Edit(int id)
        {
            var level = _unitOfWork.LevelRepository.GetLevelIncludingSection(id);
            if (level == null)
            {
                return HttpNotFound();
            }
            PopulateSectionsDropDownList(level.SectionID);
            PopulateClientsDropDownList(level.Section.ClientID);
            return View(level);
        }

        // POST: Level/Edit/5
        [HttpPost]
        public ActionResult EditLevel(Level level)
        {
            var levelToUpdate = _unitOfWork.LevelRepository.GetLevelIncludingSection(level.ID);
            if (levelToUpdate == null)
            {
                return HttpNotFound();
            }
            var oldSection = _unitOfWork.SectionRepository.Get(levelToUpdate.SectionID);
            if (oldSection == null)
            {
                return HttpNotFound();
            }
            oldSection.Surface = oldSection.Surface - levelToUpdate.Surface;
            oldSection.People = oldSection.People - levelToUpdate.People;
            if (TryUpdateModel(levelToUpdate, "", new[] { "Number", "SectionID" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateLevel = _unitOfWork.LevelRepository.SingleOrDefault(l => l.Number == levelToUpdate.Number && l.SectionID == levelToUpdate.SectionID);
                    if (duplicateLevel != null && duplicateLevel.ID != levelToUpdate.ID)
                    {
                        ModelState.AddModelError("Number", "A level with this number already exists for this section.");
                        PopulateSectionsDropDownList(levelToUpdate.SectionID);
                        PopulateClientsDropDownList(levelToUpdate.ClientID);
                        return View("Edit", levelToUpdate);
                    }
                    var section = _unitOfWork.SectionRepository.Get(levelToUpdate.SectionID);
                    if (section == null)
                    {
                        return HttpNotFound();
                    }
                    section.Surface = section.Surface + levelToUpdate.Surface;
                    section.People = section.People + levelToUpdate.People;
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Level {0} has been edited.", levelToUpdate.Number);
                    return Json(levelToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateSectionsDropDownList(levelToUpdate.SectionID);
            PopulateClientsDropDownList(levelToUpdate.ClientID);
            return View("Edit", levelToUpdate);
        }

        // GET: Level/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var level = _unitOfWork.LevelRepository.GetLevelIncludingSection(id);
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
                var level = _unitOfWork.LevelRepository.SingleOrDefault(l => l.ID == id);
                if (level == null)
                {
                    return HttpNotFound();
                }
                _unitOfWork.LevelRepository.Remove(level);
                _unitOfWork.Save();
                TempData["message"] = string.Format("Level {0} has been deleted.", level.Number);
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
            var sectionsQuery = from s in _unitOfWork.SectionRepository.GetAll() select s;
            ViewBag.SectionID = new SelectList(sectionsQuery, "ID", "Number", selectedSection);
        }

        private void PopulateClientsDropDownList(object selectedClient = null)
        {
            var clientsQuery = from c in _unitOfWork.ClientRepository.GetAll() select c;
            ViewBag.ClientID = new SelectList(clientsQuery, "ID", "Name", selectedClient);
        }

        [HttpPost]
        public ActionResult GetSectionsByClient(int? clientId, int? sectionId)
        {
            var list = new List<SelectListItem>();
            var sections = clientId != null ? _unitOfWork.SectionRepository.Find(s => s.ClientID == clientId).ToList() : _unitOfWork.SectionRepository.GetAll().ToList();
            foreach (var section in sections)
            {
                if (clientId != null && sectionId != null && sectionId == section.ID)
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
