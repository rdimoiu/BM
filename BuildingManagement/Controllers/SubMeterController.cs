using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using System.Web.Script.Serialization;
using X.PagedList;

namespace BuildingManagement.Controllers
{
    public class SubMeterController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubMeterController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: SubMeter
        public ActionResult Index(int? page, string currentFilter, string searchString, string sortOrder)
        {
            IEnumerable<SubMeter> subMeters;
            var pageNumber = page ?? 1;
            const int pageSize = 3;
            if (searchString != null)
            {
                pageNumber = 1;
                subMeters = _unitOfWork.SubMeterRepository.GetFilteredSubMetersIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces(searchString, sortOrder).ToList();
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    subMeters = _unitOfWork.SubMeterRepository.GetFilteredSubMetersIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces(searchString, sortOrder).ToList();
                }
                else
                {
                    subMeters = _unitOfWork.SubMeterRepository.GetAllSubMetersIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces(sortOrder).ToList();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CodeSortParm = string.IsNullOrEmpty(sortOrder) ? "code_desc" : "";
            ViewBag.DetailsSortParm = sortOrder == "Details" ? "details_desc" : "Details";
            ViewBag.InitialIndexSortParm = sortOrder == "InitialIndex" ? "initialIndex_desc" : "InitialIndex";
            ViewBag.DefectSortParm = sortOrder == "Defect" ? "defect_desc" : "Defect";
            ViewBag.DistributionModeSortParm = sortOrder == "DistributionMode" ? "distributionMode_desc" : "DistributionMode";
            ViewBag.MeterSortParm = sortOrder == "Meter" ? "meter_desc" : "Meter";
            subMeters = _unitOfWork.SubMeterRepository.OrderSubMeters(subMeters, sortOrder);
            ViewBag.OnePageOfSubMeters = subMeters.ToPagedList(pageNumber, pageSize);
            return View(ViewBag.OnePageOfSubMeters);
        }

        // GET: SubMeter/Details/5
        public ActionResult Details(int id)
        {
            var subMeter = _unitOfWork.SubMeterRepository.Get(id);
            if (subMeter == null)
            {
                return HttpNotFound();
            }
            return View(subMeter);
        }

        // GET: SubMeter/Create
        public ActionResult Create()
        {
            var model = new SubMeter();
            PopulateMetersDropDownList();
            PopulateDistributionModesDropDownList();
            return View(model);
        }

        // POST: SubMeter/Create
        [HttpPost]
        public ActionResult CreateSubMeter(SubMeter subMeter)
        {
            //uniqueness condition check
            if (subMeter.Code != null)
            {
                var duplicateSubMeter = _unitOfWork.SubMeterRepository.SingleOrDefault(sm => sm.Code == subMeter.Code);
                if (duplicateSubMeter != null)
                {
                    ModelState.AddModelError("Code", "A sub meter with this code already exists.");
                    PopulateMetersDropDownList(subMeter.MeterID);
                    PopulateDistributionModesDropDownList(subMeter.DistributionModeID);
                    return View("Create", subMeter);
                }
            }
            var meter = _unitOfWork.MeterRepository.Get(subMeter.MeterID);
            if (meter != null)
            {
                subMeter.Meter = meter;
            }
            var distributionMode = _unitOfWork.DistributionModeRepository.Get(subMeter.DistributionModeID);
            if (distributionMode != null)
            {
                subMeter.DistributionMode = distributionMode;
            }
            subMeter.MeterTypes = new List<MeterType>();
            if (subMeter.MeterTypesSelected != null)
            {
                foreach (var subMeterTypeSelected in subMeter.MeterTypesSelected)
                {
                    if (subMeterTypeSelected != "root")
                    {
                        var meterType = _unitOfWork.MeterTypeRepository.Get(int.Parse(subMeterTypeSelected));
                        if (meterType != null)
                        {
                            subMeter.MeterTypes.Add(meterType);
                        }
                    }
                }
            }
            if (subMeter.MeterSLSSelected != null)
            {
                #region Add Sections, Levels and Spaces

                var tree3D = Utils.TreeHelper.MapSelectedIDsToDatabaseIDsForSpaces(subMeter.MeterSLSSelected);
                subMeter.Sections = new List<Section>();
                foreach (var sectionId in tree3D.SectionIDs)
                {
                    var section = _unitOfWork.SectionRepository.Get(sectionId);
                    if (section != null)
                    {
                        subMeter.Sections.Add(section);
                    }
                }
                subMeter.Levels = new List<Level>();
                foreach (var levelId in tree3D.LevelIDs)
                {
                    var level = _unitOfWork.LevelRepository.Get(levelId);
                    if (level != null)
                    {
                        subMeter.Levels.Add(level);
                    }
                }
                subMeter.Spaces = new List<Space>();
                foreach (var spaceId in tree3D.SpaceIDs)
                {
                    var space = _unitOfWork.SpaceRepository.Get(spaceId);
                    if (space != null)
                    {
                        subMeter.Spaces.Add(space);
                    }
                }

                #endregion
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.SubMeterRepository.Add(subMeter);
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("SubMeter {0} has been created.", subMeter.Code);
                    return Json(subMeter.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateMetersDropDownList(subMeter.MeterID);
            PopulateDistributionModesDropDownList(subMeter.DistributionModeID);
            return View("Create", subMeter);
        }

        // GET: SubMeter/Edit/5
        public ActionResult Edit(int id)
        {
            var subMeter = _unitOfWork.SubMeterRepository.Get(id);
            if (subMeter == null)
            {
                return HttpNotFound();
            }
            PopulateMetersDropDownList(subMeter.MeterID);
            PopulateDistributionModesDropDownList(subMeter.DistributionModeID);
            return View(subMeter);
        }

        // POST: SubMeter/Edit/5
        [HttpPost]
        public ActionResult EditSubMeter(SubMeter subMeter)
        {
            var subMeterToUpdate = _unitOfWork.SubMeterRepository.GetSubMeterIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces(subMeter.ID);
            if (subMeterToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(subMeterToUpdate, "", new[] { "Code", "Details", "InitialIndex", "Defect", "MeterID", "DistributionModeID" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateSubMeter = _unitOfWork.SubMeterRepository.SingleOrDefault(sm => sm.Code == subMeterToUpdate.Code);
                    if (duplicateSubMeter != null && duplicateSubMeter.ID != subMeterToUpdate.ID)
                    {
                        ModelState.AddModelError("Code", "A sub meter with this code already exists.");
                        PopulateMetersDropDownList(subMeterToUpdate.MeterID);
                        PopulateDistributionModesDropDownList(subMeterToUpdate.DistributionModeID);
                        return View("Edit", subMeter);
                    }

                    #region Update MeterTypes

                    var tree1D = Utils.TreeHelper.MapSelectedIDsToDatabaseIDsForMeterTypes(subMeter.MeterTypesSelected);
                    subMeterToUpdate.MeterTypes.Clear();
                    foreach (var meterTypeId in tree1D.MeterTypeIDs)
                    {
                        var meterType = _unitOfWork.MeterTypeRepository.Get(meterTypeId);
                        if (meterType != null)
                        {
                            subMeterToUpdate.MeterTypes.Add(meterType);
                        }
                    }

                    #endregion

                    #region Update Sections, Levels and Spaces

                    var tree3D = Utils.TreeHelper.MapSelectedIDsToDatabaseIDsForSpaces(subMeter.MeterSLSSelected);
                    subMeterToUpdate.Sections.Clear();
                    foreach (var sectionId in tree3D.SectionIDs)
                    {
                        var section = _unitOfWork.SectionRepository.Get(sectionId);
                        if (section != null)
                        {
                            subMeterToUpdate.Sections.Add(section);
                        }
                    }
                    subMeterToUpdate.Levels.Clear();
                    foreach (var levelId in tree3D.LevelIDs)
                    {
                        var level = _unitOfWork.LevelRepository.Get(levelId);
                        if (level != null)
                        {
                            subMeterToUpdate.Levels.Add(level);
                        }
                    }
                    subMeterToUpdate.Spaces.Clear();
                    foreach (var spaceId in tree3D.SpaceIDs)
                    {
                        var space = _unitOfWork.SpaceRepository.Get(spaceId);
                        if (space != null)
                        {
                            subMeterToUpdate.Spaces.Add(space);
                        }
                    }

                    #endregion

                    _unitOfWork.Save();
                    TempData["message"] = string.Format("SubMeter {0} has been edited.", subMeterToUpdate.Code);
                    return Json(subMeterToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateMetersDropDownList(subMeterToUpdate.MeterID);
            PopulateDistributionModesDropDownList(subMeterToUpdate.DistributionModeID);
            return View("Edit", subMeterToUpdate);
        }

        // GET: SubMeter/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var subMeter = _unitOfWork.SubMeterRepository.GetSubMeterIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces(id);
            if (subMeter == null)
            {
                return HttpNotFound();
            }
            return View(subMeter);
        }

        // POST: SubMeter/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var subMeter = _unitOfWork.SubMeterRepository.Get(id);
                if (subMeter == null)
                {
                    return HttpNotFound();
                }
                _unitOfWork.SubMeterRepository.Remove(subMeter);
                _unitOfWork.Save();
                TempData["message"] = string.Format("SubMeter {0} has been deleted.", subMeter.Code);
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
            var distributionModesQuery = from dm in _unitOfWork.DistributionModeRepository.GetAll() select dm;
            ViewBag.DistributionModeID = new SelectList(distributionModesQuery, "ID", "Mode", selectedDistributionMode);
        }

        private void PopulateMetersDropDownList(object selectedMeter = null)
        {
            var metersQuery = from m in _unitOfWork.MeterRepository.GetAll() select m;
            ViewBag.MeterID = new SelectList(metersQuery, "ID", "Code", selectedMeter);
        }

        [HttpGet]
        public string GetMeterTypesTreeData(int? subMeterId, int? meterId)
        {
            var root = new TreeNode
            {
                id = "root",
                children = {},
                text = "-",
                state = new TreeNodeState {opened = true}
            };

            var selectedMeterTypes = new HashSet<int>();
            if (subMeterId != null && subMeterId != 0)
            {
                var subMeter = _unitOfWork.SubMeterRepository.GetSubMeterIncludingMeterTypes((int)subMeterId);
                if (subMeter != null)
                {
                    selectedMeterTypes = new HashSet<int>(subMeter.MeterTypes.Select(mt => mt.ID));
                }
            }

            var meterTypes = new List<MeterType>();
            if (meterId != null && subMeterId != 0)
            {
                var meter = _unitOfWork.MeterRepository.GetMeterIncludingMeterTypes((int)meterId);
                if (meter != null)
                {
                    meterTypes = meter.MeterTypes.ToList();
                }
            }
            
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
        public string GetSpacesTreeData(int? subMeterId, int? meterId)
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
            if (subMeterId != null && subMeterId != 0)
            {
                var subMeter = _unitOfWork.SubMeterRepository.GetSubMeterIncludingSectionsAndLevelsAndSpaces((int)subMeterId);
                if (subMeter != null)
                {
                    selectedSpacesIDs = new HashSet<int>(subMeter.Spaces.Select(s => s.ID));
                    selectedLevelsIDs = new HashSet<int>(subMeter.Levels.Select(l => l.ID));
                    selectedSectionsIDs = new HashSet<int>(subMeter.Sections.Select(s => s.ID));
                }
            }

            if (meterId != null && meterId != 0)
            {
                var meter = _unitOfWork.MeterRepository.GetMeterIncludingSectionsAndLevelsAndSpaces((int)meterId);
                if (meter != null)
                {
                    var treeHelper = new Utils.TreeHelper(_unitOfWork);
                    root = treeHelper.GetSectionsLevelsSpacesByParent(root, meter.ClientID, meter.Sections, meter.Levels, meter.Spaces, selectedSectionsIDs, selectedLevelsIDs, selectedSpacesIDs);
                }
            }

            return new JavaScriptSerializer().Serialize(root);
        }
    }
}
