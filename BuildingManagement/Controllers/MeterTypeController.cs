using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
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
                meterTypes = _unitOfWork.MeterTypeRepository.GetFilteredMeterTypes(searchString);
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    meterTypes = _unitOfWork.MeterTypeRepository.GetFilteredMeterTypes(searchString);
                }
                else
                {
                    meterTypes = _unitOfWork.MeterTypeRepository.GetAll();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.TypeSortParm = string.IsNullOrEmpty(sortOrder) ? "type_desc" : "";
            meterTypes = _unitOfWork.MeterTypeRepository.OrderMeterTypes(meterTypes, sortOrder);
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
            return View();
        }

        // POST: MeterType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Type")] MeterType meterType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //uniqueness condition check
                    var duplicateMeterType = _unitOfWork.MeterTypeRepository.SingleOrDefault(mt => mt.Type == meterType.Type);
                    if (duplicateMeterType != null)
                    {
                        ModelState.AddModelError("Type", "A meter type with this type already exists.");
                        return View(duplicateMeterType);
                    }
                    _unitOfWork.MeterTypeRepository.Add(meterType);
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Meter type {0} has been created.", meterType.Type);
                    return RedirectToAction("Index");
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int id)
        {
            var meterType = _unitOfWork.MeterTypeRepository.Get(id);
            if (meterType == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(meterType, "", new[] {"Type"}))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateMeterType = _unitOfWork.MeterTypeRepository.SingleOrDefault(mt => mt.Type == meterType.Type);
                    if (duplicateMeterType != null && duplicateMeterType.ID != meterType.ID)
                    {
                        ModelState.AddModelError("Type", "A meter type with this type already exists.");
                        return View(duplicateMeterType);
                    }
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Meter type {0} has been edited.", meterType.Type);
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(meterType);
        }

        // GET: MeterType/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError=false)
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
        [HttpPost]
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
                TempData["message"] = string.Format("Meter type {0} has been deleted.", meterType.Type);
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new {id, saveChangesError = true });
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
