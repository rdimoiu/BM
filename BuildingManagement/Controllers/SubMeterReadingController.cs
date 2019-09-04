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
    public class SubMeterReadingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubMeterReadingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: SubMeterReading
        public ActionResult Index(int? page, string currentFilter, string searchString, string sortOrder)
        {
            IEnumerable<SubMeterReading> subMeterReadings;
            var pageNumber = page ?? 1;
            const int pageSize = 3;
            if (searchString != null)
            {
                pageNumber = 1;
                subMeterReadings = _unitOfWork.SubMeterReadingRepository.GetFilteredSubMeterReadingsIncludingSubMeterAndMeterType(searchString).ToList();
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    subMeterReadings = _unitOfWork.SubMeterReadingRepository.GetFilteredSubMeterReadingsIncludingSubMeterAndMeterType(searchString).ToList();
                }
                else
                {
                    subMeterReadings = _unitOfWork.SubMeterReadingRepository.GetAllSubMeterReadingsIncludingSubMeterAndMeterType().ToList();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IndexSortParm = string.IsNullOrEmpty(sortOrder) ? "index_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.DiscountMonthSortParm = sortOrder == "DiscountMonth" ? "discountMonth_desc" : "DiscountMonth";
            ViewBag.SubMeterSortParm = sortOrder == "SubMeter" ? "subMeter_desc" : "SubMeter";
            ViewBag.MeterTypeSortParm = sortOrder == "MeterType" ? "meterType_desc" : "MeterType";
            subMeterReadings = _unitOfWork.SubMeterReadingRepository.OrderSubMeterReadings(subMeterReadings, sortOrder).ToList();
            ViewBag.OnePageOfSubMeterReadings = subMeterReadings.ToPagedList(pageNumber, pageSize);
            return View(ViewBag.OnePageOfSubMeterReadings);
        }

        // GET: SubMeterReading/Details/5
        public ActionResult Details(int id)
        {
            var subMeterReading = _unitOfWork.SubMeterReadingRepository.Get(id);
            if (subMeterReading == null)
            {
                return HttpNotFound();
            }
            return View(subMeterReading);
        }

        // GET: SubMeterReading/Create
        public ActionResult Create()
        {
            var subMeterReading = new SubMeterReading();
            PopulateSubMetersDropDownList();
            return View(subMeterReading);
        }

        // POST: SubMeterReading/Create
        [HttpPost]
        public ActionResult Create(SubMeterReading subMeterReading)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                var initialSubMeterReading = _unitOfWork.SubMeterReadingRepository.FirstOrDefault(smr => smr.SubMeterID == subMeterReading.SubMeterID && smr.MeterTypeID == subMeterReading.MeterTypeID && smr.Initial);
                if (initialSubMeterReading != null)
                {
                    if (subMeterReading.Initial)
                    {
                        PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                        PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, $"An initial submeter reading already exists. ({initialSubMeterReading.Date.ToString("dd/MM/yyyy")} | {initialSubMeterReading.Index})");
                    }
                    if (initialSubMeterReading.Date >= subMeterReading.Date || initialSubMeterReading.Index > subMeterReading.Index)
                    {
                        PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                        PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, $"An initial submeter reading on the same or later date or with a greater index already exists. ({initialSubMeterReading.Date.ToString("dd/MM/yyyy")} | {initialSubMeterReading.Index})");
                    }
                    var soonerDateSubMeterReading = _unitOfWork.SubMeterReadingRepository.FirstOrDefault(smr => smr.Date <= subMeterReading.Date && smr.Index > subMeterReading.Index && smr.SubMeterID == subMeterReading.SubMeterID && smr.MeterTypeID == subMeterReading.MeterTypeID);
                    if (soonerDateSubMeterReading != null)
                    {
                        PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                        PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, $"A submeter reading on the same or sooner date and with a greater index already exists. ({soonerDateSubMeterReading.Date.ToString("dd/MM/yyyy")} | {soonerDateSubMeterReading.Index})");
                    }
                    var lowerIndexSubMeterReading = _unitOfWork.SubMeterReadingRepository.FirstOrDefault(smr => smr.Date >= subMeterReading.Date && smr.Index < subMeterReading.Index && smr.SubMeterID == subMeterReading.SubMeterID && smr.MeterTypeID == subMeterReading.MeterTypeID);
                    if (lowerIndexSubMeterReading != null)
                    {
                        PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                        PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, $"A submeter reading on the same or later date and with a smaller index already exists. ({lowerIndexSubMeterReading.Date.ToString("dd/MM/yyyy")} | {lowerIndexSubMeterReading.Index})");
                    }
                    if (!CheckChildrenConsumption(subMeterReading))
                    {
                        PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                        PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, "Subsubmeters consumption is greater than this submeter consumption.");
                    }
                    if (!CheckParentConsumption(subMeterReading))
                    {
                        PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                        PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, "This submeter plus its siblings consumption is greater than parent meter consumption.");
                    }
                }
                else
                {
                    if (!subMeterReading.Initial)
                    {
                        PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                        PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, "Add an initial submeter reading first.");
                    }
                    var soonerDateOrLowerIndexSubMeterReading = _unitOfWork.SubMeterReadingRepository.FirstOrDefault(smr => smr.SubMeterID == subMeterReading.SubMeterID && smr.MeterTypeID == subMeterReading.MeterTypeID && (smr.Date <= subMeterReading.Date || smr.Index < subMeterReading.Index));
                    if (soonerDateOrLowerIndexSubMeterReading != null)
                    {
                        PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                        PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, $"A submeter reading on the same or sooner date or with a smaller index already exists. ({soonerDateOrLowerIndexSubMeterReading.Date.ToString("dd/MM/yyyy")} | {soonerDateOrLowerIndexSubMeterReading.Index})");
                    }
                }
                try
                {
                    _unitOfWork.SubMeterReadingRepository.Add(subMeterReading);
                    _unitOfWork.Save();
                    TempData["message"] = $"SubMeterReading {subMeterReading.Index} has been created.";
                    return Json(subMeterReading.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
            return View(subMeterReading);
        }

        // GET: SubMeterReading/Edit/5
        public ActionResult Edit(int id)
        {
            var subMeterReading = _unitOfWork.SubMeterReadingRepository.Get(id);
            if (subMeterReading == null)
            {
                return HttpNotFound();
            }
            PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
            return View(subMeterReading);
        }

        // POST: SubMeterReading/Edit/5
        [HttpPost]
        public ActionResult Edit(SubMeterReading subMeterReading)
        {
            var subMeterReadingToUpdate = _unitOfWork.SubMeterReadingRepository.Get(subMeterReading.ID);
            if (subMeterReadingToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(subMeterReadingToUpdate, "", new[] { "Index", "Date", "SubMeterID", "MeterTypeID", "DiscountMonth", "Initial", "Estimated" }))
            {
                try
                {
                    //uniqueness condition check
                    var initialSubMeterReading = _unitOfWork.SubMeterReadingRepository.FirstOrDefault(smr => smr.ID != subMeterReading.ID && smr.SubMeterID == subMeterReading.SubMeterID && smr.MeterTypeID == subMeterReading.MeterTypeID && smr.Initial);
                    if (initialSubMeterReading != null)
                    {
                        if (subMeterReading.Initial)
                        {
                            PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                            PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, $"An initial submeter reading already exists. ({initialSubMeterReading.Date.ToString("dd/MM/yyyy")} | {initialSubMeterReading.Index})");
                        }
                        if (initialSubMeterReading.Date >= subMeterReading.Date || initialSubMeterReading.Index > subMeterReading.Index)
                        {
                            PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                            PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, $"An initial submeter reading on the same or later date or with a greater index already exists. ({initialSubMeterReading.Date.ToString("dd/MM/yyyy")} | {initialSubMeterReading.Index})");
                        }
                        var soonerDateSubMeterReading = _unitOfWork.SubMeterReadingRepository.FirstOrDefault(smr => smr.ID != subMeterReading.ID && smr.Date <= subMeterReading.Date && smr.Index > subMeterReading.Index && smr.SubMeterID == subMeterReading.SubMeterID && smr.MeterTypeID == subMeterReading.MeterTypeID);
                        if (soonerDateSubMeterReading != null)
                        {
                            PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                            PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, $"A submeter reading on the same or sooner date and with a greater index already exists. ({soonerDateSubMeterReading.Date.ToString("dd/MM/yyyy")} | {soonerDateSubMeterReading.Index})");
                        }
                        var lowerIndexSubMeterReading = _unitOfWork.SubMeterReadingRepository.FirstOrDefault(smr => smr.ID != subMeterReading.ID && smr.Date >= subMeterReading.Date && smr.Index < subMeterReading.Index && smr.SubMeterID == subMeterReading.SubMeterID && smr.MeterTypeID == subMeterReading.MeterTypeID);
                        if (lowerIndexSubMeterReading != null)
                        {
                            PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                            PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, $"A submeter reading on the same or later date and with a smaller index already exists. ({lowerIndexSubMeterReading.Date.ToString("dd/MM/yyyy")} | {lowerIndexSubMeterReading.Index})");
                        }
                        if (!CheckChildrenConsumption(subMeterReading))
                        {
                            PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                            PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, "Subsubmeters consumption is greater than this submeter consumption.");
                        }
                        if (!CheckParentConsumption(subMeterReading))
                        {
                            PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                            PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, "This submeter plus its siblings consumption is greater than parent meter consumption.");
                        }
                    }
                    else
                    {
                        if (!subMeterReading.Initial)
                        {
                            PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                            PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, "Add an initial submeter reading first.");
                        }
                        var soonerDateOrLowerIndexSubMeterReading = _unitOfWork.SubMeterReadingRepository.FirstOrDefault(smr => smr.ID != subMeterReading.ID && smr.SubMeterID == subMeterReading.SubMeterID && smr.MeterTypeID == subMeterReading.MeterTypeID && (smr.Date <= subMeterReading.Date || smr.Index < subMeterReading.Index));
                        if (soonerDateOrLowerIndexSubMeterReading != null)
                        {
                            PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                            PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                            return new HttpStatusCodeResult(409, $"A submeter reading on the same or sooner date or with a smaller index already exists. {soonerDateOrLowerIndexSubMeterReading.Date.ToString("dd/MM/yyyy")} | {soonerDateOrLowerIndexSubMeterReading.Index}");
                        }
                    }
                    _unitOfWork.Save();
                    TempData["message"] = $"SubMeterReading {subMeterReadingToUpdate.Index} has been edited.";
                    return Json(subMeterReadingToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateSubMetersDropDownList(subMeterReadingToUpdate.SubMeterID);
            PopulateMeterTypesDropDownList(subMeterReadingToUpdate.SubMeterID, subMeterReadingToUpdate.MeterTypeID);
            return View(subMeterReadingToUpdate);
        }

        // GET: SubMeterReading/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var subMeterReading = _unitOfWork.SubMeterReadingRepository.Get(id);
            if (subMeterReading == null)
            {
                return HttpNotFound();
            }
            return View(subMeterReading);
        }

        // POST: SubMeterReading/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var subMeterReading = _unitOfWork.SubMeterReadingRepository.Get(id);
                if (subMeterReading == null)
                {
                    return HttpNotFound();
                }
                _unitOfWork.SubMeterReadingRepository.Remove(subMeterReading);
                _unitOfWork.Save();
                TempData["message"] = $"SubMeterReading {subMeterReading.Index} has been deleted.";
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

        private void PopulateSubMetersDropDownList(object selectedSubMeter = null)
        {
            var subMeters = _unitOfWork.SubMeterRepository.GetAllNoDefect().ToList();
            ViewBag.SubMeterID = new SelectList(subMeters, "ID", "Code", selectedSubMeter);
        }

        private void PopulateMeterTypesDropDownList(int subMeterId, int? meterTypeId)
        {
            ViewBag.MeterTypeID = GetMeterTypesBySubMeter(subMeterId, meterTypeId);
        }

        [HttpPost]
        public ActionResult GetMeterTypes(int subMeterId, int? meterTypeId)
        {
            return Json(GetMeterTypesBySubMeter(subMeterId, meterTypeId));
        }

        private List<SelectListItem> GetMeterTypesBySubMeter(int subMeterId, int? meterTypeId)
        {
            var list = new List<SelectListItem>();
            var subMeter = _unitOfWork.SubMeterRepository.GetSubMeterIncludingMeterTypes(subMeterId);
            if (subMeter != null)
            {
                foreach (var meterType in subMeter.MeterTypes)
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

        private decimal GetSubMeterConsumption(int subMeterId, int meterTypeId, DateTime? discountMonth, decimal index)
        {
            var consumption = 0.0m;
            var previousReading = _unitOfWork.SubMeterReadingRepository.GetPreviousSubMeterReading(subMeterId, meterTypeId, (DateTime)discountMonth);
            if (previousReading != null)
            {
                consumption = index - previousReading.Index;
            }
            else
            {
                var initialReading = _unitOfWork.SubMeterReadingRepository.GetInitialSubMeterReading(subMeterId, meterTypeId);
                if (initialReading != null)
                {
                    consumption = index - initialReading.Index;
                }
            }

            return consumption;
        }

        private bool CheckChildrenConsumption(SubMeterReading subMeterReading)
        {
            var consumption = GetSubMeterConsumption(subMeterReading.SubMeterID, subMeterReading.MeterTypeID, subMeterReading.DiscountMonth, subMeterReading.Index);
            List<int> childIds = _unitOfWork.SubSubMeterRepository.GetSubSubMeterIDsBySubMeterIDNoDefect(subMeterReading.SubMeterID).ToList();
            var childrenConsumption = 0.0m;
            foreach (var childId in childIds)
            {
                var actualChildReading = _unitOfWork.SubSubMeterReadingRepository.GetSubSubMeterReadingByDiscountMonth(childId, subMeterReading.MeterTypeID, (DateTime)subMeterReading.DiscountMonth);
                if (actualChildReading != null)
                {
                    var previousChildReading = _unitOfWork.SubSubMeterReadingRepository.GetPreviousSubSubMeterReading(childId, subMeterReading.MeterTypeID, (DateTime)subMeterReading.DiscountMonth);
                    if (previousChildReading != null)
                    {
                        childrenConsumption += actualChildReading.Index - previousChildReading.Index;
                    }
                    else
                    {
                        var initialChildReading = _unitOfWork.SubSubMeterReadingRepository.GetInitialSubSubMeterReading(childId, subMeterReading.MeterTypeID);
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

        private bool CheckParentConsumption(SubMeterReading subMeterReading)
        {
            var siblingsConsumption = GetSubMeterConsumption(subMeterReading.SubMeterID, subMeterReading.MeterTypeID, subMeterReading.DiscountMonth, subMeterReading.Index);
            var subMeter = _unitOfWork.SubMeterRepository.Get(subMeterReading.SubMeterID);
            List<int> siblingIds = _unitOfWork.SubMeterRepository.GetSubMeterIDsByMeterIDNoDefect(subMeter.MeterID).ToList();
            foreach (var siblingId in siblingIds)
            {
                var actualSiblingReading = _unitOfWork.SubMeterReadingRepository.GetSubMeterReadingByDiscountMonth(siblingId, subMeterReading.MeterTypeID, (DateTime)subMeterReading.DiscountMonth);
                if (actualSiblingReading != null)
                {
                    var previousSiblingReading = _unitOfWork.SubMeterReadingRepository.GetPreviousSubMeterReading(siblingId, subMeterReading.MeterTypeID, (DateTime)subMeterReading.DiscountMonth);
                    if (previousSiblingReading != null)
                    {
                        siblingsConsumption += actualSiblingReading.Index - previousSiblingReading.Index;
                    }
                    else
                    {
                        var initialSiblingReading = _unitOfWork.SubMeterReadingRepository.GetInitialSubMeterReading(siblingId, subMeterReading.MeterTypeID);
                        if (initialSiblingReading != null)
                        {
                            siblingsConsumption += actualSiblingReading.Index - initialSiblingReading.Index;
                        }
                    }
                }
            }

            var parentConsumption = 0.0m;
            var actualParentReading = _unitOfWork.MeterReadingRepository.GetMeterReadingByDiscountMonth(subMeter.MeterID, subMeterReading.MeterTypeID, (DateTime)subMeterReading.DiscountMonth);
            if (actualParentReading != null)
            {
                var previousParentReading = _unitOfWork.MeterReadingRepository.GetPreviousMeterReading(subMeter.MeterID, subMeterReading.MeterTypeID, (DateTime) subMeterReading.DiscountMonth);
                if (previousParentReading != null)
                {
                    parentConsumption += actualParentReading.Index - previousParentReading.Index;
                }
                else
                {
                    var initialParentReading = _unitOfWork.MeterReadingRepository.GetInitialMeterReading(subMeter.MeterID, subMeterReading.MeterTypeID);
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