﻿using System;
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
            ViewBag.DefectSortParm = sortOrder == "Defect" ? "defect_desc" : "Defect";
            ViewBag.DistributionModeSortParm = sortOrder == "DistributionMode" ? "distributionMode_desc" : "DistributionMode";
            ViewBag.MeterSortParm = sortOrder == "Meter" ? "meter_desc" : "Meter";
            subMeters = _unitOfWork.SubMeterRepository.OrderSubMeters(subMeters, sortOrder).ToList();
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
            var subMeter = new SubMeter();
            PopulateMetersDropDownList();
            PopulateDistributionModesDropDownList();
            return View(subMeter);
        }

        // POST: SubMeter/Create
        [HttpPost]
        public ActionResult Create(SubMeter subMeter)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                if (subMeter.Code != null)
                {
                    var duplicateSubMeter = _unitOfWork.SubMeterRepository.FirstOrDefault(sm => sm.Code == subMeter.Code);
                    if (duplicateSubMeter != null)
                    {
                        PopulateMetersDropDownList(subMeter.MeterID);
                        PopulateDistributionModesDropDownList(subMeter.DistributionModeID);
                        return new HttpStatusCodeResult(409, "A submeter with this code already exists.");
                    }
                }
                var meter = _unitOfWork.MeterRepository.Get(subMeter.MeterID);
                if (meter != null)
                {
                    subMeter.Meter = meter;
                }
                var types = string.Empty;
                if (subMeter.MeterTypesSelected != null)
                {
                    subMeter.MeterTypes = new List<MeterType>();
                    foreach (var subMeterTypeSelected in subMeter.MeterTypesSelected)
                    {
                        if (subMeterTypeSelected != "root")
                        {
                            var meterType = _unitOfWork.MeterTypeRepository.Get(int.Parse(subMeterTypeSelected));
                            if (meterType != null)
                            {
                                subMeter.MeterTypes.Add(meterType);
                                types += meterType.Type + ", ";
                            }
                        }
                    }
                    types = types.Remove(types.Length - 2, 2);
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
                try
                {
                    _unitOfWork.SubMeterRepository.Add(subMeter);
                    _unitOfWork.Save();
                    TempData["message"] = $"SubMeter {subMeter.Code} has been created. Add initial index for {types}.";
                    return Json(subMeter.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateMetersDropDownList(subMeter.MeterID);
            PopulateDistributionModesDropDownList(subMeter.DistributionModeID);
            return View(subMeter);
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
        public ActionResult Edit(SubMeter subMeter)
        {
            var subMeterToUpdate = _unitOfWork.SubMeterRepository.GetSubMeterIncludingMeterTypesAndDistributionModeAndMeterAndSectionsAndLevelsAndSpaces(subMeter.ID);
            if (subMeterToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(subMeterToUpdate, "", new[] { "Code", "Details", "Defect", "MeterID", "DistributionModeID" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateSubMeter = _unitOfWork.SubMeterRepository.FirstOrDefault(sm => sm.ID != subMeterToUpdate.ID && sm.Code == subMeterToUpdate.Code);
                    if (duplicateSubMeter != null)
                    {
                        PopulateMetersDropDownList(subMeterToUpdate.MeterID);
                        PopulateDistributionModesDropDownList(subMeterToUpdate.DistributionModeID);
                        return new HttpStatusCodeResult(409, "A submeter with this code already exists.");
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
                    TempData["message"] = $"SubMeter {subMeterToUpdate.Code} has been edited.";
                    return Json(subMeterToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateMetersDropDownList(subMeterToUpdate.MeterID);
            PopulateDistributionModesDropDownList(subMeterToUpdate.DistributionModeID);
            return View(subMeterToUpdate);
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
                TempData["message"] = $"SubMeter {subMeter.Code} has been deleted.";
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

        private void PopulateMetersDropDownList(object selectedMeter = null)
        {
            var metersQuery = _unitOfWork.MeterRepository.GetAll().ToList();
            ViewBag.MeterID = new SelectList(metersQuery, "ID", "Code", selectedMeter);
        }

        [HttpGet]
        public string GetMeterTypesTreeData(int? subMeterId, int? meterId)
        {
            var root = new TreeNode
            {
                id = "root",
                children = { },
                text = "-",
                state = new TreeNodeState { opened = true }
            };

            var selectedMeterTypes = new HashSet<int>();
            if (subMeterId != null && subMeterId != 0)
            {
                var subMeter = _unitOfWork.SubMeterRepository.Get((int)subMeterId);
                if (subMeter != null)
                {
                    selectedMeterTypes = new HashSet<int>(subMeter.MeterTypes.Select(mt => mt.ID));
                }
            }

            var meterTypes = new List<MeterType>();
            if (meterId != null && subMeterId != 0)
            {
                var meter = _unitOfWork.MeterRepository.Get((int)meterId);
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
        public string GetSpacesTreeData(int? subMeterId, int? meterId)
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
            if (subMeterId != null && subMeterId != 0)
            {
                var subMeter = _unitOfWork.SubMeterRepository.Get((int)subMeterId);
                if (subMeter != null)
                {
                    selectedSpacesIDs = new HashSet<int>(subMeter.Spaces.Select(s => s.ID));
                    selectedLevelsIDs = new HashSet<int>(subMeter.Levels.Select(l => l.ID));
                    selectedSectionsIDs = new HashSet<int>(subMeter.Sections.Select(s => s.ID));
                }
            }

            if (meterId != null && meterId != 0)
            {
                var meter = _unitOfWork.MeterRepository.Get((int)meterId);
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
