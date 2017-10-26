using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;

namespace BuildingManagement.Controllers
{
    public class SpaceController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        // GET: Space
        public ActionResult Index()
        {
            var spaces = _unitOfWork.SpaceRepository.Get(includeProperties: "Level, SpaceType, Client, SubClient");
            return View(spaces);
        }

        // GET: Space/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var space = _unitOfWork.SpaceRepository.GetById(id);
            if (space == null)
            {
                return HttpNotFound();
            }
            return View(space);
        }

        // GET: Space/Create
        public ActionResult Create()
        {
            var model = new Space();
            PopulateLevelsDropDownList();
            PopulateSpaceTypesDropDownList();
            PopulateClientsDropDownList();
            PopulateSubClientsDropDownList();
            return View(model);
        }

        // POST: Space/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Number,Surface,People,Inhabited,LevelID,SpaceTypeID,SubClientID")] Space space)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //uniqueness condition check
                    var duplicateSpace =
                        _unitOfWork.SpaceRepository.Get(filter: s => s.Number == space.Number && s.LevelID == space.LevelID && s.SpaceTypeID == space.SpaceTypeID).FirstOrDefault();
                    if (duplicateSpace != null)
                    {
                        ModelState.AddModelError("Number", "A space with this number, of this type, already exists for this level.");
                        PopulateLevelsDropDownList(space.LevelID);
                        PopulateSpaceTypesDropDownList(space.SpaceTypeID);
                        PopulateClientsDropDownList(space.ClientID);
                        PopulateSubClientsDropDownList(space.SubClientID);
                        return View(space);
                    }
                    var level = _unitOfWork.LevelRepository.GetById(space.LevelID);
                    if (level == null)
                    {
                        return HttpNotFound();
                    }
                    level.Surface = level.Surface + space.Surface;
                    //level.People = level.People + space.People;
                    var section = _unitOfWork.SectionRepository.GetById(level.SectionID);
                    if (section == null)
                    {
                        return HttpNotFound();
                    }
                    section.Surface = section.Surface + space.Surface;
                    section.People = section.People + space.People;
                    _unitOfWork.SpaceRepository.Insert(space);
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateLevelsDropDownList(space.LevelID);
            PopulateSpaceTypesDropDownList(space.SpaceTypeID);
            PopulateClientsDropDownList(space.ClientID);
            PopulateSubClientsDropDownList(space.SubClientID);
            return View(space);
        }

        // GET: Space/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var space = _unitOfWork.SpaceRepository.GetById(id);
            if (space == null)
            {
                return HttpNotFound();
            }
            PopulateLevelsDropDownList(space.LevelID);
            PopulateSpaceTypesDropDownList(space.SpaceTypeID);
            PopulateClientsDropDownList(space.ClientID);
            PopulateSubClientsDropDownList(space.SubClientID);
            return View(space);
        }

        // POST: Space/Edit/5
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
            var spaceToUpdate = _unitOfWork.SpaceRepository.GetById(id);
            if (spaceToUpdate == null)
            {
                return HttpNotFound();
            }
            var oldLevel = _unitOfWork.LevelRepository.GetById(spaceToUpdate.LevelID);
            if (oldLevel == null)
            {
                return HttpNotFound();
            }
            oldLevel.Surface = oldLevel.Surface - spaceToUpdate.Surface;
            oldLevel.People = oldLevel.People - spaceToUpdate.People;
            var oldSection = _unitOfWork.SectionRepository.GetById(oldLevel.SectionID);
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
                    var duplicateSpace = _unitOfWork.SpaceRepository.Get(filter: s => s.Number == spaceToUpdate.Number && s.LevelID == spaceToUpdate.LevelID && s.SpaceTypeID == spaceToUpdate.SpaceTypeID).FirstOrDefault();
                    if (duplicateSpace != null && duplicateSpace.ID != spaceToUpdate.ID)
                    {
                        ModelState.AddModelError("Number", "A space with this number, of this type, already exists for this level.");
                        PopulateLevelsDropDownList(spaceToUpdate.LevelID);
                        PopulateSpaceTypesDropDownList(spaceToUpdate.SpaceTypeID);
                        PopulateClientsDropDownList(spaceToUpdate.ClientID);
                        return View(spaceToUpdate);
                    }
                    var level = _unitOfWork.LevelRepository.GetById(spaceToUpdate.LevelID);
                    if (level == null)
                    {
                        return HttpNotFound();
                    }
                    level.Surface = level.Surface + spaceToUpdate.Surface;
                    level.People = level.People + spaceToUpdate.People;
                    var section = _unitOfWork.SectionRepository.GetById(level.SectionID);
                    if (section == null)
                    {
                        return HttpNotFound();
                    }
                    section.Surface = section.Surface + spaceToUpdate.Surface;
                    section.People = section.People + spaceToUpdate.People;
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateLevelsDropDownList(spaceToUpdate.LevelID);
            PopulateSpaceTypesDropDownList(spaceToUpdate.SpaceTypeID);
            PopulateClientsDropDownList(spaceToUpdate.ClientID);
            PopulateSubClientsDropDownList(spaceToUpdate.SubClientID);
            return View(spaceToUpdate);
        }

        // GET: Space/Delete/5
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
            var space = _unitOfWork.SpaceRepository.Get(includeProperties: "Level, SpaceType, Client, SubClient").Single(s => s.ID == id);
            if (space == null)
            {
                return HttpNotFound();
            }
            return View(space);
        }

        // POST: Space/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var spaceToDelete = _unitOfWork.SpaceRepository.GetById(id);
                if (spaceToDelete == null)
                {
                    return HttpNotFound();
                }
                var level = _unitOfWork.LevelRepository.GetById(spaceToDelete.LevelID);
                if (level == null)
                {
                    return HttpNotFound();
                }
                level.Surface = level.Surface - spaceToDelete.Surface;
                level.People = level.People - spaceToDelete.People;
                var section = _unitOfWork.SectionRepository.GetById(level.SectionID);
                if (section == null)
                {
                    return HttpNotFound();
                }
                section.Surface = section.Surface - spaceToDelete.Surface;
                section.People = section.People - spaceToDelete.People;
                _unitOfWork.SpaceRepository.Delete(id);
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

        private void PopulateLevelsDropDownList(object selectedLevel = null)
        {
            var levelsQuery = from l in _unitOfWork.LevelRepository.Get() select l;
            ViewBag.LevelID = new SelectList(levelsQuery, "ID", "Number", selectedLevel);
        }

        private void PopulateSpaceTypesDropDownList(object selectedSpaceType = null)
        {
            var spaceTypesQuery = from st in _unitOfWork.SpaceTypeRepository.Get() select st;
            ViewBag.SpaceTypeID = new SelectList(spaceTypesQuery, "ID", "Type", selectedSpaceType);
        }

        private void PopulateClientsDropDownList(object selectedClient = null)
        {
            var clientsQuery = from c in _unitOfWork.ClientRepository.Get() select c;
            ViewBag.ClientID = new SelectList(clientsQuery, "ID", "Name", selectedClient);
        }

        private void PopulateSubClientsDropDownList(object selectedSubClient = null)
        {
            var subClientsQuery = from sc in _unitOfWork.SubClientRepository.Get() select sc;
            ViewBag.SubClientID = new SelectList(subClientsQuery, "ID", "Name", selectedSubClient);
        }

        [HttpPost]
        public ActionResult GetLevelsByClient(int clientId, int? levelId)
        {
            var list = new List<SelectListItem>();
            var levels = _unitOfWork.LevelRepository.Get().Where(l => l.ClientID == clientId).ToList();
            foreach (var level in levels)
            {
                if (levelId != null && levelId == level.ID)
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
