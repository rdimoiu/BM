using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using X.PagedList;

namespace BuildingManagement.Controllers
{
    public class SpaceTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SpaceTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: SpaceType
        public ActionResult Index(int? page, string currentFilter, string searchString, string sortOrder)
        {
            IEnumerable<SpaceType> spaceTypes;
            var pageNumber = page ?? 1;
            const int pageSize = 3;
            if (searchString != null)
            {
                pageNumber = 1;
                spaceTypes = _unitOfWork.SpaceTypeRepository.GetFilteredSpaceTypes(searchString);
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    spaceTypes = _unitOfWork.SpaceTypeRepository.GetFilteredSpaceTypes(searchString);
                }
                else
                {
                    spaceTypes = _unitOfWork.SpaceTypeRepository.GetAll();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.TypeSortParm = string.IsNullOrEmpty(sortOrder) ? "type_desc" : "";
            spaceTypes = _unitOfWork.SpaceTypeRepository.OrderSpaceTypes(spaceTypes, sortOrder);
            ViewBag.OnePageOfSpaceTypes = spaceTypes.ToPagedList(pageNumber, pageSize);
            return View(ViewBag.OnePageOfSpaceTypes);
        }

        // GET: SpaceType/Details/5
        public ActionResult Details(int id)
        {
            var spaceType = _unitOfWork.SpaceTypeRepository.Get(id);
            if (spaceType == null)
            {
                return HttpNotFound();
            }
            return View(spaceType);
        }

        // GET: SpaceType/Create
        public ActionResult Create()
        {
            var spaceType = new SpaceType();
            return View(spaceType);
        }

        // POST: SpaceType/Create
        [HttpPost]
        public ActionResult Create(SpaceType spaceType)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                var duplicateSpaceType = _unitOfWork.SpaceTypeRepository.FirstOrDefault(st => st.Type == spaceType.Type);
                if (duplicateSpaceType != null)
                {
                    return new HttpStatusCodeResult(409, "A space type with this type already exists.");
                }
                try
                {
                    _unitOfWork.SpaceTypeRepository.Add(spaceType);
                    _unitOfWork.Save();
                    TempData["message"] = $"Space type {spaceType.Type} has been created.";
                    return Json(spaceType.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(spaceType);
        }

        // GET: SpaceType/Edit/5
        public ActionResult Edit(int id)
        {
            var spaceType = _unitOfWork.SpaceTypeRepository.Get(id);
            if (spaceType == null)
            {
                return HttpNotFound();
            }
            return View(spaceType);
        }

        // POST: SpaceType/Edit/5
        [HttpPost]
        public ActionResult Edit(SpaceType spaceType)
        {
            var spaceTypeToUpdate = _unitOfWork.SpaceTypeRepository.Get(spaceType.ID);
            if (spaceTypeToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(spaceTypeToUpdate, "", new[] { "Type" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateSpaceType = _unitOfWork.SpaceTypeRepository.FirstOrDefault(st => st.Type == spaceTypeToUpdate.Type);
                    if (duplicateSpaceType != null && duplicateSpaceType.ID != spaceTypeToUpdate.ID)
                    {
                        return new HttpStatusCodeResult(409, "A space type with this type already exists.");
                    }
                    _unitOfWork.Save();
                    TempData["message"] = $"Space type {spaceTypeToUpdate.Type} has been edited.";
                    return Json(spaceTypeToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(spaceTypeToUpdate);
        }

        // GET: SpaceType/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var spaceType = _unitOfWork.SpaceTypeRepository.Get(id);
            if (spaceType == null)
            {
                return HttpNotFound();
            }
            return View(spaceType);
        }

        // POST: SpaceType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var spaceType = _unitOfWork.SpaceTypeRepository.Get(id);
                if (spaceType == null)
                {
                    return HttpNotFound();
                }
                _unitOfWork.SpaceTypeRepository.Remove(spaceType);
                _unitOfWork.Save();
                TempData["message"] = $"Space type {spaceType.Type} has been deleted.";
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
