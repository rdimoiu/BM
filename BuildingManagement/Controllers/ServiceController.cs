using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using BuildingManagement.ViewModels;
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
                services = _unitOfWork.ServiceRepository.GetFilteredServicesIncludingInvoiceAndDistributionModeAndMeterTypeAndSectionsAndLevelsAndSpaces(searchString).ToList();
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    services = _unitOfWork.ServiceRepository.GetFilteredServicesIncludingInvoiceAndDistributionModeAndMeterTypeAndSectionsAndLevelsAndSpaces(searchString).ToList();
                }
                else
                {
                    services = _unitOfWork.ServiceRepository.GetAllServicesIncludingInvoiceAndDistributionModeAndMeterTypeAndSectionsAndLevelsAndSpaces().ToList();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.InvoiceSortParm = string.IsNullOrEmpty(sortOrder) ? "invoice_desc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.QuantitySortParm = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            ViewBag.UnitSortParm = sortOrder == "Unit" ? "unit_desc" : "Unit";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";
            ViewBag.QuotaTVASortParm = sortOrder == "QuotaTVA" ? "quotaTVA_desc" : "QuotaTVA";
            ViewBag.FixedSortParm = sortOrder == "Fixed" ? "fixed_desc" : "Fixed";
            ViewBag.CountedSortParm = sortOrder == "Counted" ? "counted_desc" : "Counted";
            ViewBag.InhabitedSortParm = sortOrder == "Inhabited" ? "inhabited_desc" : "Inhabited";
            services = _unitOfWork.ServiceRepository.OrderServices(services, sortOrder).ToList();
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
            PopulateMeterTypesDropDownList();
            return View(service);
        }

        // POST: Service/Create
        [HttpPost]
        public ActionResult Create(Service service)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                var duplicateService = _unitOfWork.ServiceRepository.FirstOrDefault(s => s.Name == service.Name && s.InvoiceID == service.InvoiceID);
                if (duplicateService != null)
                {
                    PopulateInvoicesDropDownList(service.InvoiceID);
                    PopulateDistributionModesDropDownList(service.DistributionModeID);
                    PopulateMeterTypesDropDownList(service.MeterTypeID);
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
                if (service.Counted)
                {
                    if (service.MeterTypeID != null)
                    {
                        var meterType = _unitOfWork.MeterTypeRepository.Get((int)service.MeterTypeID);
                        if (meterType != null)
                        {
                            service.MeterType = meterType;
                            service.DistributionMode = null;
                        }
                    }
                }
                else
                {
                    if (service.DistributionModeID != null)
                    {
                        var distributionMode = _unitOfWork.DistributionModeRepository.Get((int)service.DistributionModeID);
                        if (distributionMode != null)
                        {
                            service.DistributionMode = distributionMode;
                            service.MeterType = null;
                        }
                    }
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
                    TempData["message"] = $"Service {service.Name} has been created.";
                    return Json(service.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateInvoicesDropDownList(service.InvoiceID);
            PopulateDistributionModesDropDownList(service.DistributionModeID);
            PopulateMeterTypesDropDownList(service.MeterTypeID);
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
            PopulateMeterTypesDropDownList(service.MeterTypeID);
            return View(service);
        }

        // POST: Service/Edit/5
        [HttpPost]
        public ActionResult Edit(Service service)
        {
            var serviceToUpdate = _unitOfWork.ServiceRepository.GetServiceIncludingInvoiceAndDistributionModeAndMeterTypeAndSectionsAndLevelsAndSpaces(service.ID);
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
            if (TryUpdateModel(serviceToUpdate, "", new[]{"Name", "Quantity", "Unit", "Price", "QuotaTVA", "Fixed", "Inhabited", "InvoiceID", "DistributionModeID", "MeterTypeID", "Counted"}))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateService = _unitOfWork.ServiceRepository.FirstOrDefault(s => s.ID != service.ID && s.Name == service.Name && s.InvoiceID == service.InvoiceID);
                    if (duplicateService != null)
                    {
                        PopulateInvoicesDropDownList(service.InvoiceID);
                        PopulateDistributionModesDropDownList(service.DistributionModeID);
                        PopulateMeterTypesDropDownList(service.MeterTypeID);
                        return new HttpStatusCodeResult(409, "A service with this name, for this invoice, already exists.");
                    }
                    if (service.Counted)
                    {
                        if (service.MeterTypeID != null)
                        {
                            var meterType = _unitOfWork.MeterTypeRepository.Get((int)service.MeterTypeID);
                            if (meterType != null)
                            {
                                service.MeterType = meterType;
                                service.DistributionMode = null;
                            }
                        }
                    }
                    else
                    {
                        if (service.DistributionModeID != null)
                        {
                            var distributionMode = _unitOfWork.DistributionModeRepository.Get((int)service.DistributionModeID);
                            if (distributionMode != null)
                            {
                                service.DistributionMode = distributionMode;
                                service.MeterType = null;
                            }
                        }
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
                    TempData["message"] = $"Service {serviceToUpdate.Name} has been edited.";
                    return Json(serviceToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateInvoicesDropDownList(serviceToUpdate.InvoiceID);
            PopulateDistributionModesDropDownList(serviceToUpdate.DistributionModeID);
            PopulateMeterTypesDropDownList(serviceToUpdate.MeterTypeID);
            return View(serviceToUpdate);
        }

        // GET: Service/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var service = _unitOfWork.ServiceRepository.GetServiceIncludingInvoiceAndDistributionModeAndMeterTypeAndSectionsAndLevelsAndSpaces(id);
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
                TempData["message"] = $"Service {service.Name} has been deleted.";
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new {id, saveChangesError = true});
            }
            if (PreviousPage.Equals("/Service/Index"))
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "InvoiceDistribution");
        }

        // GET: Service/Distribute/5
        public ActionResult Distribute(int id)
        {
            var serviceDistributeData = new ServiceDistributeData();
            var service = _unitOfWork.ServiceRepository.Get(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            serviceDistributeData.Service = service;
            serviceDistributeData.Costs = new List<Cost>();
            var totalCost = 0.0m;
            var totalSpaces = GetSpaces(service);
            var valueWithTVA = service.ValueWithoutTVA + service.TVA;
            //DistributionMode = cote parti
            if (service.DistributionModeID == 1)
            {
                var totalSurface = totalSpaces.Sum(s => s.Surface);
                if (totalSurface > 0)
                {
                    foreach (var space in totalSpaces)
                    {
                        var cost = new Cost();
                        var quota = space.Surface / totalSurface;
                        cost.Quota = quota;
                        cost.Value = quota * valueWithTVA;
                        cost.ServiceID = service.ID;
                        cost.SpaceID = space.ID;
                        totalCost += cost.Value;
                        serviceDistributeData.Costs.Add(cost);
                        _unitOfWork.CostRepository.Add(cost);
                    }
                }
            }
            //DistributionMode = numar persoane
            else if (service.DistributionModeID == 2)
            {
                var totalPeople = totalSpaces.Sum(s => s.People);
                if (totalPeople > 0)
                {
                    foreach (var space in totalSpaces)
                    {
                        var cost = new Cost();
                        var quota = ((decimal)space.People) / ((decimal)totalPeople);
                        cost.Quota = quota;
                        cost.Value = quota * valueWithTVA;
                        cost.ServiceID = service.ID;
                        cost.SpaceID = space.ID;
                        totalCost += cost.Value;
                        serviceDistributeData.Costs.Add(cost);
                        _unitOfWork.CostRepository.Add(cost);
                    }
                }
            }
            service.Distributed = true;
            try
            {
                _unitOfWork.Save();
                TempData["message"] = $"Service {service.Name} has been distributed.";
            }
            catch (DataException)
            {
                TempData["message"] = $"Unexpected error occurred. Service {service.Name} can not be distributed.";
                return RedirectToAction("Distribute", new { id, saveChangesError = true });
            }
            if (Request.UrlReferrer != null)
            {
                service.PreviousPage = Request.UrlReferrer.AbsolutePath;
            }
            return View(serviceDistributeData);
        }

        private List<Space> GetSpaces(Service service)
        {
            var totalSpaces = new List<Space>();
            foreach (var section in service.Sections)
            {
                var levels = _unitOfWork.LevelRepository.GetLevelsBySection(section.ID).ToList();
                foreach (var level in levels)
                {
                    var spaces = _unitOfWork.SpaceRepository.GetSpacesByLevel(level.ID).ToList();
                    foreach (var space in spaces)
                    {
                        if (service.Inhabited && !space.Inhabited)
                        {
                            continue;
                        }
                        totalSpaces.Add(space);
                    }
                }
            }
            foreach (var level in service.Levels)
            {
                var spaces = _unitOfWork.SpaceRepository.GetSpacesByLevel(level.ID).ToList();
                foreach (var space in spaces)
                {
                    if (service.Inhabited && !space.Inhabited)
                    {
                        continue;
                    }
                    totalSpaces.Add(space);
                }
            }
            foreach (var space in service.Spaces)
            {
                if (service.Inhabited && !space.Inhabited)
                {
                    continue;
                }
                totalSpaces.Add(space);
            }
            return totalSpaces;
        }

        // GET: Service/Undistribute/5
        public ActionResult Undistribute(int id)
        {
            var service = _unitOfWork.ServiceRepository.Get(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            var costs = _unitOfWork.CostRepository.GetCostsByService(service.ID).ToList();
            foreach (var cost in costs)
            {
                _unitOfWork.CostRepository.Remove(cost);
            }
            service.Distributed = false;
            try
            {
                _unitOfWork.Save();
                TempData["message"] = $"Service {service.Name} has been undistributed.";
            }
            catch (DataException)
            {
                TempData["message"] = $"Unexpected error occurred. Service {service.Name} can not be undistributed.";
                return RedirectToAction("Undistribute", new { id, saveChangesError = true });
            }
            if (Request.UrlReferrer.AbsolutePath.Equals("/Service/Index"))
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "InvoiceDistribution");
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
            var invoicesQuery = _unitOfWork.InvoiceRepository.GetAll().Where(i => i.Closed == false).ToList();
            ViewBag.InvoiceID = new SelectList(invoicesQuery, "ID", "Number", selectedInvoice);
        }

        private void PopulateDistributionModesDropDownList(object selectedDistributionMode = null)
        {
            var distributionModesQuery = _unitOfWork.DistributionModeRepository.GetAll().ToList();
            ViewBag.DistributionModeID = new SelectList(distributionModesQuery, "ID", "Mode", selectedDistributionMode);
        }

        private void PopulateMeterTypesDropDownList(object selectedMeterType = null)
        {
            var meterTypesQuery = _unitOfWork.MeterTypeRepository.GetAll().ToList();
            ViewBag.MeterTypeID = new SelectList(meterTypesQuery, "ID", "Type", selectedMeterType);
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
