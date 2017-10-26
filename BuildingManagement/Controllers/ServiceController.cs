using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using BuildingManagement.ViewModels;

namespace BuildingManagement.Controllers
{
    public class ServiceController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        // GET: Service
        public ActionResult Index()
        {
            var viewModel = new ServiceIndexData
            {
                Services = _unitOfWork.ServiceRepository.Get(includeProperties: "Invoice, DistributionMode, Sections, Levels, Spaces")
            };
            return View(viewModel);
        }

        // GET: Service/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var service = _unitOfWork.ServiceRepository.GetById(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View(service);
        }

        // GET: Service/Create
        public ActionResult Create(int? invoiceId)
        {
            var model = new Service();
            if (invoiceId != null)
            {
                model.InvoiceID = (int) invoiceId;
            }
            if (Request.UrlReferrer != null && Request.UrlReferrer.AbsoluteUri.Contains("Distribution"))
            {
                model.PreviousPage = "InvoiceDistribution";
            }
            else
            {
                model.PreviousPage = "Invoice";
            }
            PopulateInvoicesDropDownList();
            PopulateDistributionModesDropDownList();
            return View(model);
        }

        // POST: Service/Create
        [HttpPost]
        public ActionResult CreateService(Service service)
        {
            if (service.InvoiceID != 0)
            {
                var invoice = _unitOfWork.InvoiceRepository.GetById(service.InvoiceID);
                if (invoice != null)
                {
                    invoice.Quantity = invoice.Quantity + service.Quantity;
                    invoice.TotalValueWithoutTVA = invoice.TotalValueWithoutTVA + service.ValueWithoutTVA;
                    invoice.TotalTVA = invoice.TotalTVA + service.TVA;
                    service.Invoice = invoice;
                }
            }
            if (service.DistributionModeID != 0)
            {
                var distributionMode = _unitOfWork.DistributionModeRepository.GetById(service.DistributionModeID);
                if (distributionMode != null)
                {
                    service.DistributionMode = distributionMode;
                }
            }
            if (service.ServiceSLSSelected != null)
            {
                #region Add Sections, Levels and Spaces

                var building = Utils.MapSelectedIDsToDatabaseIDs(service.ServiceSLSSelected);
                service.Sections = new List<Section>();
                foreach (var sectionId in building.SectionIDs)
                {
                    var section = _unitOfWork.SectionRepository.GetById(sectionId);
                    if (section != null)
                    {
                        service.Sections.Add(section);
                    }
                }
                service.Levels = new List<Level>();
                foreach (var levelId in building.LevelIDs)
                {
                    var level = _unitOfWork.LevelRepository.GetById(levelId);
                    if (level != null)
                    {
                        service.Levels.Add(level);
                    }
                }
                service.Spaces = new List<Space>();
                foreach (var spaceId in building.SpaceIDs)
                {
                    var space = _unitOfWork.SpaceRepository.GetById(spaceId);
                    if (space != null)
                    {
                        service.Spaces.Add(space);
                    }
                }

                #endregion
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.ServiceRepository.Insert(service);
                    _unitOfWork.Save();
                    //if (service.PreviousPage.Equals("Invoice"))
                    //{
                    //    return RedirectToAction("Index", "Service");
                    //}
                    //return RedirectToAction("Index", "InvoiceDistribution", new { service.Invoice.ClientID, service.Invoice.ProviderID });
                    return Json(service.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("",
                        "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateInvoicesDropDownList(service.InvoiceID);
            PopulateDistributionModesDropDownList(service.DistributionModeID);
            return View("Create", service);
        }

        // GET: Service/Edit/5
        public ActionResult Edit(int? id, int? invoiceId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var service =
                _unitOfWork.ServiceRepository.Get(includeProperties: "Invoice, DistributionMode, Sections, Levels, Spaces")
                    .Single(s => s.ID == id);
            if (service == null)
            {
                return HttpNotFound();
            }
            PopulateInvoicesDropDownList(service.InvoiceID);
            PopulateDistributionModesDropDownList(service.DistributionModeID);
            return View(service);
        }

        // POST: Service/Edit/5
        [HttpPost]
        public ActionResult EditService(Service service)
        {
            var serviceToUpdate =
                _unitOfWork.ServiceRepository.Get(includeProperties: "Invoice, DistributionMode, Sections, Levels, Spaces")
                    .FirstOrDefault(s => s.ID == service.ID);
            if (serviceToUpdate == null)
            {
                return HttpNotFound();
            }
            var oldService = _unitOfWork.ServiceRepository.GetById(service.ID);
            if (oldService == null)
            {
                return HttpNotFound();
            }
            var oldInvoice = _unitOfWork.InvoiceRepository.GetById(service.InvoiceID);
            if (oldInvoice == null)
            {
                return HttpNotFound();
            }
            oldInvoice.Quantity = oldInvoice.Quantity - oldService.Quantity;
            oldInvoice.TotalValueWithoutTVA = oldInvoice.TotalValueWithoutTVA - oldService.ValueWithoutTVA;
            oldInvoice.TotalTVA = oldInvoice.TotalTVA - oldService.TVA;
            if (TryUpdateModel(serviceToUpdate, "",
                new[]
                {
                    "Name", "Quantity", "Unit", "Price", "TVA", "ValueWithoutTVA", "QuotaTVA", "Fixed", "Inhabited",
                    "InvoiceID", "DistributionModeID", "Counted"
                }))
            {
                try
                {
                    #region Update Sections, Levels and Spaces

                    var building = Utils.MapSelectedIDsToDatabaseIDs(service.ServiceSLSSelected);
                    serviceToUpdate.Sections.Clear();
                    foreach (var sectionId in building.SectionIDs)
                    {
                        var section = _unitOfWork.SectionRepository.GetById(sectionId);
                        if (section != null)
                        {
                            serviceToUpdate.Sections.Add(section);
                        }
                    }
                    serviceToUpdate.Levels.Clear();
                    foreach (var levelId in building.LevelIDs)
                    {
                        var level = _unitOfWork.LevelRepository.GetById(levelId);
                        if (level != null)
                        {
                            serviceToUpdate.Levels.Add(level);
                        }
                    }
                    serviceToUpdate.Spaces.Clear();
                    foreach (var spaceId in building.SpaceIDs)
                    {
                        var space = _unitOfWork.SpaceRepository.GetById(spaceId);
                        if (space != null)
                        {
                            serviceToUpdate.Spaces.Add(space);
                        }
                    }

                    #endregion

                    var invoice = _unitOfWork.InvoiceRepository.GetById(serviceToUpdate.InvoiceID);
                    if (invoice == null)
                    {
                        return HttpNotFound();
                    }
                    invoice.Quantity = invoice.Quantity + service.Quantity;
                    invoice.TotalValueWithoutTVA = invoice.TotalValueWithoutTVA + service.ValueWithoutTVA;
                    invoice.TotalTVA = invoice.TotalTVA + service.TVA;
                    _unitOfWork.Save();
                    return Json(serviceToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("",
                        "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateInvoicesDropDownList(serviceToUpdate.InvoiceID);
            PopulateDistributionModesDropDownList(serviceToUpdate.DistributionModeID);
            return View("Edit", serviceToUpdate);
        }

        // GET: Service/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage =
                    "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var service = _unitOfWork.ServiceRepository.GetById(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View(service);
        }

        // POST: Service/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var serviceToDelete = _unitOfWork.ServiceRepository.GetById(id);
                if (serviceToDelete == null)
                {
                    return HttpNotFound();
                }
                var invoice = _unitOfWork.InvoiceRepository.GetById(serviceToDelete.InvoiceID);
                if (invoice == null)
                {
                    return HttpNotFound();
                }
                invoice.Quantity = invoice.Quantity - serviceToDelete.Quantity;
                invoice.TotalValueWithoutTVA = invoice.TotalValueWithoutTVA - serviceToDelete.ValueWithoutTVA;
                invoice.TotalTVA = invoice.TotalTVA - serviceToDelete.TVA;
                _unitOfWork.ServiceRepository.Delete(id);
                _unitOfWork.Save();
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
            var invoicesQuery = from i in _unitOfWork.InvoiceRepository.Get() select i;
            ViewBag.InvoiceID = new SelectList(invoicesQuery, "ID", "Number", selectedInvoice);
        }

        private void PopulateDistributionModesDropDownList(object selectedDistributionMode = null)
        {
            var distributionModesQuery = from dm in _unitOfWork.DistributionModeRepository.Get() select dm;
            ViewBag.DistributionModeID = new SelectList(distributionModesQuery, "ID", "Mode", selectedDistributionMode);
        }

        [HttpGet]
        public string GetSpacesTreeData(int? serviceId, int invoiceId)
        {
            var root = new TreeNode
            {
                id = "root",
                children = {},
                text = "root",
                state = new TreeNodeState {opened = true}
            };

            HashSet<int> selectedSpacesIDs = new HashSet<int>();
            HashSet<int> selectedLevelsIDs = new HashSet<int>();
            HashSet<int> selectedSectionsIDs = new HashSet<int>();
            if (serviceId != null)
            {
                var service =
                    _unitOfWork.ServiceRepository.Get(includeProperties: "Sections, Levels, Spaces")
                        .FirstOrDefault(s => s.ID == serviceId);
                if (service != null)
                {
                    selectedSpacesIDs = new HashSet<int>(service.Spaces.Select(s => s.ID));
                    selectedLevelsIDs = new HashSet<int>(service.Levels.Select(l => l.ID));
                    selectedSectionsIDs = new HashSet<int>(service.Sections.Select(s => s.ID));
                }
            }

            var invoice = _unitOfWork.InvoiceRepository.Get().FirstOrDefault(i => i.ID == invoiceId);
            if (invoice != null)
            {
                var sections = _unitOfWork.SectionRepository.Get().Where(s => s.ClientID == invoice.ClientID).ToList();
                if (sections.Any())
                {
                    foreach (var section in sections)
                    {
                        var sectionNode = new TreeNode();
                        sectionNode.id = section.ID.ToString();
                        sectionNode.text = section.Number;
                        sectionNode.state = new TreeNodeState();
                        sectionNode.state.opened = true;
                        if (selectedSectionsIDs.Count > 0 && selectedSectionsIDs.Contains(section.ID))
                        {
                            sectionNode.state.selected = true;
                        }
                        root.children.Add(sectionNode);
                        var levels = _unitOfWork.LevelRepository.Get().Where(l => l.SectionID == section.ID).ToList();
                        if (levels.Any())
                        {
                            foreach (var level in levels)
                            {
                                var levelNode = new TreeNode();
                                levelNode.id = section.ID + "." + level.ID;
                                levelNode.text = level.Number;
                                levelNode.state = new TreeNodeState();
                                levelNode.state.opened = true;
                                if (selectedLevelsIDs.Count > 0 && selectedLevelsIDs.Contains(level.ID))
                                {
                                    levelNode.state.selected = true;
                                }
                                sectionNode.children.Add(levelNode);
                                var spaces =
                                    _unitOfWork.SpaceRepository.Get().Where(s => s.LevelID == level.ID).ToList();
                                if (spaces.Any())
                                {
                                    foreach (var space in spaces)
                                    {
                                        var spaceNode = new TreeNode();
                                        spaceNode.id = section.ID + "." + level.ID + "." + space.ID;
                                        spaceNode.text = space.Number;
                                        if (selectedSpacesIDs.Count > 0 && selectedSpacesIDs.Contains(space.ID))
                                        {
                                            spaceNode.state = new TreeNodeState();
                                            spaceNode.state.selected = true;
                                        }
                                        levelNode.children.Add(spaceNode);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return new JavaScriptSerializer().Serialize(root);
        }
    }
}
