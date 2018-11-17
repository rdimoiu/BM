﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using BuildingManagement.DAL;
using BuildingManagement.Models;
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
                subSubMeters = _unitOfWork.SubSubMeterRepository.GetFilteredSubSubMetersIncludingMeterTypesAndDistributionModeAndSubMeterAndSectionsAndLevelsAndSpaces(searchString);
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    subSubMeters = _unitOfWork.SubSubMeterRepository.GetFilteredSubSubMetersIncludingMeterTypesAndDistributionModeAndSubMeterAndSectionsAndLevelsAndSpaces(searchString);
                }
                else
                {
                    subSubMeters = _unitOfWork.SubSubMeterRepository.GetAllSubSubMetersIncludingMeterTypesAndDistributionModeAndSubMeterAndSectionsAndLevelsAndSpaces();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CodeSortParm = string.IsNullOrEmpty(sortOrder) ? "code_desc" : "";
            ViewBag.DetailsSortParm = sortOrder == "Details" ? "details_desc" : "Details";
            ViewBag.InitialIndexSortParm = sortOrder == "InitialIndex" ? "initialIndex_desc" : "InitialIndex";
            ViewBag.DefectSortParm = sortOrder == "Defect" ? "defect_desc" : "Defect";
            ViewBag.DistributionModeSortParm = sortOrder == "DistributionMode" ? "distributionMode_desc" : "DistributionMode";
            ViewBag.SubMeterSortParm = sortOrder == "SubMeter" ? "SubMeter_desc" : "SubMeter";
            subSubMeters = _unitOfWork.SubSubMeterRepository.OrderSubSubMeters(subSubMeters, sortOrder);
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
            var model = new SubSubMeter();
            PopulateSubMetersDropDownList();
            PopulateDistributionModesDropDownList();
            return View(model);
        }

        // POST: SubSubMeter/Create
        [HttpPost]
        public ActionResult CreateSubSubMeter(SubSubMeter subSubMeter)
        {
            //uniqueness condition check
            if (subSubMeter.Code != null)
            {
                var duplicateSubSubMeter = _unitOfWork.SubMeterRepository.SingleOrDefault(ssm => ssm.Code == subSubMeter.Code);
                if (duplicateSubSubMeter != null)
                {
                    ModelState.AddModelError("Code", "A sub sub meter with this code already exists.");
                    PopulateSubMetersDropDownList(subSubMeter.SubMeterID);
                    PopulateDistributionModesDropDownList(subSubMeter.DistributionModeID);
                    return View("Create", subSubMeter);
                }
            }
            var subMeter = _unitOfWork.MeterRepository.Get(subSubMeter.SubMeterID);
            if (subMeter != null)
            {
                subSubMeter.SubMeter = subMeter;
            }
            var distributionMode = _unitOfWork.DistributionModeRepository.Get(subSubMeter.DistributionModeID);
            if (distributionMode != null)
            {
                subSubMeter.DistributionMode = distributionMode;
            }
            subSubMeter.MeterTypes = new List<MeterType>();
            if (subSubMeter.MeterTypesSelected != null)
            {
                foreach (var subMeterTypeSelected in subSubMeter.MeterTypesSelected)
                {
                    if (subMeterTypeSelected != "root")
                    {
                        var meterType = _unitOfWork.MeterTypeRepository.Get(int.Parse(subMeterTypeSelected));
                        if (meterType != null)
                        {
                            subSubMeter.MeterTypes.Add(meterType);
                        }
                    }
                }
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
            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.SubSubMeterRepository.Add(subSubMeter);
                    _unitOfWork.Save();
                    TempData["message"] = string.Format("SubSubMeter {0} has been created.", subSubMeter.Code);
                    return Json(subSubMeter.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateSubMetersDropDownList(subSubMeter.SubMeterID);
            PopulateDistributionModesDropDownList(subSubMeter.DistributionModeID);
            return View("Create", subSubMeter);
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
        public ActionResult EditSubSubMeter(SubSubMeter subSubMeter)
        {
            var subSubMeterToUpdate = _unitOfWork.SubSubMeterRepository.GetSubSubMeterIncludingMeterTypesAndDistributionModeAndSubMeterAndSectionsAndLevelsAndSpaces(subSubMeter.ID);
            if (subSubMeterToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(subSubMeterToUpdate, "", new[] { "Code", "Details", "InitialIndex", "Defect", "SubMeterID", "DistributionModeID" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateSubSubMeter = _unitOfWork.SubSubMeterRepository.SingleOrDefault(ssm => ssm.Code == subSubMeterToUpdate.Code);
                    if (duplicateSubSubMeter != null && duplicateSubSubMeter.ID != subSubMeterToUpdate.ID)
                    {
                        ModelState.AddModelError("Code", "A sub sub meter with this code already exists.");
                        PopulateSubMetersDropDownList(subSubMeterToUpdate.SubMeterID);
                        PopulateDistributionModesDropDownList(subSubMeterToUpdate.DistributionModeID);
                        return View("Edit", subSubMeter);
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
                    TempData["message"] = string.Format("SubSubMeter {0} has been edited.", subSubMeterToUpdate.Code);
                    return Json(subSubMeterToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateSubMetersDropDownList(subSubMeterToUpdate.SubMeterID);
            PopulateDistributionModesDropDownList(subSubMeterToUpdate.DistributionModeID);
            return View("Edit", subSubMeterToUpdate);
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
                TempData["message"] = string.Format("SubSubMeter {0} has been deleted.", subSubMeter.Code);
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
            var distributionModesQuery = from dm in _unitOfWork.DistributionModeRepository.GetAll() select dm;
            ViewBag.DistributionModeID = new SelectList(distributionModesQuery, "ID", "Mode", selectedDistributionMode);
        }

        private void PopulateSubMetersDropDownList(object selectedSubMeter = null)
        {
            var subMetersQuery = from sm in _unitOfWork.SubMeterRepository.GetAll() select sm;
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
                var subSubMeter = _unitOfWork.SubSubMeterRepository.GetSubSubMeterIncludingMeterTypes((int)subSubMeterId);
                if (subSubMeter != null)
                {
                    selectedMeterTypes = new HashSet<int>(subSubMeter.MeterTypes.Select(mt => mt.ID));
                }
            }

            var meterTypes = new List<MeterType>();
            if (subMeterId != null && subSubMeterId != 0)
            {
                var meter = _unitOfWork.SubMeterRepository.GetSubMeterIncludingMeterTypes((int)subMeterId);
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
                var subSubMeter = _unitOfWork.SubSubMeterRepository.GetSubSubMeterIncludingSectionsAndLevelsAndSpaces((int)subSubMeterId);
                if (subSubMeter != null)
                {
                    selectedSpacesIDs = new HashSet<int>(subSubMeter.Spaces.Select(s => s.ID));
                    selectedLevelsIDs = new HashSet<int>(subSubMeter.Levels.Select(l => l.ID));
                    selectedSectionsIDs = new HashSet<int>(subSubMeter.Sections.Select(s => s.ID));
                }
            }

            if (subMeterId != null && subMeterId != 0)
            {
                var subMeter = _unitOfWork.SubMeterRepository.GetSubMeterIncludingSectionsAndLevelsAndSpaces((int)subMeterId);
                if (subMeter != null)
                {
                    var treeHelper = new Utils.TreeHelper(_unitOfWork);
                    root = treeHelper.GetSectionsLevelsSpacesByParent(root, subMeter.MeterID, subMeter.Sections, subMeter.Levels, subMeter.Spaces, selectedSectionsIDs, selectedLevelsIDs, selectedSpacesIDs);
                }
            }

            return new JavaScriptSerializer().Serialize(root);
        }
    }
}