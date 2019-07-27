using BuildingManagement.DAL;
using BuildingManagement.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using X.PagedList;

namespace BuildingManagement.Controllers
{
    public class MeterTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MeterTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: MeterType
        public ActionResult Index(int? page, string currentFilter, string searchString, string sortOrder)
        {
            IEnumerable<MeterType> meterTypes;
            var pageNumber = page ?? 1;
            const int pageSize = 3;
            if (searchString != null)
            {
                pageNumber = 1;
                meterTypes = _unitOfWork.MeterTypeRepository.GetFilteredMeterTypes(searchString).ToList();
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    meterTypes = _unitOfWork.MeterTypeRepository.GetFilteredMeterTypes(searchString).ToList();
                }
                else
                {
                    meterTypes = _unitOfWork.MeterTypeRepository.GetAll().ToList();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.TypeSortParm = string.IsNullOrEmpty(sortOrder) ? "type_desc" : "";
            meterTypes = _unitOfWork.MeterTypeRepository.OrderMeterTypes(meterTypes, sortOrder).ToList();
            ViewBag.OnePageOfMeterTypes = meterTypes.ToPagedList(pageNumber, pageSize);
            return View(ViewBag.OnePageOfMeterTypes);
        }

        // GET: MeterType/Details/5
        public ActionResult Details(int id)
        {
            var meterType = _unitOfWork.MeterTypeRepository.Get(id);
            if (meterType == null)
            {
                return HttpNotFound();
            }
            return View(meterType);
        }

        // GET: MeterType/Create
        public ActionResult Create()
        {
            var meterType = new MeterType();
            return View(meterType);
        }

        // POST: MeterType/Create
        [HttpPost]
        public ActionResult Create(MeterType meterType)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                var duplicateMeterType = _unitOfWork.MeterTypeRepository.FirstOrDefault(mt => mt.Type == meterType.Type);
                if (duplicateMeterType != null)
                {
                    return new HttpStatusCodeResult(409, "A meter type with this type already exists.");
                }
                try
                {
                    _unitOfWork.MeterTypeRepository.Add(meterType);
                    _unitOfWork.Save();
                    TempData["message"] = $"Meter type {meterType.Type} has been created.";
                    return Json(meterType.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(meterType);
        }

        // GET: MeterType/Edit/5
        public ActionResult Edit(int id)
        {
            var meterType = _unitOfWork.MeterTypeRepository.Get(id);
            if (meterType == null)
            {
                return HttpNotFound();
            }
            return View(meterType);
        }

        // POST: MeterType/Edit/5
        [HttpPost]
        public ActionResult Edit(MeterType meterType)
        {
            var meterTypeToUpdate = _unitOfWork.MeterTypeRepository.Get(meterType.ID);
            if (meterTypeToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(meterTypeToUpdate, "", new[] { "Type" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateMeterType = _unitOfWork.MeterTypeRepository.FirstOrDefault(mt => mt.ID != meterTypeToUpdate.ID && mt.Type == meterTypeToUpdate.Type);
                    if (duplicateMeterType != null)
                    {
                        return new HttpStatusCodeResult(409, "A meter type with this type already exists.");
                    }
                    _unitOfWork.Save();
                    TempData["message"] = $"Meter type {meterTypeToUpdate.Type} has been edited.";
                    return Json(meterTypeToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(meterTypeToUpdate);
        }

        // GET: MeterType/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var meterType = _unitOfWork.MeterTypeRepository.Get(id);
            if (meterType == null)
            {
                return HttpNotFound();
            }
            return View(meterType);
        }

        // POST: MeterType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var meterType = _unitOfWork.MeterTypeRepository.Get(id);
                if (meterType == null)
                {
                    return HttpNotFound();
                }
                _unitOfWork.MeterTypeRepository.Remove(meterType);
                _unitOfWork.Save();
                TempData["message"] = $"Meter type {meterType.Type} has been deleted.";
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
