﻿using BuildingManagement.DAL;
using BuildingManagement.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
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
                levels = _unitOfWork.LevelRepository.GetFilteredLevelsIncludingSection(searchString).ToList();
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    levels = _unitOfWork.LevelRepository.GetFilteredLevelsIncludingSection(searchString).ToList();
                }
                else
                {
                    levels = _unitOfWork.LevelRepository.GetAllLevelsIncludingSection().ToList();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClientSortParm = string.IsNullOrEmpty(sortOrder) ? "section_desc" : "";
            ViewBag.NumberSortParm = sortOrder == "Number" ? "number_desc" : "Number";
            ViewBag.SurfaceSortParm = sortOrder == "Surface" ? "surface_desc" : "Surface";
            ViewBag.PeopleSortParm = sortOrder == "People" ? "people_desc" : "People";
            levels = _unitOfWork.LevelRepository.OrderLevels(levels, sortOrder).ToList();
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
            var level = new Level();
            PopulateSectionsDropDownList();
            PopulateClientsDropDownList();
            return View(level);
        }

        // POST: Level/Create
        [HttpPost]
        public ActionResult Create(Level level)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                var duplicateLevel = _unitOfWork.LevelRepository.FirstOrDefault(l => l.Number == level.Number && l.SectionID == level.SectionID);
                if (duplicateLevel != null)
                {
                    PopulateSectionsDropDownList(level.SectionID);
                    PopulateClientsDropDownList(level.ClientID);
                    return new HttpStatusCodeResult(409, "A level with this number already exists for this section.");
                }
                var section = _unitOfWork.SectionRepository.Get(level.SectionID);
                if (section != null)
                {
                    level.Section = section;
                }
                level.Surface = 0m;
                level.People = 0;
                try
                {
                    _unitOfWork.LevelRepository.Add(level);
                    _unitOfWork.Save();
                    TempData["message"] = $"Level {level.Number} has been created.";
                    return Json(level.ID);
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
        public ActionResult Edit(Level level)
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
                    var duplicateLevel = _unitOfWork.LevelRepository.FirstOrDefault(l => l.ID != levelToUpdate.ID && l.Number == levelToUpdate.Number && l.SectionID == levelToUpdate.SectionID);
                    if (duplicateLevel != null)
                    {
                        PopulateSectionsDropDownList(levelToUpdate.SectionID);
                        PopulateClientsDropDownList(levelToUpdate.ClientID);
                        return new HttpStatusCodeResult(409, "A level with this number already exists for this section.");
                    }
                    var section = _unitOfWork.SectionRepository.Get(levelToUpdate.SectionID);
                    if (section == null)
                    {
                        return HttpNotFound();
                    }
                    section.Surface = section.Surface + levelToUpdate.Surface;
                    section.People = section.People + levelToUpdate.People;
                    _unitOfWork.Save();
                    TempData["message"] = $"Level {levelToUpdate.Number} has been edited.";
                    return Json(levelToUpdate.ID);
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
                var level = _unitOfWork.LevelRepository.Get(id);
                if (level == null)
                {
                    return HttpNotFound();
                }
                _unitOfWork.LevelRepository.Remove(level);
                _unitOfWork.Save();
                TempData["message"] = $"Level {level.Number} has been deleted.";
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
            var sectionsQuery = _unitOfWork.SectionRepository.GetAll().ToList();
            ViewBag.SectionID = new SelectList(sectionsQuery, "ID", "Number", selectedSection);
        }

        private void PopulateClientsDropDownList(object selectedClient = null)
        {
            var clientsQuery = _unitOfWork.ClientRepository.GetAll().ToList();
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
