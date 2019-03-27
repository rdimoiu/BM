using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using X.PagedList;

namespace BuildingManagement.Controllers
{
    public class SpaceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SpaceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Space
        public async Task<ActionResult> Index(int? page, string currentFilter, string searchString, string sortOrder)
        {
            IEnumerable<Space> spaces;
            var pageNumber = page ?? 1;
            const int pageSize = 3;
            if (searchString != null)
            {
                pageNumber = 1;
                spaces = await _unitOfWork.SpaceRepository.GetFilteredSpacesIncludingLevelAndSpaceTypeAndSubClient(searchString, sortOrder).ToListAsync();
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    spaces = await _unitOfWork.SpaceRepository.GetFilteredSpacesIncludingLevelAndSpaceTypeAndSubClient(searchString, sortOrder).ToListAsync();
                }
                else
                {
                    spaces = await _unitOfWork.SpaceRepository.GetAllSpacesIncludingLevelAndSpaceTypeAndSubClient(sortOrder).ToListAsync();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NumberSortParm = string.IsNullOrEmpty(sortOrder) ? "number_desc" : "";
            ViewBag.SurfaceSortParm = sortOrder == "Surface" ? "surface_desc" : "Surface";
            ViewBag.PeopleSortParm = sortOrder == "People" ? "people_desc" : "People";
            ViewBag.InhabitedSortParm = sortOrder == "Inhabited" ? "inhabited_desc" : "Inhabited";
            ViewBag.LevelSortParm = sortOrder == "Level" ? "level_desc" : "Level";
            ViewBag.SpaceTypeSortParm = sortOrder == "SpaceType" ? "spaceType_desc" : "SpaceType";
            ViewBag.SubClientSortParm = sortOrder == "SubClient" ? "subClient_desc" : "SubClient";
            ViewBag.OnePageOfSpaces = spaces.ToPagedList(pageNumber, pageSize);
            return View(ViewBag.OnePageOfSpaces);
        }

        // GET: Space/Details/5
        public ActionResult Details(int id)
        {
            var space = _unitOfWork.SpaceRepository.Get(id);
            if (space == null)
            {
                return HttpNotFound();
            }
            return View(space);
        }

        // GET: Space/Create
        public ActionResult Create()
        {
            var space = new Space();
            PopulateClientsDropDownList();
            PopulateSectionsDropDownList();
            PopulateLevelsDropDownList();
            PopulateSpaceTypesDropDownList();
            PopulateSubClientsDropDownList();
            return View(space);
        }

        // POST: Space/Create
        [HttpPost]
        public async Task<ActionResult> Create(Space space)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                var duplicateSpace = _unitOfWork.SpaceRepository.FirstOrDefault(s => s.Number == space.Number && s.LevelID == space.LevelID && s.SpaceTypeID == space.SpaceTypeID);
                if (duplicateSpace != null)
                {
                    PopulateClientsDropDownList(space.ClientID);
                    PopulateSectionsDropDownList(space.SectionID);
                    PopulateLevelsDropDownList(space.LevelID);
                    PopulateSpaceTypesDropDownList(space.SpaceTypeID);
                    PopulateSubClientsDropDownList(space.SubClientID);
                    return new HttpStatusCodeResult(409, "A space with this number, of this type, already exists for this level.");
                }
                var level = _unitOfWork.LevelRepository.Get(space.LevelID);
                if (level == null)
                {
                    return HttpNotFound();
                }
                level.Surface = level.Surface + space.Surface;
                level.People = level.People + space.People;
                var section = _unitOfWork.SectionRepository.Get(level.SectionID);
                if (section == null)
                {
                    return HttpNotFound();
                }
                section.Surface = section.Surface + space.Surface;
                section.People = section.People + space.People;
                try
                {
                    _unitOfWork.SpaceRepository.Add(space);
                    await _unitOfWork.SaveAsync();
                    TempData["message"] = $"Space {space.Number} has been created.";
                    return Json(space.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateClientsDropDownList(space.ClientID);
            PopulateSectionsDropDownList(space.SectionID);
            PopulateLevelsDropDownList(space.LevelID);
            PopulateSpaceTypesDropDownList(space.SpaceTypeID);
            PopulateSubClientsDropDownList(space.SubClientID);
            return View(space);
        }

        // GET: Space/Edit/5
        public ActionResult Edit(int id)
        {
            var space = _unitOfWork.SpaceRepository.GetSpaceIncludingLevel(id);
            if (space == null)
            {
                return HttpNotFound();
            }
            var level = _unitOfWork.LevelRepository.GetLevelIncludingSection(space.LevelID);
            if (level == null)
            {
                return HttpNotFound();
            }
            space.ClientID = level.Section.ClientID;
            space.SectionID = level.SectionID;
            PopulateClientsDropDownList(space.ClientID);
            PopulateSectionsDropDownList(space.Level.SectionID);
            PopulateLevelsDropDownList(space.LevelID);
            PopulateSpaceTypesDropDownList(space.SpaceTypeID);
            PopulateSubClientsDropDownList(space.SubClientID);
            return View(space);
        }

        // POST: Space/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(Space space)
        {
            var spaceToUpdate = _unitOfWork.SpaceRepository.GetSpaceIncludingLevel(space.ID);
            if (spaceToUpdate == null)
            {
                return HttpNotFound();
            }
            var oldLevel = _unitOfWork.LevelRepository.Get(spaceToUpdate.LevelID);
            if (oldLevel == null)
            {
                return HttpNotFound();
            }
            oldLevel.Surface = oldLevel.Surface - spaceToUpdate.Surface;
            oldLevel.People = oldLevel.People - spaceToUpdate.People;
            var oldSection = _unitOfWork.SectionRepository.Get(oldLevel.SectionID);
            if (oldSection == null)
            {
                return HttpNotFound();
            }
            oldSection.Surface = oldSection.Surface - spaceToUpdate.Surface;
            oldSection.People = oldSection.People - spaceToUpdate.People;
            if (TryUpdateModel(spaceToUpdate, "", new[] { "Number", "Surface", "People", "Inhabited", "LevelID", "SpaceTypeID", "ClientID", "SubClientID" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateSpace = _unitOfWork.SpaceRepository.FirstOrDefault(s => s.ID != spaceToUpdate.ID && s.Number == spaceToUpdate.Number && s.LevelID == spaceToUpdate.LevelID && s.SpaceTypeID == spaceToUpdate.SpaceTypeID);
                    if (duplicateSpace != null)
                    {
                        PopulateClientsDropDownList(spaceToUpdate.ClientID);
                        PopulateSectionsDropDownList(spaceToUpdate.SectionID);
                        PopulateLevelsDropDownList(spaceToUpdate.LevelID);
                        PopulateSpaceTypesDropDownList(spaceToUpdate.SpaceTypeID);
                        PopulateSubClientsDropDownList(spaceToUpdate.SubClientID);
                        return new HttpStatusCodeResult(409, "A space with this number, of this type, already exists for this level.");
                    }
                    var level = _unitOfWork.LevelRepository.Get(spaceToUpdate.LevelID);
                    if (level == null)
                    {
                        return HttpNotFound();
                    }
                    level.Surface = level.Surface + spaceToUpdate.Surface;
                    level.People = level.People + spaceToUpdate.People;
                    var section = _unitOfWork.SectionRepository.Get(level.SectionID);
                    if (section == null)
                    {
                        return HttpNotFound();
                    }
                    section.Surface = section.Surface + spaceToUpdate.Surface;
                    section.People = section.People + spaceToUpdate.People;
                    await _unitOfWork.SaveAsync();
                    TempData["message"] = $"Space {spaceToUpdate.Number} has been edited.";
                    return Json(spaceToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateClientsDropDownList(spaceToUpdate.ClientID);
            PopulateSectionsDropDownList(spaceToUpdate.SectionID);
            PopulateLevelsDropDownList(spaceToUpdate.LevelID);
            PopulateSpaceTypesDropDownList(spaceToUpdate.SpaceTypeID);
            PopulateSubClientsDropDownList(spaceToUpdate.SubClientID);
            return View(spaceToUpdate);
        }

        // GET: Space/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var space = _unitOfWork.SpaceRepository.GetSpaceIncludingLevel(id);
            if (space == null)
            {
                return HttpNotFound();
            }
            return View(space);
        }

        // POST: Space/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var space = _unitOfWork.SpaceRepository.Get(id);
                if (space == null)
                {
                    return HttpNotFound();
                }
                var level = _unitOfWork.LevelRepository.Get(space.LevelID);
                if (level == null)
                {
                    return HttpNotFound();
                }
                level.Surface = level.Surface - space.Surface;
                level.People = level.People - space.People;
                var section = _unitOfWork.SectionRepository.Get(level.SectionID);
                if (section == null)
                {
                    return HttpNotFound();
                }
                section.Surface = section.Surface - space.Surface;
                section.People = section.People - space.People;
                _unitOfWork.SpaceRepository.Remove(space);
                await _unitOfWork.SaveAsync();
                TempData["message"] = $"Space {space.Number} has been deleted.";
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

        private void PopulateSectionsDropDownList(object selectedSection = null)
        {
            var sectionsQuery = _unitOfWork.SectionRepository.GetAll().ToList();
            ViewBag.SectionID = new SelectList(sectionsQuery, "ID", "Number", selectedSection);
        }

        private void PopulateLevelsDropDownList(object selectedLevel = null)
        {
            var levelsQuery = _unitOfWork.LevelRepository.GetAll().ToList();
            ViewBag.LevelID = new SelectList(levelsQuery, "ID", "Number", selectedLevel);
        }

        private void PopulateSpaceTypesDropDownList(object selectedSpaceType = null)
        {
            var spaceTypesQuery = _unitOfWork.SpaceTypeRepository.GetAll().ToList();
            ViewBag.SpaceTypeID = new SelectList(spaceTypesQuery, "ID", "Type", selectedSpaceType);
        }

        private void PopulateSubClientsDropDownList(object selectedSubClient = null)
        {
            var subClientsQuery = _unitOfWork.SubClientRepository.GetAll().ToList();
            ViewBag.SubClientID = new SelectList(subClientsQuery, "ID", "Name", selectedSubClient);
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

        [HttpPost]
        public ActionResult GetLevelsBySection(int? sectionId, int? levelId)
        {
            var list = new List<SelectListItem>();
            var levels = sectionId != null ? _unitOfWork.LevelRepository.Find(s => s.SectionID == sectionId).ToList() : _unitOfWork.LevelRepository.GetAll().ToList();
            foreach (var level in levels)
            {
                if (sectionId != null && levelId != null && levelId == level.ID)
                {
                    list.Add(new SelectListItem { Value = level.ID.ToString(), Text = level.Number, Selected = true });
                }
                else
                {
                    list.Add(new SelectListItem { Value = level.ID.ToString(), Text = level.Number });
                }
            }
            return Json(list);
        }
    }
}
