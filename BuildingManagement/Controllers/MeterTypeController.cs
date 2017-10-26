using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;

namespace BuildingManagement.Controllers
{
    public class MeterTypeController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        // GET: MeterType
        public ActionResult Index()
        {
            var meterTypes = _unitOfWork.MeterTypeRepository.Get();
            return View(meterTypes);
        }

        // GET: MeterType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var meterType = _unitOfWork.MeterTypeRepository.GetById(id);
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
                    var duplicateMeterType =
                        _unitOfWork.MeterTypeRepository.Get(filter: mt => mt.Type == meterType.Type).FirstOrDefault();
                    if (duplicateMeterType != null)
                    {
                        ModelState.AddModelError("Type", "A meter type with this type already exists.");
                        return View(duplicateMeterType);
                    }
                    _unitOfWork.MeterTypeRepository.Insert(meterType);
                    _unitOfWork.Save();
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
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var meterType = _unitOfWork.MeterTypeRepository.GetById(id);
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
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var meterTypeToUpdate = _unitOfWork.MeterTypeRepository.GetById(id);
            if (meterTypeToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(meterTypeToUpdate, "", new[] {"Type"}))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateMeterType =
                        _unitOfWork.MeterTypeRepository.Get(filter: mt => mt.Type == meterTypeToUpdate.Type).FirstOrDefault();
                    if (duplicateMeterType != null && duplicateMeterType.ID != meterTypeToUpdate.ID)
                    {
                        ModelState.AddModelError("Type", "A meter type with this type already exists.");
                        return View(duplicateMeterType);
                    }
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(meterTypeToUpdate);
        }

        // GET: MeterType/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError=false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var meterType = _unitOfWork.MeterTypeRepository.Get(includeProperties: "Meters").Single(mt => mt.ID == id); ;
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
                _unitOfWork.MeterTypeRepository.Delete(id);
                _unitOfWork.Save();
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
