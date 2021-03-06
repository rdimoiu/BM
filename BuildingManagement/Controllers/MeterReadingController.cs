using System;
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
            const int pageSize = 10;
            if (searchString != null)
            {
                pageNumber = 1;
                meterReadings = _unitOfWork.MeterReadingRepository.GetFilteredMeterReadingsIncludingMeterAndMeterType(searchString).ToList();
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    meterReadings = _unitOfWork.MeterReadingRepository.GetFilteredMeterReadingsIncludingMeterAndMeterType(searchString).ToList();
                }
                else
                {
                    meterReadings = _unitOfWork.MeterReadingRepository.GetAllMeterReadingsIncludingMeterAndMeterType().ToList();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IndexSortParm = string.IsNullOrEmpty(sortOrder) ? "index_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.DiscountMonthSortParm = sortOrder == "DiscountMonth" ? "discountMonth_desc" : "DiscountMonth";
            ViewBag.MeterSortParm = sortOrder == "Meter" ? "meter_desc" : "Meter";
            ViewBag.MeterTypeSortParm = sortOrder == "MeterType" ? "meterType_desc" : "MeterType";
            meterReadings = _unitOfWork.MeterReadingRepository.OrderMeterReadings(meterReadings, sortOrder).ToList();
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
            var meterReading = new MeterReading();
            PopulateMetersDropDownList();
            return View(meterReading);
        }

        // POST: MeterReading/Create
        [HttpPost]
        public ActionResult Create(MeterReading meterReading)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                var initialMeterReading = _unitOfWork.MeterReadingRepository.FirstOrDefault(mr => mr.MeterID == meterReading.MeterID && mr.MeterTypeID == meterReading.MeterTypeID && mr.Initial);
                if (initialMeterReading != null)
                {
                    if (meterReading.Initial)
                    {
                        PopulateMetersDropDownList(meterReading.MeterID);
                        PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, $"An initial meter reading already exists. ({initialMeterReading.Date:dd/MM/yyyy} | {initialMeterReading.Index})");
                    }
                    if (initialMeterReading.Date >= meterReading.Date || initialMeterReading.Index > meterReading.Index)
                    {
                        PopulateMetersDropDownList(meterReading.MeterID);
                        PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, $"An initial meter reading on the same or later date or with a greater index already exists. ({initialMeterReading.Date:dd/MM/yyyy} | {initialMeterReading.Index})");
                    }
                    var soonerDateMeterReading = _unitOfWork.MeterReadingRepository.FirstOrDefault(mr => mr.Date <= meterReading.Date && mr.Index > meterReading.Index && mr.MeterID == meterReading.MeterID && mr.MeterTypeID == meterReading.MeterTypeID);
                    if (soonerDateMeterReading != null)
                    {
                        PopulateMetersDropDownList(meterReading.MeterID);
                        PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, $"A meter reading on the same or sooner date and with a greater index already exists. ({soonerDateMeterReading.Date:dd/MM/yyyy} | {soonerDateMeterReading.Index})");
                    }
                    var lowerIndexMeterReading = _unitOfWork.MeterReadingRepository.FirstOrDefault(mr => mr.Date >= meterReading.Date && mr.Index < meterReading.Index && mr.MeterID == meterReading.MeterID && mr.MeterTypeID == meterReading.MeterTypeID);
                    if (lowerIndexMeterReading != null)
                    {
                        PopulateMetersDropDownList(meterReading.MeterID);
                        PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, $"A meter reading on the same or later date and with a smaller index already exists. ({lowerIndexMeterReading.Date:dd/MM/yyyy} | {lowerIndexMeterReading.Index})");
                    }
                    var sameDiscountMonthMeterReading = _unitOfWork.MeterReadingRepository.GetMeterReadingByDiscountMonth(meterReading.MeterID, meterReading.MeterTypeID, (DateTime)meterReading.DiscountMonth);
                    if (sameDiscountMonthMeterReading != null)
                    {
                        PopulateMetersDropDownList(meterReading.MeterID);
                        PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, $"A meter reading with the same discount month already exists. ({(DateTime)sameDiscountMonthMeterReading.DiscountMonth:dd/MM/yyyy})");
                    }
                    if (!CheckChildrenConsumption(meterReading))
                    {
                        PopulateMetersDropDownList(meterReading.MeterID);
                        PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, "Submeters consumption is greater than this meter consumption.");
                    }
                }
                else
                {
                    if (!meterReading.Initial)
                    {
                        PopulateMetersDropDownList(meterReading.MeterID);
                        PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, "Add an initial meter reading first.");
                    }
                    var soonerDateOrLowerIndexMeterReading = _unitOfWork.MeterReadingRepository.FirstOrDefault(mr => mr.MeterID == meterReading.MeterID && mr.MeterTypeID == meterReading.MeterTypeID && (mr.Date <= meterReading.Date || mr.Index < meterReading.Index));
                    if (soonerDateOrLowerIndexMeterReading != null)
                    {
                        PopulateMetersDropDownList(meterReading.MeterID);
                        PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, $"A meter reading on the same or sooner date or with a smaller index already exists. ({soonerDateOrLowerIndexMeterReading.Date:dd/MM/yyyy} | {soonerDateOrLowerIndexMeterReading.Index})");
                    }
                }
                try
                {
                    _unitOfWork.MeterReadingRepository.Add(meterReading);
                    _unitOfWork.Save();
                    TempData["message"] = $"MeterReading {meterReading.Index} has been created.";
                    return Json(meterReading.ID);
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
        [HttpPost]
        public ActionResult Edit(MeterReading meterReading)
        {
            var meterReadingToUpdate = _unitOfWork.MeterReadingRepository.Get(meterReading.ID);
            if (meterReadingToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(meterReadingToUpdate, "", new[] { "Index", "Date", "MeterID", "MeterTypeID", "DiscountMonth", "Initial", "Estimated" }))
            {
                try
                {
                    //uniqueness condition check
                    var initialMeterReading = _unitOfWork.MeterReadingRepository.FirstOrDefault(mr => mr.ID != meterReading.ID && mr.MeterID == meterReading.MeterID && mr.MeterTypeID == meterReading.MeterTypeID && mr.Initial);
                    if (initialMeterReading != null)
                    {
                        if (meterReading.Initial)
                        {
                            PopulateMetersDropDownList(meterReading.MeterID);
                            PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, $"An initial meter reading already exists. ({initialMeterReading.Date:dd/MM/yyyy} | {initialMeterReading.Index})");
                        }
                        if (initialMeterReading.Date >= meterReading.Date || initialMeterReading.Index > meterReading.Index)
                        {
                            PopulateMetersDropDownList(meterReading.MeterID);
                            PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, $"An initial meter reading on the same or later date or with a greater index already exists. ({initialMeterReading.Date:dd/MM/yyyy} | {initialMeterReading.Index})");
                        }
                        var soonerDateMeterReading = _unitOfWork.MeterReadingRepository.FirstOrDefault(mr => mr.ID != meterReading.ID && mr.Date <= meterReading.Date && mr.Index > meterReading.Index && mr.MeterID == meterReading.MeterID && mr.MeterTypeID == meterReading.MeterTypeID);
                        if (soonerDateMeterReading != null)
                        {
                            PopulateMetersDropDownList(meterReading.MeterID);
                            PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, $"A meter reading on the same or sooner date and with a greater index already exists. ({soonerDateMeterReading.Date:dd/MM/yyyy} | {soonerDateMeterReading.Index})");
                        }
                        var lowerIndexMeterReading = _unitOfWork.MeterReadingRepository.FirstOrDefault(mr => mr.ID != meterReading.ID && mr.Date >= meterReading.Date && mr.Index < meterReading.Index && mr.MeterID == meterReading.MeterID && mr.MeterTypeID == meterReading.MeterTypeID);
                        if (lowerIndexMeterReading != null)
                        {
                            PopulateMetersDropDownList(meterReading.MeterID);
                            PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, $"A meter reading on the same or later date and with a smaller index already exists. ({lowerIndexMeterReading.Date:dd/MM/yyyy} | {lowerIndexMeterReading.Index})");
                        }
                        var sameDiscountMonthMeterReading = _unitOfWork.MeterReadingRepository.GetMeterReadingByDiscountMonth(meterReading.MeterID, meterReading.MeterTypeID, (DateTime)meterReading.DiscountMonth);
                        if (sameDiscountMonthMeterReading != null)
                        {
                            PopulateMetersDropDownList(meterReading.MeterID);
                            PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, $"A meter reading with the same discount month already exists. ({(DateTime)sameDiscountMonthMeterReading.DiscountMonth:dd/MM/yyyy})");
                        }
                        if (!CheckChildrenConsumption(meterReading))
                        {
                            PopulateMetersDropDownList(meterReading.MeterID);
                            PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, "Submeters consumption is greater than this meter consumption.");
                        }
                    }
                    else
                    {
                        if (!meterReading.Initial)
                        {
                            PopulateMetersDropDownList(meterReading.MeterID);
                            PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, "Add an initial meter reading first.");
                        }
                        var soonerDateOrLowerIndexMeterReading = _unitOfWork.MeterReadingRepository.FirstOrDefault(mr => mr.ID != meterReading.ID && mr.MeterID == meterReading.MeterID && mr.MeterTypeID == meterReading.MeterTypeID && (mr.Date <= meterReading.Date || mr.Index < meterReading.Index));
                        if (soonerDateOrLowerIndexMeterReading != null)
                        {
                            PopulateMetersDropDownList(meterReading.MeterID);
                            PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, $"A meter reading on the same or sooner date or with a smaller index already exists. {soonerDateOrLowerIndexMeterReading.Date:dd/MM/yyyy} | {soonerDateOrLowerIndexMeterReading.Index}");
                        }
                    }
                    _unitOfWork.Save();
                    TempData["message"] = $"MeterReading {meterReadingToUpdate.Index} has been edited.";
                    return Json(meterReadingToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateMetersDropDownList(meterReadingToUpdate.MeterID);
            PopulateMeterTypesDropDownList(meterReadingToUpdate.MeterID, meterReadingToUpdate.MeterTypeID);
            return View(meterReadingToUpdate);
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
                TempData["message"] = $"MeterReading {meterReading.Index} has been deleted.";
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
            var metersQuery = _unitOfWork.MeterRepository.GetAllNoDefect().ToList();
            ViewBag.MeterID = new SelectList(metersQuery, "ID", "Code", selectedMeter);
        }

        private void PopulateMeterTypesDropDownList(int meterId, int? meterTypeId)
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

        private bool CheckChildrenConsumption(MeterReading meterReading)
        {
            var consumption = 0.0m;
            var previousReading = _unitOfWork.MeterReadingRepository.GetPreviousMeterReading(meterReading.MeterID, meterReading.MeterTypeID, (DateTime)meterReading.DiscountMonth);
            if (previousReading != null)
            {
                consumption = meterReading.Index - previousReading.Index;
            }
            else
            {
                var initialReading = _unitOfWork.MeterReadingRepository.GetInitialMeterReading(meterReading.MeterID, meterReading.MeterTypeID);
                if (initialReading != null)
                {
                    consumption = meterReading.Index - initialReading.Index;
                }
            }

            List<int> childIds = _unitOfWork.SubMeterRepository.GetSubMeterIDsByMeterIDNoDefect(meterReading.MeterID).ToList();
            var childrenConsumption = 0.0m;
            foreach (var childId in childIds)
            {
                var actualChildReading = _unitOfWork.SubMeterReadingRepository.GetSubMeterReadingByDiscountMonth(childId, meterReading.MeterTypeID, (DateTime)meterReading.DiscountMonth);
                if (actualChildReading != null)
                {
                    var previousChildReading = _unitOfWork.SubMeterReadingRepository.GetPreviousSubMeterReading(childId, meterReading.MeterTypeID, (DateTime)meterReading.DiscountMonth);
                    if (previousChildReading != null)
                    {
                        childrenConsumption += actualChildReading.Index - previousChildReading.Index;
                    }
                    else
                    {
                        var initialChildReading = _unitOfWork.SubMeterReadingRepository.GetInitialSubMeterReading(childId,meterReading.MeterTypeID);
                        if (initialChildReading != null)
                        {
                            childrenConsumption += actualChildReading.Index - initialChildReading.Index;
                        }
                    }
                }
            }

            if (consumption < childrenConsumption)
            {
                return false;
            }
            return true;
        }
    }
}
