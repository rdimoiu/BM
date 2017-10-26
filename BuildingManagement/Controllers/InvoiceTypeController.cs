using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;

namespace BuildingManagement.Controllers
{
    public class InvoiceTypeController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        // GET: InvoiceType
        public ActionResult Index()
        {
            var invoiceTypes = _unitOfWork.InvoiceTypeRepository.Get();
            return View(invoiceTypes);
        }

        // GET: InvoiceType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var invoiceType = _unitOfWork.InvoiceTypeRepository.GetById(id);
            if (invoiceType == null)
            {
                return HttpNotFound();
            }
            return View(invoiceType);
        }

        // GET: InvoiceType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InvoiceType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Type")] InvoiceType invoiceType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //uniqueness condition check
                    var duplicateInvoiceType =
                        _unitOfWork.InvoiceTypeRepository.Get(filter: it => it.Type == invoiceType.Type).FirstOrDefault();
                    if (duplicateInvoiceType != null)
                    {
                        ModelState.AddModelError("Type", "An invoice type with this type already exists.");
                        return View(invoiceType);
                    }
                    _unitOfWork.InvoiceTypeRepository.Insert(invoiceType);
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(invoiceType);
        }

        // GET: InvoiceType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var invoiceType = _unitOfWork.InvoiceTypeRepository.GetById(id);
            if (invoiceType == null)
            {
                return HttpNotFound();
            }
            return View(invoiceType);
        }

        // POST: InvoiceType/Edit/5
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
            var invoiceTypeToUpdate = _unitOfWork.InvoiceTypeRepository.GetById(id);
            if (invoiceTypeToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(invoiceTypeToUpdate, "", new[] { "Type" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateInvoiceType =
                        _unitOfWork.InvoiceTypeRepository.Get(filter: it => it.Type == invoiceTypeToUpdate.Type).FirstOrDefault();
                    if (duplicateInvoiceType != null && duplicateInvoiceType.ID != invoiceTypeToUpdate.ID)
                    {
                        ModelState.AddModelError("Type", "An invoice type with this type already exists.");
                        return View(invoiceTypeToUpdate);
                    }
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(invoiceTypeToUpdate);
        }

        // GET: InvoiceType/Delete/5
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
            var invoiceType = _unitOfWork.InvoiceTypeRepository.GetById(id);
            if (invoiceType == null)
            {
                return HttpNotFound();
            }
            return View(invoiceType);
        }

        // POST: InvoiceType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _unitOfWork.InvoiceTypeRepository.Delete(id);
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
