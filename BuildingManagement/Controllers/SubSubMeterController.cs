using System;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using X.PagedList;

namespace BuildingManagement.Controllers
{
    public class SubSubMeterController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubSubMeterController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: SubSubMeter
        public ActionResult Index(int? page, string currentFilter, string searchString, string sortOrder)
        {
            IEnumerable<SubSubMeter> subSubMeters;
            var pageNumber = page ?? 1;
            const int pageSize = 3;
            if (searchString != null)
            {
                pageNumber = 1;
                subSubMeters = _unitOfWork.SubSubMeterRepository.GetFilteredSubSubMetersIncludingMeterTypesAndDistributionModeAndSubMeterAndSectionsAndLevelsAndSpaces(searchString).ToList();
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    subSubMeters = _unitOfWork.SubSubMeterRepository.GetFilteredSubSubMetersIncludingMeterTypesAndDistributionModeAndSubMeterAndSectionsAndLevelsAndSpaces(searchString).ToList();
                }
                else
                {
                    subSubMeters = _unitOfWork.SubSubMeterRepository.GetAllSubSubMetersIncludingMeterTypesAndDistributionModeAndSubMeterAndSectionsAndLevelsAndSpaces().ToList();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CodeSortParm = string.IsNullOrEmpty(sortOrder) ? "code_desc" : "";
            ViewBag.DetailsSortParm = sortOrder == "Details" ? "details_desc" : "Details";
            ViewBag.DefectSortParm = sortOrder == "Defect" ? "defect_desc" : "Defect";
            ViewBag.DistributionModeSortParm = sortOrder == "DistributionMode" ? "distributionMode_desc" : "DistributionMode";
            ViewBag.SubMeterSortParm = sortOrder == "SubMeter" ? "SubMeter_desc" : "SubMeter";
            subSubMeters = _unitOfWork.SubSubMeterRepository.OrderSubSubMeters(subSubMeters, sortOrder).ToList();
            ViewBag.OnePageOfSubSubMeters = subSubMeters.ToPagedList(pageNumber, pageSize);
            return View(ViewBag.OnePageOfSubSubMeters);
        }

        // GET: SubSubMeter/Details/5
        public ActionResult Details(int id)
        {
            var subSubMeter = _unitOfWork.SubSubMeterRepository.Get(id);
            if (subSubMeter == null)
            {
                return HttpNotFound();
            }
            return View(subSubMeter);
        }

        // GET: SubMeter/Create
        public ActionResult Create()
        {
            var subSubMeter = new SubSubMeter();
            PopulateSubMetersDropDownList();
            PopulateDistributionModesDropDownList();
            return View(subSubMeter);
        }

        // POST: SubSubMeter/Create
        [HttpPost]
        public ActionResult Create(SubSubMeter subSubMeter)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                if (subSubMeter.Code != null)
                {
                    var duplicateSubSubMeter = _unitOfWork.SubMeterRepository.FirstOrDefault(ssm => ssm.Code == subSubMeter.Code);
                    if (duplicateSubSubMeter != null)
                    {
                        PopulateSubMetersDropDownList(subSubMeter.SubMeterID);
                        PopulateDistributionModesDropDownList(subSubMeter.DistributionModeID);
                        return new HttpStatusCodeResult(409, "A sub sub meter with this code already exists.");
                    }
                }
                var subMeter = _unitOfWork.SubMeterRepository.Get(subSubMeter.SubMeterID);
                if (subMeter != null)
                {
                    subSubMeter.SubMeter = subMeter;
                }
                var types = string.Empty;
                if (subSubMeter.MeterTypesSelected != null)
                {
                    subSubMeter.MeterTypes = new List<MeterType>();
                    foreach (var subMeterTypeSelected in subSubMeter.MeterTypesSelected)
                    {
                        if (subMeterTypeSelected != "root")
                        {
                            var meterType = _unitOfWork.MeterTypeRepository.Get(int.Parse(subMeterTypeSelected));
                            if (meterType != null)
                            {
                                subSubMeter.MeterTypes.Add(meterType);
                                types += meterType.Type + ", ";
                            }
                        }
                    }
                    types = types.Remove(types.Length - 2, 2);
                }
                if (subSubMeter.MeterSLSSelected != null)
                {
                    #region Add Sections, Levels and Spaces

                    var tree3D = Utils.TreeHelper.MapSelectedIDsToDatabaseIDsForSpaces(subSubMeter.MeterSLSSelected);
                    subSubMeter.Sections = new List<Section>();
                    foreach (var sectionId in tree3D.SectionIDs)
                    {
                        var section = _unitOfWork.SectionRepository.Get(sectionId);
                        if (section != null)
                        {
                            subSubMeter.Sections.Add(section);
                        }
                    }
                    subSubMeter.Levels = new List<Level>();
                    foreach (var levelId in tree3D.LevelIDs)
                    {
                        var level = _unitOfWork.LevelRepository.Get(levelId);
                        if (level != null)
                        {
                            subSubMeter.Levels.Add(level);
                        }
                    }
                    subSubMeter.Spaces = new List<Space>();
                    foreach (var spaceId in tree3D.SpaceIDs)
                    {
                        var space = _unitOfWork.SpaceRepository.Get(spaceId);
                        if (space != null)
                        {
                            subSubMeter.Spaces.Add(space);
                        }
                    }

                    #endregion
                }
                try
                {
                    _unitOfWork.SubSubMeterRepository.Add(subSubMeter);
                    _unitOfWork.Save();
                    TempData["message"] = $"SubSubMeter {subSubMeter.Code} has been created. Add initial index for {types}.";
                    return Json(subSubMeter.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateSubMetersDropDownList(subSubMeter.SubMeterID);
            PopulateDistributionModesDropDownList(subSubMeter.DistributionModeID);
            return View(subSubMeter);
        }

        // GET: SubSubMeter/Edit/5
        public ActionResult Edit(int id)
        {
            var subSubMeter = _unitOfWork.SubSubMeterRepository.Get(id);
            if (subSubMeter == null)
            {
                return HttpNotFound();
            }
            PopulateSubMetersDropDownList(subSubMeter.SubMeterID);
            PopulateDistributionModesDropDownList(subSubMeter.DistributionModeID);
            return View(subSubMeter);
        }

        // POST: SubSubMeter/Edit/5
        [HttpPost]
        public ActionResult Edit(SubSubMeter subSubMeter)
        {
            var subSubMeterToUpdate = _unitOfWork.SubSubMeterRepository.GetSubSubMeterIncludingMeterTypesAndDistributionModeAndSubMeterAndSectionsAndLevelsAndSpaces(subSubMeter.ID);
            if (subSubMeterToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(subSubMeterToUpdate, "", new[] { "Code", "Details", "Defect", "SubMeterID", "DistributionModeID" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateSubSubMeter = _unitOfWork.SubSubMeterRepository.FirstOrDefault(ssm => ssm.ID != subSubMeterToUpdate.ID && ssm.Code == subSubMeterToUpdate.Code);
                    if (duplicateSubSubMeter != null)
                    {
                        PopulateSubMetersDropDownList(subSubMeterToUpdate.SubMeterID);
                        PopulateDistributionModesDropDownList(subSubMeterToUpdate.DistributionModeID);
                        return new HttpStatusCodeResult(409, "A sub sub meter with this code already exists.");
                    }

                    #region Update MeterTypes

                    var tree1D = Utils.TreeHelper.MapSelectedIDsToDatabaseIDsForMeterTypes(subSubMeter.MeterTypesSelected);
                    subSubMeterToUpdate.MeterTypes.Clear();
                    foreach (var meterTypeId in tree1D.MeterTypeIDs)
                    {
                        var meterType = _unitOfWork.MeterTypeRepository.Get(meterTypeId);
                        if (meterType != null)
                        {
                            subSubMeterToUpdate.MeterTypes.Add(meterType);
                        }
                    }

                    #endregion

                    #region Update Sections, Levels and Spaces

                    var tree3D = Utils.TreeHelper.MapSelectedIDsToDatabaseIDsForSpaces(subSubMeter.MeterSLSSelected);
                    subSubMeterToUpdate.Sections.Clear();
                    foreach (var sectionId in tree3D.SectionIDs)
                    {
                        var section = _unitOfWork.SectionRepository.Get(sectionId);
                        if (section != null)
                        {
                            subSubMeterToUpdate.Sections.Add(section);
                        }
                    }
                    subSubMeterToUpdate.Levels.Clear();
                    foreach (var levelId in tree3D.LevelIDs)
                    {
                        var level = _unitOfWork.LevelRepository.Get(levelId);
                        if (level != null)
                        {
                            subSubMeterToUpdate.Levels.Add(level);
                        }
                    }
                    subSubMeterToUpdate.Spaces.Clear();
                    foreach (var spaceId in tree3D.SpaceIDs)
                    {
                        var space = _unitOfWork.SpaceRepository.Get(spaceId);
                        if (space != null)
                        {
                            subSubMeterToUpdate.Spaces.Add(space);
                        }
                    }

                    #endregion

                    _unitOfWork.Save();
                    TempData["message"] = $"SubSubMeter {subSubMeterToUpdate.Code} has been edited.";
                    return Json(subSubMeterToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateSubMetersDropDownList(subSubMeterToUpdate.SubMeterID);
            PopulateDistributionModesDropDownList(subSubMeterToUpdate.DistributionModeID);
            return View(subSubMeterToUpdate);
        }

        // GET: SubSubMeter/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var subSubMeter = _unitOfWork.SubSubMeterRepository.GetSubSubMeterIncludingMeterTypesAndDistributionModeAndSubMeterAndSectionsAndLevelsAndSpaces(id);
            if (subSubMeter == null)
            {
                return HttpNotFound();
            }
            return View(subSubMeter);
        }

        // POST: SubSubMeter/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var subSubMeter = _unitOfWork.SubSubMeterRepository.Get(id);
                if (subSubMeter == null)
                {
                    return HttpNotFound();
                }
                _unitOfWork.SubSubMeterRepository.Remove(subSubMeter);
                _unitOfWork.Save();
                TempData["message"] = $"SubSubMeter {subSubMeter.Code} has been deleted.";
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

        private void PopulateDistributionModesDropDownList(object selectedDistributionMode = null)
        {
            var distributionModesQuery = from DistributionMode d in Enum.GetValues(typeof(DistributionMode)) select new { ID = (int)d, Name = d.ToString() };
            ViewBag.DistributionModeID = new SelectList(distributionModesQuery, "ID", "Name", selectedDistributionMode);
        }

        private void PopulateSubMetersDropDownList(object selectedSubMeter = null)
        {
            var subMetersQuery = _unitOfWork.SubMeterRepository.GetAll().ToList();
            ViewBag.SubMeterID = new SelectList(subMetersQuery, "ID", "Code", selectedSubMeter);
        }

        [HttpGet]
        public string GetMeterTypesTreeData(int? subSubMeterId, int? subMeterId)
        {
            var root = new TreeNode
            {
                id = "root",
                children = { },
                text = "-",
                state = new TreeNodeState { opened = true }
            };

            var selectedMeterTypes = new HashSet<int>();
            if (subSubMeterId != null && subSubMeterId != 0)
            {
                var subSubMeter = _unitOfWork.SubSubMeterRepository.Get((int)subSubMeterId);
                if (subSubMeter != null)
                {
                    selectedMeterTypes = new HashSet<int>(subSubMeter.MeterTypes.Select(mt => mt.ID));
                }
            }

            var meterTypes = new List<MeterType>();
            if (subMeterId != null && subSubMeterId != 0)
            {
                var meter = _unitOfWork.SubMeterRepository.Get((int)subMeterId);
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
                        meterTypeNode.state = new TreeNodeState { selected = true };
                    }
                    root.children.Add(meterTypeNode);
                }
            }

            return new JavaScriptSerializer().Serialize(root);
        }

        [HttpGet]
        public string GetSpacesTreeData(int? subSubMeterId, int? subMeterId)
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
            if (subSubMeterId != null && subSubMeterId != 0)
            {
                var subSubMeter = _unitOfWork.SubSubMeterRepository.Get((int)subSubMeterId);
                if (subSubMeter != null)
                {
                    selectedSpacesIDs = new HashSet<int>(subSubMeter.Spaces.Select(s => s.ID));
                    selectedLevelsIDs = new HashSet<int>(subSubMeter.Levels.Select(l => l.ID));
                    selectedSectionsIDs = new HashSet<int>(subSubMeter.Sections.Select(s => s.ID));
                }
            }

            if (subMeterId != null && subMeterId != 0)
            {
                var subMeter = _unitOfWork.SubMeterRepository.Get((int)subMeterId);
                if (subMeter != null)
                {
                    var meter = _unitOfWork.MeterRepository.Get(subMeter.MeterID);
                    if (meter != null)
                    {
                        var treeHelper = new Utils.TreeHelper(_unitOfWork);
                        root = treeHelper.GetSectionsLevelsSpacesByParent(root, meter.ClientID, subMeter.Sections, subMeter.Levels, subMeter.Spaces, selectedSectionsIDs, selectedLevelsIDs, selectedSpacesIDs);
                    }
                }
            }

            return new JavaScriptSerializer().Serialize(root);
        }
    }
}