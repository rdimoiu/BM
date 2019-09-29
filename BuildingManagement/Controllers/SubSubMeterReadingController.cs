using System;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using X.PagedList;

namespace BuildingManagement.Controllers
{
    public class SubSubMeterReadingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubSubMeterReadingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: SubSubMeterReading
        public ActionResult Index(int? page, string currentFilter, string searchString, string sortOrder)
        {
            IEnumerable<SubSubMeterReading> subSubMeterReadings;
            var pageNumber = page ?? 1;
            const int pageSize = 3;
            if (searchString != null)
            {
                pageNumber = 1;
                subSubMeterReadings = _unitOfWork.SubSubMeterReadingRepository.GetFilteredSubSubMeterReadingsIncludingSubSubMeterAndMeterType(searchString).ToList();
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    subSubMeterReadings = _unitOfWork.SubSubMeterReadingRepository.GetFilteredSubSubMeterReadingsIncludingSubSubMeterAndMeterType(searchString).ToList();
                }
                else
                {
                    subSubMeterReadings = _unitOfWork.SubSubMeterReadingRepository.GetAllSubSubMeterReadingsIncludingSubSubMeterAndMeterType().ToList();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IndexSortParm = string.IsNullOrEmpty(sortOrder) ? "index_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.DiscountMonthSortParm = sortOrder == "DiscountMonth" ? "discountMonth_desc" : "DiscountMonth";
            ViewBag.SubSubMeterSortParm = sortOrder == "SubSubMeter" ? "subSubMeter_desc" : "SubSubMeter";
            ViewBag.MeterTypeSortParm = sortOrder == "MeterType" ? "meterType_desc" : "MeterType";
            subSubMeterReadings = _unitOfWork.SubSubMeterReadingRepository.OrderSubSubMeterReadings(subSubMeterReadings, sortOrder).ToList();
            ViewBag.OnePageOfSubSubMeterReadings = subSubMeterReadings.ToPagedList(pageNumber, pageSize);
            return View(ViewBag.OnePageOfSubSubMeterReadings);
        }

        // GET: SubSubMeterReading/Details/5
        public ActionResult Details(int id)
        {
            var subSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.Get(id);
            if (subSubMeterReading == null)
            {
                return HttpNotFound();
            }
            return View(subSubMeterReading);
        }

        // GET: SubSubMeterReading/Create
        public ActionResult Create()
        {
            var subSubMeterReading = new SubSubMeterReading();
            PopulateSubSubMetersDropDownList();
            return View(subSubMeterReading);
        }

        // POST: SubSubMeterReading/Create
        [HttpPost]
        public ActionResult Create(SubSubMeterReading subSubMeterReading)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                var initialSubSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.FirstOrDefault(ssmr => ssmr.SubSubMeterID == subSubMeterReading.SubSubMeterID && ssmr.MeterTypeID == subSubMeterReading.MeterTypeID && ssmr.Initial);
                if (initialSubSubMeterReading != null)
                {
                    if (subSubMeterReading.Initial)
                    {
                        PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                        PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, $"An initial subsubmeter reading already exists. ({initialSubSubMeterReading.Date:dd/MM/yyyy} | {initialSubSubMeterReading.Index})");
                    }
                    if (initialSubSubMeterReading.Date >= subSubMeterReading.Date || initialSubSubMeterReading.Index > subSubMeterReading.Index)
                    {
                        PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                        PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, $"An initial subsubmeter reading on the same or later date or with a greater index already exists. ({initialSubSubMeterReading.Date:dd/MM/yyyy} | {initialSubSubMeterReading.Index})");
                    }
                    var soonerDateSubSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.FirstOrDefault(ssmr => ssmr.Date <= subSubMeterReading.Date && ssmr.Index > subSubMeterReading.Index && ssmr.SubSubMeterID == subSubMeterReading.SubSubMeterID && ssmr.MeterTypeID == subSubMeterReading.MeterTypeID);
                    if (soonerDateSubSubMeterReading != null)
                    {
                        PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                        PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, $"A subsubmeter reading on the same or sooner date and with a greater index already exists. ({soonerDateSubSubMeterReading.Date:dd/MM/yyyy} | {soonerDateSubSubMeterReading.Index})");
                    }
                    var lowerIndexSubSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.FirstOrDefault(ssmr => ssmr.Date >= subSubMeterReading.Date && ssmr.Index < subSubMeterReading.Index && ssmr.SubSubMeterID == subSubMeterReading.SubSubMeterID && ssmr.MeterTypeID == subSubMeterReading.MeterTypeID);
                    if (lowerIndexSubSubMeterReading != null)
                    {
                        PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                        PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, $"A subsubmeter reading on the same or later date and with a smaller index already exists. ({lowerIndexSubSubMeterReading.Date:dd/MM/yyyy} | {lowerIndexSubSubMeterReading.Index})");
                    }
                    var sameDiscountMonthSubSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.GetSubSubMeterReadingByDiscountMonth(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID, (DateTime)subSubMeterReading.DiscountMonth);
                    if (sameDiscountMonthSubSubMeterReading != null)
                    {
                        PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                        PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, $"A subsubmeter reading with the same discount month already exists. ({(DateTime)sameDiscountMonthSubSubMeterReading.DiscountMonth:dd/MM/yyyy})");
                    }
                    if (!CheckParentConsumption(subSubMeterReading))
                    {
                        PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                        PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, "This subsubmeter plus its siblings consumption is greater than parent submeter consumption.");
                    }
                }
                else
                {
                    if (!subSubMeterReading.Initial)
                    {
                        PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                        PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, "Add an initial subsubmeter reading first.");
                    }
                    var soonerDateOrLowerIndexSubSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.FirstOrDefault(ssmr => ssmr.SubSubMeterID == subSubMeterReading.SubSubMeterID && ssmr.MeterTypeID == subSubMeterReading.MeterTypeID && (ssmr.Date <= subSubMeterReading.Date || ssmr.Index < subSubMeterReading.Index));
                    if (soonerDateOrLowerIndexSubSubMeterReading != null)
                    {
                        PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                        PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, $"A subsubmeter reading on the same or sooner date or with a smaller index already exists. ({soonerDateOrLowerIndexSubSubMeterReading.Date:dd/MM/yyyy} | {soonerDateOrLowerIndexSubSubMeterReading.Index})");
                    }
                }
                try
                {
                    _unitOfWork.SubSubMeterReadingRepository.Add(subSubMeterReading);
                    _unitOfWork.Save();
                    TempData["message"] = $"SubSubMeterReading {subSubMeterReading.Index} has been created.";
                    return Json(subSubMeterReading.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
            return View(subSubMeterReading);
        }

        // GET: SubSubMeterReading/Edit/5
        public ActionResult Edit(int id)
        {
            var subSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.Get(id);
            if (subSubMeterReading == null)
            {
                return HttpNotFound();
            }
            PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
            return View(subSubMeterReading);
        }

        // POST: SubSubMeterReading/Edit/5
        [HttpPost]
        public ActionResult Edit(SubSubMeterReading subSubMeterReading)
        {
            var subSubMeterReadingToUpdate = _unitOfWork.SubSubMeterReadingRepository.Get(subSubMeterReading.ID);
            if (subSubMeterReadingToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(subSubMeterReadingToUpdate, "", new[] { "Index", "Date", "SubSubMeterID", "MeterTypeID", "DiscountMonth", "Initial", "Estimated" }))
            {
                try
                {
                    //uniqueness condition check
                    var initialSubSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.FirstOrDefault(ssmr => ssmr.ID != subSubMeterReading.ID && ssmr.SubSubMeterID == subSubMeterReading.SubSubMeterID && ssmr.MeterTypeID == subSubMeterReading.MeterTypeID && ssmr.Initial);
                    if (initialSubSubMeterReading != null)
                    {
                        if (subSubMeterReading.Initial)
                        {
                            PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                            PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, $"An initial subsubmeter reading already exists. ({initialSubSubMeterReading.Date:dd/MM/yyyy} | {initialSubSubMeterReading.Index})");
                        }
                        if (initialSubSubMeterReading.Date >= subSubMeterReading.Date || initialSubSubMeterReading.Index > subSubMeterReading.Index)
                        {
                            PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                            PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, $"An initial subsubmeter reading on the same or later date or with a greater index already exists. ({initialSubSubMeterReading.Date:dd/MM/yyyy} | {initialSubSubMeterReading.Index})");
                        }
                        var soonerDateSubSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.FirstOrDefault(ssmr => ssmr.ID != subSubMeterReading.ID && ssmr.Date <= subSubMeterReading.Date && ssmr.Index > subSubMeterReading.Index && ssmr.SubSubMeterID == subSubMeterReading.SubSubMeterID && ssmr.MeterTypeID == subSubMeterReading.MeterTypeID);
                        if (soonerDateSubSubMeterReading != null)
                        {
                            PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                            PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, $"A subsubmeter reading on the same or sooner date and with a greater index already exists. ({soonerDateSubSubMeterReading.Date:dd/MM/yyyy} | {soonerDateSubSubMeterReading.Index})");
                        }
                        var lowerIndexSubSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.FirstOrDefault(ssmr => ssmr.ID != subSubMeterReading.ID && ssmr.Date >= subSubMeterReading.Date && ssmr.Index < subSubMeterReading.Index && ssmr.SubSubMeterID == subSubMeterReading.SubSubMeterID && ssmr.MeterTypeID == subSubMeterReading.MeterTypeID);
                        if (lowerIndexSubSubMeterReading != null)
                        {
                            PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                            PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, $"A subsubmeter reading on the same or later date and with a smaller index already exists. ({lowerIndexSubSubMeterReading.Date:dd/MM/yyyy} | {lowerIndexSubSubMeterReading.Index})");
                        }
                        var sameDiscountMonthSubSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.GetSubSubMeterReadingByDiscountMonth(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID, (DateTime)subSubMeterReading.DiscountMonth);
                        if (sameDiscountMonthSubSubMeterReading != null)
                        {
                            PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                            PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, $"A subsubmeter reading with the same discount month already exists. ({(DateTime)sameDiscountMonthSubSubMeterReading.DiscountMonth:dd/MM/yyyy})");
                        }
                        if (!CheckParentConsumption(subSubMeterReading))
                        {
                            PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                            PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, "This subsubmeter plus its siblings consumption is greater than parent submeter consumption.");
                        }
                    }
                    else
                    {
                        if (!subSubMeterReading.Initial)
                        {
                            PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                            PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, "Add an initial subsubmeter reading first.");
                        }
                        var soonerDateOrLowerIndexSubSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.FirstOrDefault(ssmr => ssmr.ID != subSubMeterReading.ID && ssmr.SubSubMeterID == subSubMeterReading.SubSubMeterID && ssmr.MeterTypeID == subSubMeterReading.MeterTypeID && (ssmr.Date <= subSubMeterReading.Date || ssmr.Index < subSubMeterReading.Index));
                        if (soonerDateOrLowerIndexSubSubMeterReading != null)
                        {
                            PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                            PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, $"A subsubmeter reading on the same or sooner date or with a smaller index already exists. {soonerDateOrLowerIndexSubSubMeterReading.Date:dd/MM/yyyy} | {soonerDateOrLowerIndexSubSubMeterReading.Index}");
                        }
                    }
                    _unitOfWork.Save();
                    TempData["message"] = $"SubSubMeterReading {subSubMeterReadingToUpdate.Index} has been edited.";
                    return Json(subSubMeterReadingToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateSubSubMetersDropDownList(subSubMeterReadingToUpdate.SubSubMeterID);
            PopulateMeterTypesDropDownList(subSubMeterReadingToUpdate.SubSubMeterID, subSubMeterReadingToUpdate.MeterTypeID);
            return View(subSubMeterReadingToUpdate);
        }

        // GET: SubSubMeterReading/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var subSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.Get(id);
            if (subSubMeterReading == null)
            {
                return HttpNotFound();
            }
            return View(subSubMeterReading);
        }

        // POST: SubSubMeterReading/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var subSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.Get(id);
                if (subSubMeterReading == null)
                {
                    return HttpNotFound();
                }
                _unitOfWork.SubSubMeterReadingRepository.Remove(subSubMeterReading);
                _unitOfWork.Save();
                TempData["message"] = $"SubSubMeterReading {subSubMeterReading.Index} has been deleted.";
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

        private void PopulateSubSubMetersDropDownList(object selectedSubSubMeter = null)
        {
            var subSubMeters = _unitOfWork.SubSubMeterRepository.GetAllNoDefect().ToList();
            ViewBag.SubSubMeterID = new SelectList(subSubMeters, "ID", "Code", selectedSubSubMeter);
        }

        private void PopulateMeterTypesDropDownList(int subSubMeterId, int? meterTypeId)
        {
            ViewBag.MeterTypeID = GetMeterTypesBySubSubMeter(subSubMeterId, meterTypeId);
        }

        [HttpPost]
        public ActionResult GetMeterTypes(int subSubMeterId, int? meterTypeId)
        {
            return Json(GetMeterTypesBySubSubMeter(subSubMeterId, meterTypeId));
        }

        private List<SelectListItem> GetMeterTypesBySubSubMeter(int subSubMeterId, int? meterTypeId)
        {
            var list = new List<SelectListItem>();
            var subSubMeter = _unitOfWork.SubSubMeterRepository.GetSubSubMeterIncludingMeterTypes(subSubMeterId);
            if (subSubMeter != null)
            {
                foreach (var meterType in subSubMeter.MeterTypes)
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

        private decimal GetSubSubMeterConsumption(int subSubMeterId, int meterTypeId, DateTime? discountMonth, decimal index)
        {
            var consumption = 0.0m;
            var previousReading = _unitOfWork.SubSubMeterReadingRepository.GetPreviousSubSubMeterReading(subSubMeterId, meterTypeId, (DateTime)discountMonth);
            if (previousReading != null)
            {
                consumption = index - previousReading.Index;
            }
            else
            {
                var initialReading = _unitOfWork.SubSubMeterReadingRepository.GetInitialSubSubMeterReading(subSubMeterId, meterTypeId);
                if (initialReading != null)
                {
                    consumption = index - initialReading.Index;
                }
            }

            return consumption;
        }

        private bool CheckParentConsumption(SubSubMeterReading subSubMeterReading)
        {
            var siblingsConsumption = GetSubSubMeterConsumption(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID, subSubMeterReading.DiscountMonth, subSubMeterReading.Index);
            var subSubMeter = _unitOfWork.SubSubMeterRepository.Get(subSubMeterReading.SubSubMeterID);
            List<int> siblingIds = _unitOfWork.SubSubMeterRepository.GetSubSubMeterIDsBySubMeterIDNoDefect(subSubMeter.SubMeterID).ToList();
            foreach (var siblingId in siblingIds)
            {
                var actualSiblingReading = _unitOfWork.SubSubMeterReadingRepository.GetSubSubMeterReadingByDiscountMonth(siblingId, subSubMeterReading.MeterTypeID, (DateTime)subSubMeterReading.DiscountMonth);
                if (actualSiblingReading != null)
                {
                    var previousSiblingReading = _unitOfWork.SubSubMeterReadingRepository.GetPreviousSubSubMeterReading(siblingId, subSubMeterReading.MeterTypeID, (DateTime)subSubMeterReading.DiscountMonth);
                    if (previousSiblingReading != null)
                    {
                        siblingsConsumption += actualSiblingReading.Index - previousSiblingReading.Index;
                    }
                    else
                    {
                        var initialSiblingReading = _unitOfWork.SubSubMeterReadingRepository.GetInitialSubSubMeterReading(siblingId, subSubMeterReading.MeterTypeID);
                        if (initialSiblingReading != null)
                        {
                            siblingsConsumption += actualSiblingReading.Index - initialSiblingReading.Index;
                        }
                    }
                }
            }

            var parentConsumption = 0.0m;
            var actualParentReading = _unitOfWork.SubMeterReadingRepository.GetSubMeterReadingByDiscountMonth(subSubMeter.SubMeterID, subSubMeterReading.MeterTypeID, (DateTime)subSubMeterReading.DiscountMonth);
            if (actualParentReading != null)
            {
                var previousParentReading = _unitOfWork.SubMeterReadingRepository.GetPreviousSubMeterReading(subSubMeter.SubMeterID, subSubMeterReading.MeterTypeID, (DateTime)subSubMeterReading.DiscountMonth);
                if (previousParentReading != null)
                {
                    parentConsumption += actualParentReading.Index - previousParentReading.Index;
                }
                else
                {
                    var initialParentReading = _unitOfWork.SubMeterReadingRepository.GetInitialSubMeterReading(subSubMeter.SubMeterID, subSubMeterReading.MeterTypeID);
                    if (initialParentReading != null)
                    {
                        parentConsumption += actualParentReading.Index - initialParentReading.Index;
                    }
                }
            }

            if (parentConsumption < siblingsConsumption)
            {
                return false;
            }
            return true;
        }
    }
}