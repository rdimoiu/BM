using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using X.PagedList;

namespace BuildingManagement.Controllers
{
    public class MeterReadingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MeterReadingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: MeterReading
        public ActionResult Index(int? page, string currentFilter, string searchString, string sortOrder)
        {
            IEnumerable<MeterReading> meterReadings;
            var pageNumber = page ?? 1;
            const int pageSize = 3;
            if (searchString != null)
            {
                pageNumber = 1;
                meterReadings = _unitOfWork.MeterReadingRepository.GetFilteredMeterReadingsIncludingMeterAndMeterType(searchString);
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    meterReadings = _unitOfWork.MeterReadingRepository.GetFilteredMeterReadingsIncludingMeterAndMeterType(searchString);
                }
                else
                {
                    meterReadings = _unitOfWork.MeterReadingRepository.GetAllMeterReadingsIncludingMeterAndMeterType();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IndexSortParm = string.IsNullOrEmpty(sortOrder) ? "index_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.MeterSortParm = sortOrder == "Meter" ? "meter_desc" : "Meter";
            ViewBag.MeterTypeSortParm = sortOrder == "MeterType" ? "meterType_desc" : "MeterType";
            meterReadings = _unitOfWork.MeterReadingRepository.OrderMeterReadings(meterReadings, sortOrder);
            ViewBag.OnePageOfMeterReadings = meterReadings.ToPagedList(pageNumber, pageSize);
            return View(ViewBag.OnePageOfMeterReadings);
        }

        // GET: MeterReading/Details/5
        public ActionResult Details(int id)
        {
            var meterReading = _unitOfWork.MeterReadingRepository.Get(id);
            if (meterReading == null)
            {
                return HttpNotFound();
            }
            return View(meterReading);
        }

        // GET: MeterReading/Create
        public ActionResult Create()
        {
            PopulateMetersDropDownList();
            return View();
        }

        // POST: MeterReading/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Index,Date,MeterID,MeterTypeID")] MeterReading meterReading)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //uniqueness condition check
                    var duplicateMeterReading = _unitOfWork.MeterReadingRepository.SingleOrDefault(mr => mr.Date >= meterReading.Date && mr.MeterID == meterReading.MeterID && mr.MeterTypeID == meterReading.MeterTypeID);
                    if (duplicateMeterReading != null)
                    {
                        ModelState.AddModelError("Date", "A meter reading on the same or later date already exists.");
                        PopulateMetersDropDownList(meterReading.MeterID);
                        PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                        return View(meterReading);
                    }
                    var greaterMeterReadings = _unitOfWork.MeterReadingRepository.SingleOrDefault(mr => mr.MeterID == meterReading.MeterID && mr.MeterTypeID == meterReading.MeterTypeID && mr.Index > meterReading.Index);
                    if (greaterMeterReadings != null)
                    {
                        ModelState.AddModelError("Index", "A meter reading with the same or greater index already exists.");
                        PopulateMetersDropDownList(meterReading.MeterID);
                        PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                        return View(meterReading);
                    }
                    _unitOfWork.MeterReadingRepository.Add(meterReading);
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("MeterReading {0} has been created.", meterReading.Index);
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateMetersDropDownList(meterReading.MeterID);
            return View(meterReading);
        }

        // GET: MeterReading/Edit/5
        public ActionResult Edit(int id)
        {
            var meterReading = _unitOfWork.MeterReadingRepository.Get(id);
            if (meterReading == null)
            {
                return HttpNotFound();
            }
            PopulateMetersDropDownList(meterReading.MeterID);
            return View(meterReading);
        }

        // POST: MeterReading/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int id)
        {
            var meterReading = _unitOfWork.MeterReadingRepository.Get(id);
            if (meterReading == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(meterReading, "", new[] { "Index", "Date", "MeterID", "MeterTypeID" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateMeterReading = _unitOfWork.MeterReadingRepository.SingleOrDefault(mr => mr.Date == meterReading.Date && mr.MeterID == meterReading.MeterID && mr.MeterTypeID == meterReading.MeterTypeID);
                    if (duplicateMeterReading != null && duplicateMeterReading.ID != meterReading.ID)
                    {
                        ModelState.AddModelError("Date", "A meter reading on the same or later date already exists.");
                        PopulateMetersDropDownList(meterReading.MeterID);
                        PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                        return View(meterReading);
                    }
                    var greaterMeterReading = _unitOfWork.MeterReadingRepository.SingleOrDefault(mr => mr.MeterID == meterReading.MeterID && mr.MeterTypeID == meterReading.MeterTypeID && mr.Index > meterReading.Index);
                    if (greaterMeterReading != null && greaterMeterReading.ID != meterReading.ID)
                    {
                        ModelState.AddModelError("Index", "A meter reading with the same or greater index already exists.");
                        PopulateMetersDropDownList(meterReading.MeterID);
                        PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                        return View(meterReading);
                    }
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("MeterReading {0} has been edited.", meterReading.Index);
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateMetersDropDownList(meterReading.MeterID);
            PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
            return View(meterReading);
        }

        // GET: MeterReading/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var meterReading = _unitOfWork.MeterReadingRepository.Get(id);
            if (meterReading == null)
            {
                return HttpNotFound();
            }
            return View(meterReading);
        }

        // POST: MeterReading/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var meterReading = _unitOfWork.MeterReadingRepository.Get(id);
                if (meterReading == null)
                {
                    return HttpNotFound();
                }
                _unitOfWork.MeterReadingRepository.Remove(meterReading);
                _unitOfWork.Save();
                TempData["message"] = string.Format("MeterReading {0} has been deleted.", meterReading.Index);
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

        private void PopulateMetersDropDownList(object selectedMeter = null)
        {
            var metersQuery = from m in _unitOfWork.MeterRepository.GetAll() select m;
            ViewBag.MeterID = new SelectList(metersQuery, "ID", "Code", selectedMeter);
        }

        private void PopulateMeterTypesDropDownList(int meterId, int meterTypeId)
        {
            ViewBag.MeterTypeID = GetMeterTypesByMeter(meterId, meterTypeId);
        }

        [HttpPost]
        public ActionResult GetMeterTypes(int meterId, int? meterTypeId)
        {
            return Json(GetMeterTypesByMeter(meterId, meterTypeId));
        }

        private List<SelectListItem> GetMeterTypesByMeter(int meterId, int? meterTypeId)
        {
            var list = new List<SelectListItem>();
            var meter = _unitOfWork.MeterRepository.GetMeterIncludingMeterTypes(meterId);
            if (meter != null)
            {
                foreach (var meterType in meter.MeterTypes)
                {
                    if (meterTypeId != null && meterTypeId == meterType.ID)
                    {
                        list.Add(new SelectListItem { Value = meterType.ID.ToString(), Text = meterType.Type, Selected = true });
                    }
                    else
                    {
                        list.Add(new SelectListItem { Value = meterType.ID.ToString(), Text = meterType.Type });
                    }
                }
            }
            return list;
        }
    }
}
