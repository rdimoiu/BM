using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;

namespace BuildingManagement.Controllers
{
    public class DistributionModeController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        
        // GET: DistributionMode
        public ActionResult Index()
        {
            var distributionModes = _unitOfWork.DistributionModeRepository.Get();
            return View(distributionModes);
        }

        // GET: DistributionMode/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var distributionMode = _unitOfWork.DistributionModeRepository.GetById(id);
            if (distributionMode == null)
            {
                return HttpNotFound();
            }
            return View(distributionMode);
        }

        // GET: DistributionMode/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DistributionMode/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Mode")] DistributionMode distributionMode)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //uniqueness condition check
                    var duplicateDistributionMode =
                        _unitOfWork.DistributionModeRepository.Get(filter: dm => dm.Mode == distributionMode.Mode).FirstOrDefault();
                    if (duplicateDistributionMode != null)
                    {
                        ModelState.AddModelError("Mode", "A distribution mode with this mode already exists.");
                        return View(distributionMode);
                    }
                    _unitOfWork.DistributionModeRepository.Insert(distributionMode);
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(distributionMode);
        }

        // GET: DistributionMode/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var distributionMode = _unitOfWork.DistributionModeRepository.GetById(id);
            if (distributionMode == null)
            {
                return HttpNotFound();
            }
            return View(distributionMode);
        }

        // POST: DistributionMode/Edit/5
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
            var distributionModeToUpdate = _unitOfWork.DistributionModeRepository.GetById(id);
            if (distributionModeToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(distributionModeToUpdate, "", new[] { "Mode" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateDistributionMode = _unitOfWork.DistributionModeRepository.Get(filter: dm => dm.Mode == distributionModeToUpdate.Mode).FirstOrDefault();
                    if (duplicateDistributionMode != null && duplicateDistributionMode.ID != distributionModeToUpdate.ID)
                    {
                        ModelState.AddModelError("Mode", "A distribution mode with this mode already exists.");
                        return View(distributionModeToUpdate);
                    }
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(distributionModeToUpdate);
        }

        // GET: DistributionMode/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError= false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var distributionMode = _unitOfWork.DistributionModeRepository.GetById(id);
            if (distributionMode == null)
            {
                return HttpNotFound();
            }
            return View(distributionMode);
        }

        // POST: DistributionMode/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _unitOfWork.DistributionModeRepository.Delete(id);
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
    }
}
