using System.Collections.Generic;
using System.Data;
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
                distributionModes = _unitOfWork.DistributionModeRepository.GetFilteredDistributionModes(searchString);
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    distributionModes = _unitOfWork.DistributionModeRepository.GetFilteredDistributionModes(searchString);
                }
                else
                {
                    distributionModes = _unitOfWork.DistributionModeRepository.GetAll();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ModeSortParm = string.IsNullOrEmpty(sortOrder) ? "mode_desc" : "";
            distributionModes = _unitOfWork.DistributionModeRepository.OrderDistributionModes(distributionModes, sortOrder);
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
            return View();
        }

        // POST: DistributionMode/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Mode")] DistributionMode distributionMode)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //uniqueness condition check
                    var duplicateDistributionMode = _unitOfWork.DistributionModeRepository.SingleOrDefault(dm => dm.Mode == distributionMode.Mode);
                    if (duplicateDistributionMode != null)
                    {
                        ModelState.AddModelError("Mode", "A distribution mode with this mode already exists.");
                        return View(distributionMode);
                    }
                    _unitOfWork.DistributionModeRepository.Add(distributionMode);
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Distribution mode {0} has been created.", distributionMode.Mode);
                    return RedirectToAction("Index");
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int id)
        {
            var distributionMode = _unitOfWork.DistributionModeRepository.Get(id);
            if (distributionMode == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(distributionMode, "", new[] {"Mode"}))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateDistributionMode = _unitOfWork.DistributionModeRepository.SingleOrDefault(dm => dm.Mode == distributionMode.Mode);
                    if (duplicateDistributionMode != null && duplicateDistributionMode.ID != distributionMode.ID)
                    {
                        ModelState.AddModelError("Mode", "A distribution mode with this mode already exists.");
                        return View(distributionMode);
                    }
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Distribution mode {0} has been edited.", distributionMode.Mode);
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(distributionMode);
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
                TempData["message"] = string.Format("Distribution mode {0} has been deleted.", distributionMode.Mode);
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
