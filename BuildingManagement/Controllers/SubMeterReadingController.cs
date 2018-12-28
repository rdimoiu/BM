using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
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
                subMeterReadings = _unitOfWork.SubMeterReadingRepository.GetFilteredSubMeterReadingsIncludingSubMeterAndMeterType(searchString);
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    subMeterReadings = _unitOfWork.SubMeterReadingRepository.GetFilteredSubMeterReadingsIncludingSubMeterAndMeterType(searchString);
                }
                else
                {
                    subMeterReadings = _unitOfWork.SubMeterReadingRepository.GetAllSubMeterReadingsIncludingSubMeterAndMeterType();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IndexSortParm = string.IsNullOrEmpty(sortOrder) ? "index_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.SubMeterSortParm = sortOrder == "SubMeter" ? "subMeter_desc" : "SubMeter";
            ViewBag.MeterTypeSortParm = sortOrder == "MeterType" ? "meterType_desc" : "MeterType";
            subMeterReadings = _unitOfWork.SubMeterReadingRepository.OrderSubMeterReadings(subMeterReadings, sortOrder);
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
                var duplicateSubMeterReading = _unitOfWork.SubMeterReadingRepository.SingleOrDefault(smr => smr.Date >= subMeterReading.Date && smr.SubMeterID == subMeterReading.SubMeterID && smr.MeterTypeID == subMeterReading.MeterTypeID);
                if (duplicateSubMeterReading != null)
                {
                    PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                    PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                    return new HttpStatusCodeResult(409, "A submeter reading on the same or later date already exists.");
                }
                var greaterSubMeterReadings = _unitOfWork.SubMeterReadingRepository.SingleOrDefault(smr => smr.SubMeterID == subMeterReading.SubMeterID && smr.MeterTypeID == subMeterReading.MeterTypeID && smr.Index > subMeterReading.Index);
                if (greaterSubMeterReadings != null)
                {
                    PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                    PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                    return new HttpStatusCodeResult(409, "A submeter reading with the same or greater index already exists.");
                }
                var subMeter = _unitOfWork.SubMeterRepository.Get(subMeterReading.SubMeterID);
                if (subMeterReading.Index > subMeter.InitialIndex)
                {
                    PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                    PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                    return new HttpStatusCodeResult(409, "The submeter intial index is the same or greater than the reading.");
                }
                try
                {
                    _unitOfWork.SubMeterReadingRepository.Add(subMeterReading);
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("SubMeterReading {0} has been created.", subMeterReading.Index);
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
            if (TryUpdateModel(subMeterReadingToUpdate, "", new[] { "Index", "Date", "SubMeterID", "MeterTypeID" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateSubMeterReading = _unitOfWork.SubMeterReadingRepository.SingleOrDefault(smr => smr.Date == subMeterReadingToUpdate.Date && smr.SubMeterID == subMeterReadingToUpdate.SubMeterID && smr.MeterTypeID == subMeterReadingToUpdate.MeterTypeID);
                    if (duplicateSubMeterReading != null && duplicateSubMeterReading.ID != subMeterReadingToUpdate.ID)
                    {
                        PopulateSubMetersDropDownList(subMeterReadingToUpdate.SubMeterID);
                        PopulateMeterTypesDropDownList(subMeterReadingToUpdate.SubMeterID, subMeterReadingToUpdate.MeterTypeID);
                        return new HttpStatusCodeResult(409, "A submeter reading on the same or later date already exists.");
                    }
                    var greaterSubMeterReading = _unitOfWork.SubMeterReadingRepository.SingleOrDefault(smr => smr.SubMeterID == subMeterReadingToUpdate.SubMeterID && smr.MeterTypeID == subMeterReadingToUpdate.MeterTypeID && smr.Index > subMeterReadingToUpdate.Index);
                    if (greaterSubMeterReading != null && greaterSubMeterReading.ID != subMeterReadingToUpdate.ID)
                    {
                        PopulateSubMetersDropDownList(subMeterReadingToUpdate.SubMeterID);
                        PopulateMeterTypesDropDownList(subMeterReadingToUpdate.SubMeterID, subMeterReadingToUpdate.MeterTypeID);
                        return new HttpStatusCodeResult(409, "A submeter reading with the same or greater index already exists.");
                    }
                    var subMeter = _unitOfWork.SubMeterRepository.Get(subMeterReading.SubMeterID);
                    if (subMeterReading.Index > subMeter.InitialIndex)
                    {
                        PopulateSubMetersDropDownList(subMeterReading.SubMeterID);
                        PopulateMeterTypesDropDownList(subMeterReading.SubMeterID, subMeterReading.MeterTypeID);
                        return new HttpStatusCodeResult(409, "The submeter intial index is the same or greater than the reading.");
                    }
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("SubMeterReading {0} has been edited.", subMeterReadingToUpdate.Index);
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
                TempData["message"] = string.Format("SubMeterReading {0} has been deleted.", subMeterReading.Index);
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
            var subMeters = _unitOfWork.SubMeterRepository.GetAll();
            ViewBag.SubMeterID = new SelectList(subMeters, "ID", "Code", selectedSubMeter);
        }

        private void PopulateMeterTypesDropDownList(int subMeterId, int meterTypeId)
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
    }
}