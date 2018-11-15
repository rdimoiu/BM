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
                        _unitOfWork.InvoiceTypeRepository.SingleOrDefault(it => it.Type == invoiceType.Type);
                    if (duplicateInvoiceType != null)
                    {
                        ModelState.AddModelError("Type", "An invoice type with this type already exists.");
                        return View(invoiceType);
                    }
                    _unitOfWork.InvoiceTypeRepository.Add(invoiceType);
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Invoice type {0} has been created.", invoiceType.Type);
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int id)
        {
            var invoiceType = _unitOfWork.InvoiceTypeRepository.Get(id);
            if (invoiceType == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(invoiceType, "", new[] {"Type"}))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateInvoiceType = _unitOfWork.InvoiceTypeRepository.SingleOrDefault(it => it.Type == invoiceType.Type);
                    if (duplicateInvoiceType != null && duplicateInvoiceType.ID != invoiceType.ID)
                    {
                        ModelState.AddModelError("Type", "An invoice type with this type already exists.");
                        return View(invoiceType);
                    }
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Invoice type {0} has been edited.", invoiceType.Type);
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(invoiceType);
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
                TempData["message"] = string.Format("Invoice type {0} has been deleted.", invoiceType.Type);
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
