using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;

namespace BuildingManagement.Controllers
{
    public class MeterReadingController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        // GET: MeterReading
        public ActionResult Index()
        {
            var meterReadings = _unitOfWork.MeterReadingRepository.Get(includeProperties: "Meter, MeterType");
            return View(meterReadings);
        }

        // GET: MeterReading/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var meterReading = _unitOfWork.MeterReadingRepository.GetById(id);
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
                    var duplicateMeterReading =
                        _unitOfWork.MeterReadingRepository.Get(filter: mr => mr.Date >= meterReading.Date && mr.MeterID == meterReading.MeterID && mr.MeterTypeID == meterReading.MeterTypeID).FirstOrDefault();
                    if (duplicateMeterReading != null)
                    {
                        ModelState.AddModelError("Date", "A meter reading on the same or later date already exists.");
                        PopulateMetersDropDownList(meterReading.MeterID);
                        PopulateMeterTypesDropDownList(meterReading.MeterID, meterReading.MeterTypeID);
                        ViewBag.RefreshMeterType = true;
                        return View(meterReading);
                    }
                    var greaterMeterReadings = _unitOfWork.MeterReadingRepository.Get(filter: mr => mr.MeterID == meterReading.MeterID && mr.MeterTypeID == meterReading.MeterTypeID && mr.Index > meterReading.Index);
                    if (greaterMeterReadings.Any())
                    {
                        ModelState.AddModelError("Index", "A meter reading with the same or greater index already exists.");
                        PopulateMetersDropDownList(meterReading.MeterID);
                        return View(meterReading);
                    }
                    var olderReadings =
                        _unitOfWork.MeterReadingRepository.Get(filter: mr => mr.MeterID == meterReading.MeterID && mr.MeterTypeID == meterReading.MeterTypeID);
                    if (olderReadings.Any())
                    {
                        if (olderReadings.Count() == 1)
                        {
                            //var 
                        }
                        olderReadings = olderReadings.OrderByDescending(mr => mr.Date).Take(2);
                        
                    }
                    _unitOfWork.MeterReadingRepository.Insert(meterReading);
                    _unitOfWork.Save();
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
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var meterReading = _unitOfWork.MeterReadingRepository.GetById(id);
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
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var meterReadingToUpdate = _unitOfWork.MeterReadingRepository.GetById(id);
            if (meterReadingToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(meterReadingToUpdate, "", new[] { "Index", "Date", "MeterID", "MeterType" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateMeterReading =
                        _unitOfWork.MeterReadingRepository.Get(filter: mr => mr.Date == meterReadingToUpdate.Date && mr.MeterID == meterReadingToUpdate.MeterID && mr.MeterTypeID == meterReadingToUpdate.MeterTypeID).FirstOrDefault();
                    if (duplicateMeterReading != null && duplicateMeterReading.ID != meterReadingToUpdate.ID)
                    {
                        ModelState.AddModelError("Date", "A meter reading on this date already exists.");
                        PopulateMetersDropDownList(meterReadingToUpdate.MeterID);
                        return View(meterReadingToUpdate);
                    }
                    var greaterMeterReadings = _unitOfWork.MeterReadingRepository.Get(filter: mr => mr.MeterID == meterReadingToUpdate.MeterID && mr.MeterTypeID == meterReadingToUpdate.MeterTypeID && mr.Index > meterReadingToUpdate.Index);
                    if (greaterMeterReadings.Any())
                    {
                        ModelState.AddModelError("Index", "A meter reading with greater index already exists.");
                        PopulateMetersDropDownList(meterReadingToUpdate.MeterID);
                        return View(meterReadingToUpdate);
                    }
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateMetersDropDownList(meterReadingToUpdate.MeterID);
            return View(meterReadingToUpdate);
        }

        // GET: MeterReading/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var meterReading = _unitOfWork.MeterReadingRepository.Get(includeProperties: "Meter, MeterType").Single(mr => mr.ID == id); ;
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
                _unitOfWork.MeterReadingRepository.Delete(id);
                _unitOfWork.Save();
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
            var metersQuery = from m in _unitOfWork.MeterRepository.Get(includeProperties: "MeterTypes") select m;
            ViewBag.MeterID = new SelectList(metersQuery, "ID", "Code", selectedMeter);
        }

        private void PopulateMeterTypesDropDownList(int? meterId, int? meterTypeId)
        {
            var list = new List<SelectListItem>();
            var meter = _unitOfWork.MeterRepository.Get(includeProperties: "MeterTypes").FirstOrDefault(m => m.ID == meterId);
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
            ViewBag.MeterTypeID = list;
        }

        [HttpPost]
        public ActionResult GetMeterTypesByMeter(int meterId, int? meterTypeId)
        {
            var list = new List<SelectListItem>();
            var meter = _unitOfWork.MeterRepository.Get(includeProperties: "MeterTypes").FirstOrDefault(m => m.ID == meterId);
            if (meter != null)
            {
                foreach (var meterType in meter.MeterTypes)
                {
                    if (meterTypeId != null && meterTypeId == meterType.ID)
                    {
                        list.Add(new SelectListItem { Value = meterType.ID.ToString(), Text = meterType.Type, Selected = true});
                    }
                    else
                    {
                        list.Add(new SelectListItem { Value = meterType.ID.ToString(), Text = meterType.Type });
                    }
                }
            }
            return Json(list);
        }
    }
}
