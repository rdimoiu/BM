using BuildingManagement.DAL;
using BuildingManagement.Models;
using BuildingManagement.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
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
                service.InvoiceID = (int)invoiceId;
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
            if (TryUpdateModel(serviceToUpdate, "", new[] { "Name", "Quantity", "Unit", "Price", "QuotaTVA", "Fixed", "Inhabited", "InvoiceID", "DistributionModeID", "MeterTypeID", "Counted" }))
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
                if (!service.Rest)
                {
                    invoice.Quantity = invoice.Quantity - service.Quantity;
                    invoice.TotalValueWithoutTVA = invoice.TotalValueWithoutTVA - service.ValueWithoutTVA;
                    invoice.TotalTVA = invoice.TotalTVA - service.TVA;
                }
                _unitOfWork.ServiceRepository.Remove(service);
                _unitOfWork.Save();
                TempData["message"] = $"Service {service.Name} has been deleted.";
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new { id, saveChangesError = true });
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
            var errorMessages = "no error";
            var serviceDistributeData = new ServiceDistributeData();
            var service = _unitOfWork.ServiceRepository.GetServiceIncludingInvoiceAndDistributionModeAndMeterTypeAndSectionsAndLevelsAndSpaces(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            serviceDistributeData.Service = service;
            serviceDistributeData.Costs = new List<Cost>();
            var totalCost = 0.0m;
            var totalServiceSpaces = GetServiceSpaces(service);
            var valueWithTVA = service.ValueWithoutTVA + service.TVA;

            if (service.Counted)
            {
                var totalConsumption = 0.0m;
                #region meter consumption
                var meters = _unitOfWork.MeterRepository.GetAllMetersIncludingMeterTypesAndDistributionModeAndClientAndSectionsAndLevelsAndSpaces().ToList();
                foreach (var meter in meters)
                {
                    if (meter.MeterTypes.Contains(service.MeterType))
                    {
                        var meterConsumption = 0.0m;
                        var lastMeterReading = _unitOfWork.MeterReadingRepository.GetLastMeterReading(meter.ID, service.MeterType.ID, service.Invoice.DiscountMonth).ToList();
                        if (lastMeterReading.Count != 1)
                        {
                            TempData["message"] += $"Meter {meter.Code}|{service.MeterType.Type} has no reading with discount month {service.Invoice.DiscountMonth}.\n";
                            continue;
                        }
                        var previousMeterReading = _unitOfWork.MeterReadingRepository.GetPreviousMeterReading(meter.ID, service.MeterType.ID, service.Invoice.DiscountMonth).ToList();
                        if (previousMeterReading.Count != 1)
                        {
                            var initialMeterReading = _unitOfWork.MeterReadingRepository.GetInitialMeterReading(meter.ID, service.MeterType.ID).ToList();
                            if (initialMeterReading.Count != 1)
                            {
                                TempData["message"] += $"Meter {meter.Code}|{service.MeterType.Type} has no previous or initial reading.\n";
                                continue;
                            }
                            meterConsumption = lastMeterReading[0].Index - initialMeterReading[0].Index;
                            totalConsumption += meterConsumption;
                        }
                        else
                        {
                            meterConsumption = lastMeterReading[0].Index - previousMeterReading[0].Index;
                            totalConsumption += meterConsumption;
                        }
                        var totalMeterSpaces = GetMeterSpaces(meter, service.Inhabited);
                        //DistributionMode = cote parti
                        if (meter.DistributionModeID == 1)
                        {
                            var totalSurface = totalMeterSpaces.Sum(s => s.Surface);
                            foreach (var space in totalServiceSpaces)
                            {
                                if (totalMeterSpaces.Contains(space))
                                {
                                    var cost = new Cost();
                                    var quota = space.Surface / totalSurface;
                                    cost.Quota = quota;
                                    cost.Value = quota * meterConsumption * service.Price;
                                    cost.ServiceID = service.ID;
                                    cost.SpaceID = space.ID;
                                    totalCost += cost.Value;
                                    serviceDistributeData.Costs.Add(cost);
                                    _unitOfWork.CostRepository.Add(cost);
                                }
                            }
                        }
                        //DistributionMode = numar persoane
                        else if (meter.DistributionModeID == 2)
                        {
                            var totalPeople = totalServiceSpaces.Sum(s => s.People);
                            if (totalPeople > 0)
                            {
                                foreach (var space in totalServiceSpaces)
                                {
                                    if (totalMeterSpaces.Contains(space))
                                    {
                                        var cost = new Cost();
                                        var quota = ((decimal)space.People) / ((decimal)totalPeople);
                                        cost.Quota = quota;
                                        cost.Value = quota * meterConsumption * service.Price;
                                        cost.ServiceID = service.ID;
                                        cost.SpaceID = space.ID;
                                        totalCost += cost.Value;
                                        serviceDistributeData.Costs.Add(cost);
                                        _unitOfWork.CostRepository.Add(cost);
                                    }
                                }
                            }
                        }
                        //DistributionMode = consum
                        else if (meter.DistributionModeID == 3)
                        {
                            if (totalMeterSpaces.Count == 1)
                            {
                                foreach (var space in totalServiceSpaces)
                                {
                                    if (totalMeterSpaces.Contains(space))
                                    {
                                        var cost = new Cost();
                                        var quota = 1;
                                        cost.Quota = quota;
                                        cost.Value = quota * meterConsumption * service.Price;
                                        cost.ServiceID = service.ID;
                                        cost.SpaceID = space.ID;
                                        totalCost += cost.Value;
                                        serviceDistributeData.Costs.Add(cost);
                                        _unitOfWork.CostRepository.Add(cost);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                #region submeter consumption
                var subMeters = _unitOfWork.SubMeterRepository.GetAllSubMetersIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces("").ToList();
                foreach (var subMeter in subMeters)
                {
                    if (subMeter.MeterTypes.Contains(service.MeterType))
                    {
                        var subMeterConsumption = 0.0m;
                        var lastSubMeterReading = _unitOfWork.SubMeterReadingRepository.GetLastSubMeterReading(subMeter.ID, service.MeterType.ID, service.Invoice.DiscountMonth).ToList();
                        if (lastSubMeterReading.Count != 1)
                        {
                            TempData["message"] += $"SubMeter {subMeter.Code}|{service.MeterType.Type} has no reading with discount month {service.Invoice.DiscountMonth}.\n";
                            continue;
                        }
                        var previousSubMeterReading = _unitOfWork.SubMeterReadingRepository.GetPreviousSubMeterReading(subMeter.ID, service.MeterType.ID, service.Invoice.DiscountMonth).ToList();
                        if (previousSubMeterReading.Count != 1)
                        {
                            var initialSubMeterReading = _unitOfWork.SubMeterReadingRepository.GetInitialSubMeterReading(subMeter.ID, service.MeterType.ID).ToList();
                            if (initialSubMeterReading.Count != 1)
                            {
                                TempData["message"] += $"SubMeter {subMeter.Code}|{service.MeterType.Type} has no previous or initial reading.\n";
                                continue;
                            }
                            subMeterConsumption = lastSubMeterReading[0].Index - initialSubMeterReading[0].Index;
                            totalConsumption += subMeterConsumption;
                        }
                        else
                        {
                            subMeterConsumption = lastSubMeterReading[0].Index - previousSubMeterReading[0].Index;
                            totalConsumption += subMeterConsumption;
                        }
                        var totalSubMeterSpaces = GetSubMeterSpaces(subMeter, service.Inhabited);
                        //DistributionMode = cote parti
                        if (subMeter.DistributionModeID == 1)
                        {
                            var totalSurface = totalSubMeterSpaces.Sum(s => s.Surface);
                            foreach (var space in totalServiceSpaces)
                            {
                                if (totalSubMeterSpaces.Contains(space))
                                {
                                    var cost = new Cost();
                                    var quota = space.Surface / totalSurface;
                                    cost.Quota = quota;
                                    cost.Value = quota * subMeterConsumption * service.Price;
                                    cost.ServiceID = service.ID;
                                    cost.SpaceID = space.ID;
                                    totalCost += cost.Value;
                                    serviceDistributeData.Costs.Add(cost);
                                    _unitOfWork.CostRepository.Add(cost);
                                }
                            }
                        }
                        //DistributionMode = numar persoane
                        else if (subMeter.DistributionModeID == 2)
                        {
                            var totalPeople = totalServiceSpaces.Sum(s => s.People);
                            if (totalPeople > 0)
                            {
                                foreach (var space in totalServiceSpaces)
                                {
                                    if (totalSubMeterSpaces.Contains(space))
                                    {
                                        var cost = new Cost();
                                        var quota = ((decimal)space.People) / ((decimal)totalPeople);
                                        cost.Quota = quota;
                                        cost.Value = quota * subMeterConsumption * service.Price;
                                        cost.ServiceID = service.ID;
                                        cost.SpaceID = space.ID;
                                        totalCost += cost.Value;
                                        serviceDistributeData.Costs.Add(cost);
                                        _unitOfWork.CostRepository.Add(cost);
                                    }
                                }
                            }
                        }
                        //DistributionMode = consum
                        else if (subMeter.DistributionModeID == 3)
                        {
                            if (totalSubMeterSpaces.Count == 1)
                            {
                                foreach (var space in totalServiceSpaces)
                                {
                                    if (totalSubMeterSpaces.Contains(space))
                                    {
                                        var cost = new Cost();
                                        var quota = 1;
                                        cost.Quota = quota;
                                        cost.Value = quota * subMeterConsumption * service.Price;
                                        cost.ServiceID = service.ID;
                                        cost.SpaceID = space.ID;
                                        totalCost += cost.Value;
                                        serviceDistributeData.Costs.Add(cost);
                                        _unitOfWork.CostRepository.Add(cost);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                #region subsubmeter consumption
                var subSubMeters = _unitOfWork.SubSubMeterRepository.GetAllSubSubMetersIncludingMeterTypesAndDistributionModeAndSubMeterAndSectionsAndLevelsAndSpaces().ToList();
                foreach (var subSubMeter in subSubMeters)
                {
                    if (subSubMeter.MeterTypes.Contains(service.MeterType))
                    {
                        var subSubMeterConsumption = 0.0m;
                        var lastSubSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.GetLastSubSubMeterReading(subSubMeter.ID, service.MeterType.ID, service.Invoice.DiscountMonth).ToList();
                        if (lastSubSubMeterReading.Count != 1)
                        {
                            TempData["message"] += $"SubSubMeter {subSubMeter.Code}|{service.MeterType.Type} has no reading with discount month {service.Invoice.DiscountMonth}.\n";
                            continue;
                        }
                        var previousSubSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.GetPreviousSubSubMeterReading(subSubMeter.ID, service.MeterType.ID, service.Invoice.DiscountMonth).ToList();
                        if (previousSubSubMeterReading.Count != 1)
                        {
                            var initialSubMeterReading = _unitOfWork.SubSubMeterReadingRepository.GetInitialSubSubMeterReading(subSubMeter.ID, service.MeterType.ID).ToList();
                            if (initialSubMeterReading.Count != 1)
                            {
                                TempData["message"] += $"SubSubMeter {subSubMeter.Code}|{service.MeterType.Type} has no previous or initial reading.\n";
                                continue;
                            }
                            subSubMeterConsumption = lastSubSubMeterReading[0].Index - initialSubMeterReading[0].Index;
                            totalConsumption += subSubMeterConsumption;
                        }
                        else
                        {
                            subSubMeterConsumption = lastSubSubMeterReading[0].Index - previousSubSubMeterReading[0].Index;
                            totalConsumption += subSubMeterConsumption;
                        }
                        var totalSubSubMeterSpaces = GetSubSubMeterSpaces(subSubMeter, service.Inhabited);
                        //DistributionMode = cote parti
                        if (subSubMeter.DistributionModeID == 1)
                        {
                            var totalSurface = totalSubSubMeterSpaces.Sum(s => s.Surface);
                            foreach (var space in totalServiceSpaces)
                            {
                                if (totalSubSubMeterSpaces.Contains(space))
                                {
                                    var cost = new Cost();
                                    var quota = space.Surface / totalSurface;
                                    cost.Quota = quota;
                                    cost.Value = quota * subSubMeterConsumption * service.Price;
                                    cost.ServiceID = service.ID;
                                    cost.SpaceID = space.ID;
                                    totalCost += cost.Value;
                                    serviceDistributeData.Costs.Add(cost);
                                    _unitOfWork.CostRepository.Add(cost);
                                }
                            }
                        }
                        //DistributionMode = numar persoane
                        else if (subSubMeter.DistributionModeID == 2)
                        {
                            var totalPeople = totalServiceSpaces.Sum(s => s.People);
                            if (totalPeople > 0)
                            {
                                foreach (var space in totalServiceSpaces)
                                {
                                    if (totalSubSubMeterSpaces.Contains(space))
                                    {
                                        var cost = new Cost();
                                        var quota = ((decimal)space.People) / ((decimal)totalPeople);
                                        cost.Quota = quota;
                                        cost.Value = quota * subSubMeterConsumption * service.Price;
                                        cost.ServiceID = service.ID;
                                        cost.SpaceID = space.ID;
                                        totalCost += cost.Value;
                                        serviceDistributeData.Costs.Add(cost);
                                        _unitOfWork.CostRepository.Add(cost);
                                    }
                                }
                            }
                        }
                        //DistributionMode = consum
                        else if (subSubMeter.DistributionModeID == 3)
                        {
                            if (totalSubSubMeterSpaces.Count == 1)
                            {
                                foreach (var space in totalServiceSpaces)
                                {
                                    if (totalSubSubMeterSpaces.Contains(space))
                                    {
                                        var cost = new Cost();
                                        var quota = 1;
                                        cost.Quota = quota;
                                        cost.Value = quota * subSubMeterConsumption * service.Price;
                                        cost.ServiceID = service.ID;
                                        cost.SpaceID = space.ID;
                                        totalCost += cost.Value;
                                        serviceDistributeData.Costs.Add(cost);
                                        _unitOfWork.CostRepository.Add(cost);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                if (service.Quantity != totalConsumption)
                {
                    var restService = new Service();
                    restService.Rest = true;
                    restService.Name = service.Name + " #Rest#";
                    restService.Quantity = service.Quantity - totalConsumption;
                    restService.Unit = service.Unit;
                    restService.Price = service.Price;
                    restService.QuotaTVA = service.QuotaTVA;
                    restService.Counted = false;
                    restService.Inhabited = service.Inhabited;
                    restService.InvoiceID = service.InvoiceID;
                    restService.MeterTypeID = service.MeterTypeID;
                    restService.Spaces = service.Spaces;
                    restService.Levels = service.Levels;
                    restService.Sections = service.Sections;
                    _unitOfWork.ServiceRepository.Add(restService);

                    TempData["message"] += $"Select distribution mode for the {restService.Name}.";
                }
            }
            //DistributionMode = cote parti
            else if (service.DistributionModeID == 1)
            {
                var totalSurface = totalServiceSpaces.Sum(s => s.Surface);
                if (totalSurface > 0)
                {
                    foreach (var space in totalServiceSpaces)
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
                var totalPeople = totalServiceSpaces.Sum(s => s.People);
                if (totalPeople > 0)
                {
                    foreach (var space in totalServiceSpaces)
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
                TempData["message"] += $"Service {service.Name} has been distributed.";
            }
            catch (DataException ex)
            {
                TempData["message"] = $"Unexpected error occurred. Service {service.Name} can not be distributed.";
                return RedirectToAction("Index", new { id, saveChangesError = true });
            }
            if (Request.UrlReferrer != null)
            {
                service.PreviousPage = Request.UrlReferrer.AbsolutePath;
            }
            return View(serviceDistributeData);
        }

        private List<Space> GetServiceSpaces(Service service)
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

        private List<Space> GetMeterSpaces(Meter meter, bool inhabited)
        {
            var totalSpaces = new List<Space>();
            foreach (var section in meter.Sections)
            {
                var levels = _unitOfWork.LevelRepository.GetLevelsBySection(section.ID).ToList();
                foreach (var level in levels)
                {
                    var spaces = _unitOfWork.SpaceRepository.GetSpacesByLevel(level.ID).ToList();
                    foreach (var space in spaces)
                    {
                        if (inhabited && !space.Inhabited)
                        {
                            continue;
                        }
                        totalSpaces.Add(space);
                    }
                }
            }
            foreach (var level in meter.Levels)
            {
                var spaces = _unitOfWork.SpaceRepository.GetSpacesByLevel(level.ID).ToList();
                foreach (var space in spaces)
                {
                    if (inhabited && !space.Inhabited)
                    {
                        continue;
                    }
                    totalSpaces.Add(space);
                }
            }
            foreach (var space in meter.Spaces)
            {
                if (inhabited && !space.Inhabited)
                {
                    continue;
                }
                totalSpaces.Add(space);
            }
            return totalSpaces;
        }

        private List<Space> GetSubMeterSpaces(SubMeter subMeter, bool inhabited)
        {
            var totalSpaces = new List<Space>();
            foreach (var section in subMeter.Sections)
            {
                var levels = _unitOfWork.LevelRepository.GetLevelsBySection(section.ID).ToList();
                foreach (var level in levels)
                {
                    var spaces = _unitOfWork.SpaceRepository.GetSpacesByLevel(level.ID).ToList();
                    foreach (var space in spaces)
                    {
                        if (inhabited && !space.Inhabited)
                        {
                            continue;
                        }
                        totalSpaces.Add(space);
                    }
                }
            }
            foreach (var level in subMeter.Levels)
            {
                var spaces = _unitOfWork.SpaceRepository.GetSpacesByLevel(level.ID).ToList();
                foreach (var space in spaces)
                {
                    if (inhabited && !space.Inhabited)
                    {
                        continue;
                    }
                    totalSpaces.Add(space);
                }
            }
            foreach (var space in subMeter.Spaces)
            {
                if (inhabited && !space.Inhabited)
                {
                    continue;
                }
                totalSpaces.Add(space);
            }
            return totalSpaces;
        }

        private List<Space> GetSubSubMeterSpaces(SubSubMeter subSubMeter, bool inhabited)
        {
            var totalSpaces = new List<Space>();
            foreach (var section in subSubMeter.Sections)
            {
                var levels = _unitOfWork.LevelRepository.GetLevelsBySection(section.ID).ToList();
                foreach (var level in levels)
                {
                    var spaces = _unitOfWork.SpaceRepository.GetSpacesByLevel(level.ID).ToList();
                    foreach (var space in spaces)
                    {
                        if (inhabited && !space.Inhabited)
                        {
                            continue;
                        }
                        totalSpaces.Add(space);
                    }
                }
            }
            foreach (var level in subSubMeter.Levels)
            {
                var spaces = _unitOfWork.SpaceRepository.GetSpacesByLevel(level.ID).ToList();
                foreach (var space in spaces)
                {
                    if (inhabited && !space.Inhabited)
                    {
                        continue;
                    }
                    totalSpaces.Add(space);
                }
            }
            foreach (var space in subSubMeter.Spaces)
            {
                if (inhabited && !space.Inhabited)
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
                return RedirectToAction("Index", new { id, saveChangesError = true });
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
            var selectedSpacesIDs = new HashSet<int>();
            var selectedLevelsIDs = new HashSet<int>();
            var selectedSectionsIDs = new HashSet<int>();
            var isRestService = false;
            if (serviceId != null && serviceId != 0)
            {
                var service = _unitOfWork.ServiceRepository.GetServiceIncludingSectionsAndLevelsAndSpaces((int)serviceId);
                if (service != null)
                {
                    selectedSpacesIDs = new HashSet<int>(service.Spaces.Select(s => s.ID));
                    selectedLevelsIDs = new HashSet<int>(service.Levels.Select(l => l.ID));
                    selectedSectionsIDs = new HashSet<int>(service.Sections.Select(s => s.ID));
                    isRestService = service.Rest;
                }
            }

            var root = new TreeNode
            {
                id = "root",
                children = { },
                text = "-",
                state = new TreeNodeState { opened = true, disabled = isRestService }
            };

            if (invoiceId != null && invoiceId != 0)
            {
                var invoice = _unitOfWork.InvoiceRepository.Get((int)invoiceId);
                if (invoice != null)
                {
                    var treeHelper = new Utils.TreeHelper(_unitOfWork);
                    root = treeHelper.GetSectionsLevelsSpacesByClient(root, invoice.ClientID, selectedSectionsIDs, selectedLevelsIDs, selectedSpacesIDs, isRestService);
                }
            }

            return new JavaScriptSerializer().Serialize(root);
        }
    }
}
