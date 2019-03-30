﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
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
                        return new HttpStatusCodeResult(409, "An initial subsubmeter reading already exists.");
                    }
                    if (initialSubSubMeterReading.Date >= subSubMeterReading.Date || initialSubSubMeterReading.Index >= subSubMeterReading.Index)
                    {
                        PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                        PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, "An initial subsubmeter reading on the same or later date or with the same or greater index already exists.");
                    }
                    var soonerDateSubSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.FirstOrDefault(ssmr => ssmr.Date <= subSubMeterReading.Date && ssmr.Index >= subSubMeterReading.Index && ssmr.SubSubMeterID == subSubMeterReading.SubSubMeterID && ssmr.MeterTypeID == subSubMeterReading.MeterTypeID);
                    if (soonerDateSubSubMeterReading != null)
                    {
                        PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                        PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, "A subsubmeter reading on the same or sooner date and with the same or higher index already exists.");
                    }
                    var lowerIndexSubSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.FirstOrDefault(ssmr => ssmr.Date >= subSubMeterReading.Date && ssmr.Index <= subSubMeterReading.Index && ssmr.SubSubMeterID == subSubMeterReading.SubSubMeterID && ssmr.MeterTypeID == subSubMeterReading.MeterTypeID);
                    if (lowerIndexSubSubMeterReading != null)
                    {
                        PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                        PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, "A subsubmeter reading on the same or later date and with the same or lower index already exists.");
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
                    var soonerDateOrLowerIndexSubSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.FirstOrDefault(ssmr => ssmr.SubSubMeterID == subSubMeterReading.SubSubMeterID && ssmr.MeterTypeID == subSubMeterReading.MeterTypeID && (ssmr.Date <= subSubMeterReading.Date || ssmr.Index <= subSubMeterReading.Index));
                    if (soonerDateOrLowerIndexSubSubMeterReading != null)
                    {
                        PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                        PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, "A subsubmeter reading on the same or sooner date or with the same or lower index already exists.");
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
                            return new HttpStatusCodeResult(409, "An initial subsubmeter reading already exists.");
                        }
                        if (initialSubSubMeterReading.Date >= subSubMeterReading.Date || initialSubSubMeterReading.Index >= subSubMeterReading.Index)
                        {
                            PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                            PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, "An initial subsubmeter reading on the same or later date or with the same or greater index already exists.");
                        }
                        var soonerDateSubSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.FirstOrDefault(ssmr => ssmr.ID != subSubMeterReading.ID && ssmr.Date <= subSubMeterReading.Date && ssmr.Index >= subSubMeterReading.Index && ssmr.SubSubMeterID == subSubMeterReading.SubSubMeterID && ssmr.MeterTypeID == subSubMeterReading.MeterTypeID);
                        if (soonerDateSubSubMeterReading != null)
                        {
                            PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                            PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, "A subsubmeter reading on the same or sooner date and with the same or higher index already exists.");
                        }
                        var lowerIndexSubSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.FirstOrDefault(ssmr => ssmr.ID != subSubMeterReading.ID && ssmr.Date >= subSubMeterReading.Date && ssmr.Index <= subSubMeterReading.Index && ssmr.SubSubMeterID == subSubMeterReading.SubSubMeterID && ssmr.MeterTypeID == subSubMeterReading.MeterTypeID);
                        if (lowerIndexSubSubMeterReading != null)
                        {
                            PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                            PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, "A subsubmeter reading on the same or later date and with the same or lower index already exists.");
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
                        var soonerDateOrLowerIndexSubSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.FirstOrDefault(ssmr => ssmr.ID != subSubMeterReading.ID && ssmr.SubSubMeterID == subSubMeterReading.SubSubMeterID && ssmr.MeterTypeID == subSubMeterReading.MeterTypeID && (ssmr.Date <= subSubMeterReading.Date || ssmr.Index <= subSubMeterReading.Index));
                        if (soonerDateOrLowerIndexSubSubMeterReading != null)
                        {
                            PopulateSubSubMetersDropDownList(subSubMeterReading.SubSubMeterID);
                            PopulateMeterTypesDropDownList(subSubMeterReading.SubSubMeterID, subSubMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, "A subsubmeter reading on the same or sooner date or with the same or lower index already exists.");
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
            var subSubMeters = _unitOfWork.SubSubMeterRepository.GetAll().ToList();
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
    }
}