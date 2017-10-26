using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;

namespace BuildingManagement.Controllers
{
    public class SpaceTypeController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        // GET: SpaceType
        public ActionResult Index()
        {
            var spaceTypes = _unitOfWork.SpaceTypeRepository.Get();
            return View(spaceTypes);
        }

        // GET: SpaceType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var spaceType = _unitOfWork.SpaceTypeRepository.GetById(id);
            if (spaceType == null)
            {
                return HttpNotFound();
            }
            return View(spaceType);
        }

        // GET: SpaceType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SpaceType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Type")] SpaceType spaceType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //uniqueness condition check
                    var duplicateSpaceType =
                        _unitOfWork.SpaceTypeRepository.Get(filter: st => st.Type == spaceType.Type).FirstOrDefault();
                    if (duplicateSpaceType != null)
                    {
                        ModelState.AddModelError("Type", "A space type with this type already exists.");
                        return View(spaceType);
                    }
                    _unitOfWork.SpaceTypeRepository.Insert(spaceType);
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(spaceType);
        }

        // GET: SpaceType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var spaceType = _unitOfWork.SpaceTypeRepository.GetById(id);
            if (spaceType == null)
            {
                return HttpNotFound();
            }
            return View(spaceType);
        }

        // POST: SpaceType/Edit/5
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
            var spaceTypeToUpdate = _unitOfWork.SpaceTypeRepository.GetById(id);
            if (spaceTypeToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(spaceTypeToUpdate, "", new[] { "Type" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateSpaceType =
                        _unitOfWork.SpaceTypeRepository.Get(filter: st => st.Type == spaceTypeToUpdate.Type).FirstOrDefault();
                    if (duplicateSpaceType != null && duplicateSpaceType.ID != spaceTypeToUpdate.ID)
                    {
                        ModelState.AddModelError("Type", "A space type with this type already exists.");
                        return View(spaceTypeToUpdate);
                    }
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(spaceTypeToUpdate);
        }

        // GET: SpaceType/Delete/5
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
            var spaceType = _unitOfWork.SpaceTypeRepository.GetById(id);
            if (spaceType == null)
            {
                return HttpNotFound();
            }
            return View(spaceType);
        }

        // POST: SpaceType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _unitOfWork.SpaceTypeRepository.Delete(id);
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
