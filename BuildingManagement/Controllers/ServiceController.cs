using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using X.PagedList;

namespace BuildingManagement.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Service
        public ActionResult Index(int? page, string currentFilter, string searchString, string sortOrder)
        {
            IEnumerable<Service> services;
            var pageNumber = page ?? 1;
            const int pageSize = 3;
            if (searchString != null)
            {
                pageNumber = 1;
                services = _unitOfWork.ServiceRepository.GetFilteredServicesIncludingInvoiceAndDistributionModeAndSectionsAndLevelsAndSpaces(searchString);
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    services = _unitOfWork.ServiceRepository.GetFilteredServicesIncludingInvoiceAndDistributionModeAndSectionsAndLevelsAndSpaces(searchString);
                }
                else
                {
                    services = _unitOfWork.ServiceRepository.GetAllServicesIncludingInvoiceAndDistributionModeAndSectionsAndLevelsAndSpaces();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.InvoiceSortParm = string.IsNullOrEmpty(sortOrder) ? "invoice_desc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.QuantitySortParm = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            ViewBag.UnitSortParm = sortOrder == "Unit" ? "unit_desc" : "Unit";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";
            ViewBag.ValueWithoutTVASortParm = sortOrder == "ValueWithoutTVA" ? "valueWithoutTVA_desc" : "ValueWithoutTVA";
            ViewBag.TVASortParm = sortOrder == "TVA" ? "tva_desc" : "TVA";
            ViewBag.QuotaTVASortParm = sortOrder == "QuotaTVA" ? "quotaTVA_desc" : "QuotaTVA";
            ViewBag.DistributionModeSortParm = sortOrder == "DistributionMode" ? "distributionMode_desc" : "DistributionMode";
            ViewBag.FixedSortParm = sortOrder == "Fixed" ? "fixed_desc" : "Fixed";
            ViewBag.CountedSortParm = sortOrder == "Counted" ? "counted_desc" : "Counted";
            ViewBag.InhabitedSortParm = sortOrder == "Inhabited" ? "inhabited_desc" : "Inhabited";
            services = _unitOfWork.ServiceRepository.OrderServices(services, sortOrder);
            ViewBag.OnePageOfServices = services.ToPagedList(pageNumber, pageSize);
            return View(ViewBag.OnePageOfServices);
        }

        // GET: Service/Details/5
        public ActionResult Details(int id)
        {
            var service = _unitOfWork.ServiceRepository.Get(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View(service);
        }

        // GET: Service/Create
        public ActionResult Create(int? invoiceId)
        {
            var service = new Service();
            if (invoiceId != null)
            {
                service.InvoiceID = (int) invoiceId;
            }
            if (Request.UrlReferrer != null)
            {
                service.PreviousPage = Request.UrlReferrer.AbsolutePath;
            }
            PopulateInvoicesDropDownList();
            PopulateDistributionModesDropDownList();
            return View(service);
        }

        // POST: Service/Create
        [HttpPost]
        public ActionResult Create(Service service)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                var duplicateService = _unitOfWork.ServiceRepository.SingleOrDefault(s => s.Name == service.Name && s.InvoiceID == service.InvoiceID);
                if (duplicateService != null)
                {
                    PopulateInvoicesDropDownList(service.InvoiceID);
                    PopulateDistributionModesDropDownList(service.DistributionModeID);
                    return new HttpStatusCodeResult(409, "A service with this name, for this invoice, already exists.");
                }
                var invoice = _unitOfWork.InvoiceRepository.Get(service.InvoiceID);
                if (invoice != null)
                {
                    invoice.Quantity = invoice.Quantity + service.Quantity;
                    invoice.TotalValueWithoutTVA = invoice.TotalValueWithoutTVA + service.Quantity * service.Price;
                    invoice.TotalTVA = invoice.TotalTVA + service.ValueWithoutTVA * service.QuotaTVA;
                    service.Invoice = invoice;
                }
                var distributionMode = _unitOfWork.DistributionModeRepository.SingleOrDefault(d => d.ID == service.DistributionModeID);
                if (distributionMode != null)
                {
                    service.DistributionMode = distributionMode;
                }
                if (service.ServiceSLSSelected != null)
                {
                    #region Add Sections, Levels and Spaces

                    var tree3D = Utils.TreeHelper.MapSelectedIDsToDatabaseIDsForSpaces(service.ServiceSLSSelected);
                    service.Sections = new List<Section>();
                    foreach (var sectionId in tree3D.SectionIDs)
                    {
                        var section = _unitOfWork.SectionRepository.Get(sectionId);
                        if (section != null)
                        {
                            service.Sections.Add(section);
                        }
                    }
                    service.Levels = new List<Level>();
                    foreach (var levelId in tree3D.LevelIDs)
                    {
                        var level = _unitOfWork.LevelRepository.Get(levelId);
                        if (level != null)
                        {
                            service.Levels.Add(level);
                        }
                    }
                    service.Spaces = new List<Space>();
                    foreach (var spaceId in tree3D.SpaceIDs)
                    {
                        var space = _unitOfWork.SpaceRepository.Get(spaceId);
                        if (space != null)
                        {
                            service.Spaces.Add(space);
                        }
                    }

                    #endregion
                }
                try
                {
                    _unitOfWork.ServiceRepository.Add(service);
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Service {0} has been created.", service.Name);
                    return Json(service.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateInvoicesDropDownList(service.InvoiceID);
            PopulateDistributionModesDropDownList(service.DistributionModeID);
            return View(service);
        }

        // GET: Service/Edit/5
        public ActionResult Edit(int id)
        {
            var service = _unitOfWork.ServiceRepository.Get(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            if (Request.UrlReferrer != null)
            {
                service.PreviousPage = Request.UrlReferrer.AbsolutePath;
            }
            PopulateInvoicesDropDownList(service.InvoiceID);
            PopulateDistributionModesDropDownList(service.DistributionModeID);
            return View(service);
        }

        // POST: Service/Edit/5
        [HttpPost]
        public ActionResult Edit(Service service)
        {
            var serviceToUpdate = _unitOfWork.ServiceRepository.GetServiceIncludingInvoiceAndDistributionModeAndSectionsAndLevelsAndSpaces(service.ID);
            if (serviceToUpdate == null)
            {
                return HttpNotFound();
            }
            var oldInvoice = _unitOfWork.InvoiceRepository.Get(serviceToUpdate.InvoiceID);
            if (oldInvoice == null)
            {
                return HttpNotFound();
            }
            oldInvoice.Quantity = oldInvoice.Quantity - serviceToUpdate.Quantity;
            oldInvoice.TotalValueWithoutTVA = oldInvoice.TotalValueWithoutTVA - serviceToUpdate.ValueWithoutTVA;
            oldInvoice.TotalTVA = oldInvoice.TotalTVA - serviceToUpdate.TVA;
            if (TryUpdateModel(serviceToUpdate, "", new[]{"Name", "Quantity", "Unit", "Price", "TVA", "ValueWithoutTVA", "QuotaTVA", "Fixed", "Inhabited", "InvoiceID", "DistributionModeID", "Counted"}))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateService = _unitOfWork.ServiceRepository.SingleOrDefault(s => s.Name == service.Name && s.InvoiceID == service.InvoiceID);
                    if (duplicateService != null && duplicateService.ID != service.ID)
                    {
                        PopulateInvoicesDropDownList(service.InvoiceID);
                        PopulateDistributionModesDropDownList(service.DistributionModeID);
                        return new HttpStatusCodeResult(409, "A service with this name, for this invoice, already exists.");
                    }
                    if (service.ServiceSLSSelected != null)
                    {
                        #region Update Sections, Levels and Spaces

                        var tree3D = Utils.TreeHelper.MapSelectedIDsToDatabaseIDsForSpaces(service.ServiceSLSSelected);
                        serviceToUpdate.Sections.Clear();
                        foreach (var sectionId in tree3D.SectionIDs)
                        {
                            var section = _unitOfWork.SectionRepository.Get(sectionId);
                            if (section != null)
                            {
                                serviceToUpdate.Sections.Add(section);
                            }
                        }
                        serviceToUpdate.Levels.Clear();
                        foreach (var levelId in tree3D.LevelIDs)
                        {
                            var level = _unitOfWork.LevelRepository.Get(levelId);
                            if (level != null)
                            {
                                serviceToUpdate.Levels.Add(level);
                            }
                        }
                        serviceToUpdate.Spaces.Clear();
                        foreach (var spaceId in tree3D.SpaceIDs)
                        {
                            var space = _unitOfWork.SpaceRepository.Get(spaceId);
                            if (space != null)
                            {
                                serviceToUpdate.Spaces.Add(space);
                            }
                        }

                        #endregion
                    }
                    var invoice = _unitOfWork.InvoiceRepository.Get(serviceToUpdate.InvoiceID);
                    if (invoice == null)
                    {
                        return HttpNotFound();
                    }
                    invoice.Quantity = invoice.Quantity + service.Quantity;
                    invoice.TotalValueWithoutTVA = invoice.TotalValueWithoutTVA + service.Quantity * service.Price;
                    invoice.TotalTVA = invoice.TotalTVA + service.ValueWithoutTVA * service.QuotaTVA;
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("Service {0} has been edited.", serviceToUpdate.Name);
                    return Json(serviceToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateInvoicesDropDownList(service.InvoiceID);
            PopulateDistributionModesDropDownList(service.DistributionModeID);
            return View(serviceToUpdate);
        }

        // GET: Service/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var service = _unitOfWork.ServiceRepository.GetServiceIncludingInvoiceAndDistributionModeAndSectionsAndLevelsAndSpaces(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            if (Request.UrlReferrer != null)
            {
                service.PreviousPage = Request.UrlReferrer.AbsolutePath;
            }
            return View(service);
        }

        // POST: Service/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string PreviousPage)
        {
            try
            {
                var service = _unitOfWork.ServiceRepository.Get(id);
                if (service == null)
                {
                    return HttpNotFound();
                }
                var invoice = _unitOfWork.InvoiceRepository.Get(service.InvoiceID);
                if (invoice == null)
                {
                    return HttpNotFound();
                }
                invoice.Quantity = invoice.Quantity - service.Quantity;
                invoice.TotalValueWithoutTVA = invoice.TotalValueWithoutTVA - service.ValueWithoutTVA;
                invoice.TotalTVA = invoice.TotalTVA - service.TVA;
                _unitOfWork.ServiceRepository.Remove(service);
                _unitOfWork.Save();
                TempData["message"] = string.Format("Service {0} has been deleted.", service.Name);
                if (PreviousPage.Equals("/Service/Index"))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index", "InvoiceDistribution");
                }
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

        private void PopulateInvoicesDropDownList(object selectedInvoice = null)
        {
            var invoicesQuery = _unitOfWork.InvoiceRepository.GetAll().Where(i => i.Closed == false);
            ViewBag.InvoiceID = new SelectList(invoicesQuery, "ID", "Number", selectedInvoice);
        }

        private void PopulateDistributionModesDropDownList(object selectedDistributionMode = null)
        {
            var distributionModesQuery = _unitOfWork.DistributionModeRepository.GetAll();
            ViewBag.DistributionModeID = new SelectList(distributionModesQuery, "ID", "Mode", selectedDistributionMode);
        }

        [HttpGet]
        public string GetSpacesTreeData(int? serviceId, int? invoiceId)
        {
            var root = new TreeNode
            {
                id = "root",
                children = { },
                text = "-",
                state = new TreeNodeState { opened = true }
            };

            var selectedSpacesIDs = new HashSet<int>();
            var selectedLevelsIDs = new HashSet<int>();
            var selectedSectionsIDs = new HashSet<int>();
            if (serviceId != null && serviceId != 0)
            {
                var service = _unitOfWork.ServiceRepository.GetServiceIncludingSectionsAndLevelsAndSpaces((int)serviceId);
                if (service != null)
                {
                    selectedSpacesIDs = new HashSet<int>(service.Spaces.Select(s => s.ID));
                    selectedLevelsIDs = new HashSet<int>(service.Levels.Select(l => l.ID));
                    selectedSectionsIDs = new HashSet<int>(service.Sections.Select(s => s.ID));
                }
            }

            if (invoiceId != null && invoiceId != 0)
            {
                var invoice = _unitOfWork.InvoiceRepository.Get((int)invoiceId);
                if (invoice != null)
                {
                    var treeHelper = new Utils.TreeHelper(_unitOfWork);
                    root = treeHelper.GetSectionsLevelsSpacesByClient(root, invoice.ClientID, selectedSectionsIDs, selectedLevelsIDs, selectedSpacesIDs);
                }
            }

            return new JavaScriptSerializer().Serialize(root);
        }
    }
}
