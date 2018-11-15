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
            return View();
        }

        // POST: SpaceType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Type")] SpaceType spaceType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //uniqueness condition check
                    var duplicateSpaceType = _unitOfWork.SpaceTypeRepository.SingleOrDefault(st => st.Type == spaceType.Type);
                    if (duplicateSpaceType != null)
                    {
                        ModelState.AddModelError("Type", "A space type with this type already exists.");
                        return View(spaceType);
                    }
                    _unitOfWork.SpaceTypeRepository.Add(spaceType);
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Space type {0} has been created.", spaceType.Type);
                    return RedirectToAction("Index");
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int id)
        {
            var spaceType = _unitOfWork.SpaceTypeRepository.Get(id);
            if (spaceType == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(spaceType, "", new[] { "Type" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateSpaceType = _unitOfWork.SpaceTypeRepository.SingleOrDefault(st => st.Type == spaceType.Type);
                    if (duplicateSpaceType != null && duplicateSpaceType.ID != spaceType.ID)
                    {
                        ModelState.AddModelError("Type", "A space type with this type already exists.");
                        return View(spaceType);
                    }
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Space type {0} has been edited.", spaceType.Type);
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(spaceType);
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
                TempData["message"] = string.Format("Space type {0} has been deleted.", spaceType.Type);
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
