using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using X.PagedList;

namespace BuildingManagement.Controllers
{
    public class DistributionModeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DistributionModeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: DistributionMode
        public ActionResult Index(int? page, string currentFilter, string searchString, string sortOrder)
        {
            IEnumerable<DistributionMode> distributionModes;
            var pageNumber = page ?? 1;
            const int pageSize = 3;
            if (searchString != null)
            {
                pageNumber = 1;
                distributionModes = _unitOfWork.DistributionModeRepository.GetFilteredDistributionModes(searchString).ToList();
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    distributionModes = _unitOfWork.DistributionModeRepository.GetFilteredDistributionModes(searchString).ToList();
                }
                else
                {
                    distributionModes = _unitOfWork.DistributionModeRepository.GetAll().ToList();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ModeSortParm = string.IsNullOrEmpty(sortOrder) ? "mode_desc" : "";
            distributionModes = _unitOfWork.DistributionModeRepository.OrderDistributionModes(distributionModes, sortOrder).ToList();
            ViewBag.OnePageOfDistributionModes = distributionModes.ToPagedList(pageNumber, pageSize);
            return View(ViewBag.OnePageOfDistributionModes);
        }

        // GET: DistributionMode/Details/5
        public ActionResult Details(int id)
        {
            var distributionMode = _unitOfWork.DistributionModeRepository.Get(id);
            if (distributionMode == null)
            {
                return HttpNotFound();
            }
            return View(distributionMode);
        }

        // GET: DistributionMode/Create
        public ActionResult Create()
        {
            var distributionMode = new DistributionMode();
            return View(distributionMode);
        }

        // POST: DistributionMode/Create
        [HttpPost]
        public ActionResult Create(DistributionMode distributionMode)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                var duplicateDistributionMode = _unitOfWork.DistributionModeRepository.FirstOrDefault(dm => dm.Mode == distributionMode.Mode);
                if (duplicateDistributionMode != null)
                {
                    return new HttpStatusCodeResult(409, "A distribution mode with this mode already exists.");
                }
                try
                {
                    _unitOfWork.DistributionModeRepository.Add(distributionMode);
                    _unitOfWork.Save();
                    TempData["message"] = $"Distribution mode {distributionMode.Mode} has been created.";
                    return Json(distributionMode.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(distributionMode);
        }

        // GET: DistributionMode/Edit/5
        public ActionResult Edit(int id)
        {
            var distributionMode = _unitOfWork.DistributionModeRepository.Get(id);
            if (distributionMode == null)
            {
                return HttpNotFound();
            }
            return View(distributionMode);
        }

        // POST: DistributionMode/Edit/5
        [HttpPost]
        public ActionResult Edit(DistributionMode distributionMode)
        {
            var distributionModeToUpdate = _unitOfWork.DistributionModeRepository.Get(distributionMode.ID);
            if (distributionModeToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(distributionModeToUpdate, "", new[] {"Mode"}))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateDistributionMode = _unitOfWork.DistributionModeRepository.FirstOrDefault(dm => dm.Mode == distributionModeToUpdate.Mode);
                    if (duplicateDistributionMode != null && duplicateDistributionMode.ID != distributionModeToUpdate.ID)
                    {
                        return new HttpStatusCodeResult(409, "A distribution mode with this mode already exists.");
                    }
                    _unitOfWork.Save();
                    TempData["message"] = $"Distribution mode {distributionModeToUpdate.Mode} has been edited.";
                    return Json(distributionModeToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(distributionModeToUpdate);
        }

        // GET: DistributionMode/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var distributionMode = _unitOfWork.DistributionModeRepository.Get(id);
            if (distributionMode == null)
            {
                return HttpNotFound();
            }
            return View(distributionMode);
        }

        // POST: DistributionMode/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var distributionMode = _unitOfWork.DistributionModeRepository.Get(id);
                if (distributionMode == null)
                {
                    return HttpNotFound();
                }
                _unitOfWork.DistributionModeRepository.Remove(distributionMode);
                _unitOfWork.Save();
                TempData["message"] = $"Distribution mode {distributionMode.Mode} has been deleted.";
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
