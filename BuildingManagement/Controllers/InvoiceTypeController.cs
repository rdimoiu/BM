using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using X.PagedList;

namespace BuildingManagement.Controllers
{
    public class InvoiceTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: InvoiceType
        public ActionResult Index(int? page, string currentFilter, string searchString, string sortOrder)
        {
            IEnumerable<InvoiceType> invoiceTypes;
            var pageNumber = page ?? 1;
            const int pageSize = 3;
            if (searchString != null)
            {
                pageNumber = 1;
                invoiceTypes = _unitOfWork.InvoiceTypeRepository.GetFilteredInvoiceTypes(searchString);
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    invoiceTypes = _unitOfWork.InvoiceTypeRepository.GetFilteredInvoiceTypes(searchString);
                }
                else
                {
                    invoiceTypes = _unitOfWork.InvoiceTypeRepository.GetAll();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.TypeSortParm = string.IsNullOrEmpty(sortOrder) ? "type_desc" : "";
            invoiceTypes = _unitOfWork.InvoiceTypeRepository.OrderInvoiceTypes(invoiceTypes, sortOrder);
            ViewBag.OnePageOfInvoiceTypes = invoiceTypes.ToPagedList(pageNumber, pageSize);
            return View(ViewBag.OnePageOfInvoiceTypes);
        }

        // GET: InvoiceType/Details/5
        public ActionResult Details(int id)
        {
            var invoiceType = _unitOfWork.InvoiceTypeRepository.Get(id);
            if (invoiceType == null)
            {
                return HttpNotFound();
            }
            return View(invoiceType);
        }

        // GET: InvoiceType/Create
        public ActionResult Create()
        {
            var invoiceType = new InvoiceType();
            return View(invoiceType);
        }

        // POST: InvoiceType/Create
        [HttpPost]
        public ActionResult Create(InvoiceType invoiceType)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                var duplicateInvoiceType = _unitOfWork.InvoiceTypeRepository.FirstOrDefault(it => it.Type == invoiceType.Type);
                if (duplicateInvoiceType != null)
                {
                    return new HttpStatusCodeResult(409, "An invoice type with this type already exists.");
                }
                try
                {
                    _unitOfWork.InvoiceTypeRepository.Add(invoiceType);
                    _unitOfWork.Save();
                    TempData["message"] = $"Invoice type {invoiceType.Type} has been created.";
                    return Json(invoiceType.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(invoiceType);
        }

        // GET: InvoiceType/Edit/5
        public ActionResult Edit(int id)
        {
            var invoiceType = _unitOfWork.InvoiceTypeRepository.Get(id);
            if (invoiceType == null)
            {
                return HttpNotFound();
            }
            return View(invoiceType);
        }

        // POST: InvoiceType/Edit/5
        [HttpPost]
        public ActionResult Edit(InvoiceType invoiceType)
        {
            var invoiceTypeToUpdate = _unitOfWork.InvoiceTypeRepository.Get(invoiceType.ID);
            if (invoiceTypeToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(invoiceTypeToUpdate, "", new[] {"Type"}))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateInvoiceType = _unitOfWork.InvoiceTypeRepository.FirstOrDefault(it => it.Type == invoiceTypeToUpdate.Type);
                    if (duplicateInvoiceType != null && duplicateInvoiceType.ID != invoiceTypeToUpdate.ID)
                    {
                        return new HttpStatusCodeResult(409, "An invoice type with this type already exists.");
                    }
                    _unitOfWork.Save();
                    TempData["message"] = $"Invoice type {invoiceTypeToUpdate.Type} has been edited.";
                    return Json(invoiceTypeToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(invoiceTypeToUpdate);
        }

        // GET: InvoiceType/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var invoiceType = _unitOfWork.InvoiceTypeRepository.Get(id);
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
                var invoiceType = _unitOfWork.InvoiceTypeRepository.Get(id);
                if (invoiceType == null)
                {
                    return HttpNotFound();
                }
                _unitOfWork.InvoiceTypeRepository.Remove(invoiceType);
                _unitOfWork.Save();
                TempData["message"] = $"Invoice type {invoiceType.Type} has been deleted.";
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new {id, saveChangesError = true});
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
