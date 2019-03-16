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
    public class MeterController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MeterController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Meter
        public ActionResult Index(int? page, string currentFilter, string searchString, string sortOrder)
        {
            IEnumerable<Meter> meters;
            //var pageNumber = page ?? 1;
            //const int pageSize = 3;

            //meters = _unitOfWork.MeterRepository.GetAllNew(includeProperties: "MeterTypes, DistributionMode, Client, Sections, Levels, Spaces");

            //if (searchString != null)
            //{
            //    pageNumber = 1;
            //    meters = _unitOfWork.MeterRepository.GetFilteredMetersIncludingMeterTypesAndDistributionModeAndClientAndSectionsAndLevelsAndSpaces(searchString);
            //}
            //else
            //{
            //    if (currentFilter != null)
            //    {
            //        searchString = currentFilter;
            //        meters = _unitOfWork.MeterRepository.GetFilteredMetersIncludingMeterTypesAndDistributionModeAndClientAndSectionsAndLevelsAndSpaces(searchString);
            //    }
            //    else
            //    {
                    meters = _unitOfWork.MeterRepository.GetAllMetersIncludingMeterTypesAndDistributionModeAndClientAndSectionsAndLevelsAndSpaces().ToList();
            //    }
            //}
            //ViewBag.CurrentFilter = searchString;
            //ViewBag.CurrentSort = sortOrder;
            //ViewBag.CodeSortParm = string.IsNullOrEmpty(sortOrder) ? "code_desc" : "";
            //ViewBag.DetailsSortParm = sortOrder == "Details" ? "details_desc" : "Details";
            //ViewBag.DefectSortParm = sortOrder == "Defect" ? "defect_desc" : "Defect";
            //ViewBag.DistributionModeSortParm = sortOrder == "DistributionMode" ? "distributionMode_desc" : "DistributionMode";
            //ViewBag.ClientSortParm = sortOrder == "Client" ? "client_desc" : "Client";
            //meters = _unitOfWork.MeterRepository.OrderMeters(meters, sortOrder);
            //ViewBag.OnePageOfMeters = meters.ToPagedList(pageNumber, pageSize);
            //return View(ViewBag.OnePageOfMeters);
            return View(meters);
        }

        // GET: Meter/Details/5
        public ActionResult Details(int id)
        {
            var meter = _unitOfWork.MeterRepository.Get(id);
            if (meter == null)
            {
                return HttpNotFound();
            }
            return View(meter);
        }

        // GET: Meter/Create
        public ActionResult Create()
        {
            var meter = new Meter();
            PopulateDistributionModesDropDownList();
            PopulateClientsDropDownList();
            return View(meter);
        }

        // POST: Meter/Create
        [HttpPost]
        public ActionResult Create(Meter meter)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                var duplicateMeter = _unitOfWork.MeterRepository.FirstOrDefault(m => m.Code == meter.Code);
                if (duplicateMeter != null)
                {
                    PopulateDistributionModesDropDownList(meter.DistributionModeID);
                    PopulateClientsDropDownList(meter.ClientID);
                    return new HttpStatusCodeResult(409, "A meter with this code already exists.");
                }
                var distributionMode = _unitOfWork.DistributionModeRepository.Get(meter.DistributionModeID);
                if (distributionMode != null)
                {
                    meter.DistributionMode = distributionMode;
                }
                var client = _unitOfWork.ClientRepository.Get(meter.ClientID);
                if (client != null)
                {
                    meter.Client = client;
                }
                if (meter.MeterTypesSelected != null)
                {
                    meter.MeterTypes = new List<MeterType>();
                    foreach (var meterTypeSelected in meter.MeterTypesSelected)
                    {
                        if (meterTypeSelected != "root")
                        {
                            var meterType = _unitOfWork.MeterTypeRepository.Get(int.Parse(meterTypeSelected));
                            if (meterType != null)
                            {
                                meter.MeterTypes.Add(meterType);
                            }
                        }
                    }
                }
                if (meter.MeterSLSSelected != null)
                {
                    #region Add Sections, Levels and Spaces

                    var tree3D = Utils.TreeHelper.MapSelectedIDsToDatabaseIDsForSpaces(meter.MeterSLSSelected);
                    meter.Sections = new List<Section>();
                    foreach (var sectionId in tree3D.SectionIDs)
                    {
                        var section = _unitOfWork.SectionRepository.Get(sectionId);
                        if (section != null)
                        {
                            meter.Sections.Add(section);
                        }
                    }
                    meter.Levels = new List<Level>();
                    foreach (var levelId in tree3D.LevelIDs)
                    {
                        var level = _unitOfWork.LevelRepository.Get(levelId);
                        if (level != null)
                        {
                            meter.Levels.Add(level);
                        }
                    }
                    meter.Spaces = new List<Space>();
                    foreach (var spaceId in tree3D.SpaceIDs)
                    {
                        var space = _unitOfWork.SpaceRepository.Get(spaceId);
                        if (space != null)
                        {
                            meter.Spaces.Add(space);
                        }
                    }

                    #endregion
                }
                try
                {
                    _unitOfWork.MeterRepository.Add(meter);
                    _unitOfWork.Save();
                    TempData["message"] = $"Meter {meter.Code} has been created. Add initial index.";
                    return Json(meter.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateDistributionModesDropDownList(meter.DistributionModeID);
            PopulateClientsDropDownList(meter.ClientID);
            return View(meter);
        }

        // GET: Meter/Edit/5
        public ActionResult Edit(int id)
        {
            var meter = _unitOfWork.MeterRepository.Get(id);
            if (meter == null)
            {
                return HttpNotFound();
            }
            PopulateDistributionModesDropDownList(meter.DistributionModeID);
            PopulateClientsDropDownList(meter.ClientID);
            return View(meter);
        }

        // POST: Meter/Edit/5
        [HttpPost]
        public ActionResult Edit(Meter meter)
        {
            var meterToUpdate = _unitOfWork.MeterRepository.GetMeterIncludingMeterTypesAndDistributionModeAndClientAndSectionsAndLevelsAndSpaces(meter.ID);
            if (meterToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(meterToUpdate, "", new[] { "Code", "Details", "Defect", "DistributionModeID", "ClientID" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateMeter = _unitOfWork.MeterRepository.FirstOrDefault(m => m.Code == meterToUpdate.Code);
                    if (duplicateMeter != null && duplicateMeter.ID != meterToUpdate.ID)
                    {
                        PopulateDistributionModesDropDownList(meter.DistributionModeID);
                        PopulateClientsDropDownList(meter.ClientID);
                        return new HttpStatusCodeResult(409, "A meter with this code already exists.");
                    }

                    #region Update MeterTypes

                    var tree1D = Utils.TreeHelper.MapSelectedIDsToDatabaseIDsForMeterTypes(meter.MeterTypesSelected);
                    meterToUpdate.MeterTypes.Clear();
                    foreach (var meterTypeId in tree1D.MeterTypeIDs)
                    {
                        var meterType = _unitOfWork.MeterTypeRepository.Get(meterTypeId);
                        if (meterType != null)
                        {
                            meterToUpdate.MeterTypes.Add(meterType);
                        }
                    }

                    #endregion

                    #region Update Sections, Levels and Spaces

                    var tree3D = Utils.TreeHelper.MapSelectedIDsToDatabaseIDsForSpaces(meter.MeterSLSSelected);
                    meterToUpdate.Sections.Clear();
                    foreach (var sectionId in tree3D.SectionIDs)
                    {
                        var section = _unitOfWork.SectionRepository.Get(sectionId);
                        if (section != null)
                        {
                            meterToUpdate.Sections.Add(section);
                        }
                    }
                    meterToUpdate.Levels.Clear();
                    foreach (var levelId in tree3D.LevelIDs)
                    {
                        var level = _unitOfWork.LevelRepository.Get(levelId);
                        if (level != null)
                        {
                            meterToUpdate.Levels.Add(level);
                        }
                    }
                    meterToUpdate.Spaces.Clear();
                    foreach (var spaceId in tree3D.SpaceIDs)
                    {
                        var space = _unitOfWork.SpaceRepository.Get(spaceId);
                        if (space != null)
                        {
                            meterToUpdate.Spaces.Add(space);
                        }
                    }

                    #endregion

                    _unitOfWork.Save();
                    TempData["message"] = $"Meter {meterToUpdate.Code} has been edited.";
                    return Json(meterToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateDistributionModesDropDownList(meterToUpdate.DistributionModeID);
            PopulateClientsDropDownList(meterToUpdate.ClientID);
            return View(meterToUpdate);
        }

        // GET: Meter/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var meter = _unitOfWork.MeterRepository.GetMeterIncludingMeterTypesAndDistributionModeAndClientAndSectionsAndLevelsAndSpaces(id);
            if (meter == null)
            {
                return HttpNotFound();
            }
            return View(meter);
        }

        // POST: Meter/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var meter = _unitOfWork.MeterRepository.Get(id);
                if (meter == null)
                {
                    return HttpNotFound();
                }
                _unitOfWork.MeterRepository.Remove(meter);
                _unitOfWork.Save();
                TempData["message"] = $"Meter {meter.Code} has been deleted.";
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

        private void PopulateDistributionModesDropDownList(object selectedDistributionMode = null)
        {
            var distributionModesQuery = _unitOfWork.DistributionModeRepository.GetAll().ToList();
            ViewBag.DistributionModeID = new SelectList(distributionModesQuery, "ID", "Mode", selectedDistributionMode);
        }

        private void PopulateClientsDropDownList(object selectedClient = null)
        {
            var clientsQuery = _unitOfWork.ClientRepository.GetAll().ToList();
            ViewBag.ClientID = new SelectList(clientsQuery, "ID", "Name", selectedClient);
        }

        [HttpGet]
        public string GetMeterTypesTreeData(int? meterId)
        {
            var root = new TreeNode
            {
                id = "root",
                children = {},
                text = "-",
                state = new TreeNodeState {opened = true}
            };

            var selectedMeterTypes = new HashSet<int>();
            if (meterId != null && meterId != 0)
            {
                var meter = _unitOfWork.MeterRepository.Get((int)meterId);
                if (meter != null)
                {
                    selectedMeterTypes = new HashSet<int>(meter.MeterTypes.Select(mt => mt.ID));
                }
            }

            var meterTypes = _unitOfWork.MeterTypeRepository.GetAll().ToList();
            if (meterTypes.Any())
            {
                foreach (var meterType in meterTypes)
                {
                    var meterTypeNode = new TreeNode();
                    meterTypeNode.id = meterType.ID.ToString();
                    meterTypeNode.text = meterType.Type;
                    if (selectedMeterTypes.Count > 0 && selectedMeterTypes.Contains(meterType.ID))
                    {
                        meterTypeNode.state = new TreeNodeState {selected = true};
                    }
                    root.children.Add(meterTypeNode);
                }
            }

            return new JavaScriptSerializer().Serialize(root);
        }

        [HttpGet]
        public string GetSpacesTreeData(int? meterId, int? clientId)
        {
            var root = new TreeNode
            {
                id = "root",
                children = {},
                text = "-",
                state = new TreeNodeState {opened = true}
            };

            var selectedSpacesIDs = new HashSet<int>();
            var selectedLevelsIDs = new HashSet<int>();
            var selectedSectionsIDs = new HashSet<int>();
            if (meterId != null && meterId != 0)
            {
                var meter = _unitOfWork.MeterRepository.Get((int)meterId);
                if (meter != null)
                {
                    selectedSpacesIDs = new HashSet<int>(meter.Spaces.Select(s => s.ID));
                    selectedLevelsIDs = new HashSet<int>(meter.Levels.Select(l => l.ID));
                    selectedSectionsIDs = new HashSet<int>(meter.Sections.Select(s => s.ID));
                }
            }

            if (clientId != null && clientId != 0)
            {
                var client = _unitOfWork.ClientRepository.Get((int)clientId);
                if (client != null)
                {
                    var treeHelper = new Utils.TreeHelper(_unitOfWork);
                    root = treeHelper.GetSectionsLevelsSpacesByClient(root, client.ID, selectedSectionsIDs, selectedLevelsIDs, selectedSpacesIDs);
                }
            }

            return new JavaScriptSerializer().Serialize(root);
        }
    }
}
