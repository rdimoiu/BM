using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using BuildingManagement.ViewModels;
using System.Web.Script.Serialization;

namespace BuildingManagement.Controllers
{
    public class SubMeterController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        // GET: SubMeter
        public ActionResult Index()
        {
            var viewModel = new SubMeterIndexData
            {
                SubMeters =
                    _unitOfWork.SubMeterRepository.Get(
                        includeProperties: "Meter, DistributionMode, MeterTypes, Sections, Levels, Spaces")
            };
            return View(viewModel);
        }

        // GET: SubMeter/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var subMeter = _unitOfWork.SubMeterRepository.GetById(id);
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

        // POST: Meter/Create
        [HttpPost]
        public ActionResult CreateSubMeter(SubMeter subMeter)
        {
            //uniqueness condition check
            if (subMeter.Code != null)
            {
                var duplicateSubMeter =
                    _unitOfWork.SubMeterRepository.Get(filter: sm => sm.Code == subMeter.Code).FirstOrDefault();
                if (duplicateSubMeter != null)
                {
                    ModelState.AddModelError("Code", "A sub meter with this code already exists.");
                    PopulateMetersDropDownList(subMeter.MeterID);
                    PopulateDistributionModesDropDownList(subMeter.DistributionModeID);
                    return View("Create", subMeter);
                }
            }
            if (subMeter.MeterID != 0)
            {
                var meter = _unitOfWork.MeterRepository.GetById(subMeter.MeterID);
                if (meter != null)
                {
                    subMeter.Meter = meter;
                }
            }
            if (subMeter.DistributionModeID != 0)
            {
                var distributionMode = _unitOfWork.DistributionModeRepository.GetById(subMeter.DistributionModeID);
                if (distributionMode != null)
                {
                    subMeter.DistributionMode = distributionMode;
                }
            }
            if (subMeter.SubMeterTypesSelected != null)
            {
                subMeter.MeterTypes = new List<MeterType>();
                foreach (var subMeterTypeSelected in subMeter.SubMeterTypesSelected)
                {
                    if (subMeterTypeSelected != "root")
                    {
                        var meterType = _unitOfWork.MeterTypeRepository.GetById(int.Parse(subMeterTypeSelected));
                        if (meterType != null)
                        {
                            subMeter.MeterTypes.Add(meterType);
                        }
                    }
                }
            }
            if (subMeter.SubMeterSLSSelected != null)
            {
                #region Add Sections, Levels and Spaces

                var building = Utils.MapSelectedIDsToDatabaseIDs(subMeter.SubMeterSLSSelected);
                subMeter.Sections = new List<Section>();
                foreach (var sectionId in building.SectionIDs)
                {
                    var section = _unitOfWork.SectionRepository.GetById(sectionId);
                    if (section != null)
                    {
                        subMeter.Sections.Add(section);
                    }
                }
                subMeter.Levels = new List<Level>();
                foreach (var levelId in building.LevelIDs)
                {
                    var level = _unitOfWork.LevelRepository.GetById(levelId);
                    if (level != null)
                    {
                        subMeter.Levels.Add(level);
                    }
                }
                subMeter.Spaces = new List<Space>();
                foreach (var spaceId in building.SpaceIDs)
                {
                    var space = _unitOfWork.SpaceRepository.GetById(spaceId);
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
                    _unitOfWork.SubMeterRepository.Insert(subMeter);
                    _unitOfWork.Save();
                    return Json(subMeter.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("",
                        "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateMetersDropDownList(subMeter.MeterID);
            PopulateDistributionModesDropDownList(subMeter.DistributionModeID);
            return View("Create", subMeter);
        }

        // GET: SubMeter/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var subMeter =
                _unitOfWork.SubMeterRepository.Get(
                    includeProperties: "Meter, DistributionMode, MeterTypes, Sections, Levels, Spaces")
                    .Single(sm => sm.ID == id);
            if (subMeter == null)
            {
                return HttpNotFound();
            }
            PopulateMetersDropDownList(subMeter.MeterID);
            PopulateDistributionModesDropDownList(subMeter.DistributionModeID);
            return View(subMeter);
        }

        // POST: Meter/Edit/5
        [HttpPost]
        public ActionResult EditSubMeter(SubMeter subMeter)
        {
            var subMeterToUpdate =
                _unitOfWork.SubMeterRepository.Get(
                    includeProperties: "Meter, DistributionMode, MeterTypes, Sections, Levels, Spaces")
                    .FirstOrDefault(sm => sm.ID == subMeter.ID);
            if (subMeterToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(subMeterToUpdate, "",
                new[] {"Code", "Details", "InitialIndex", "Defect", "MeterID", "DistributionModeID"}))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateSubMeter =
                        _unitOfWork.SubMeterRepository.Get(filter: sm => sm.Code == subMeterToUpdate.Code)
                            .FirstOrDefault();
                    if (duplicateSubMeter != null && duplicateSubMeter.ID != subMeterToUpdate.ID)
                    {
                        ModelState.AddModelError("", "A sub meter with this code already exists.");
                        PopulateMetersDropDownList(subMeterToUpdate.MeterID);
                        PopulateDistributionModesDropDownList(subMeterToUpdate.DistributionModeID);
                        return View("Edit", subMeterToUpdate);
                    }
                    UpdateSubMeterMeterTypes(subMeter.SubMeterTypesSelected, subMeterToUpdate);

                    #region Update Sections, Levels and Spaces

                    var building = Utils.MapSelectedIDsToDatabaseIDs(subMeter.SubMeterSLSSelected);
                    subMeterToUpdate.Sections.Clear();
                    foreach (var sectionId in building.SectionIDs)
                    {
                        var section = _unitOfWork.SectionRepository.GetById(sectionId);
                        if (section != null)
                        {
                            subMeterToUpdate.Sections.Add(section);
                        }
                    }
                    subMeterToUpdate.Levels.Clear();
                    foreach (var levelId in building.LevelIDs)
                    {
                        var level = _unitOfWork.LevelRepository.GetById(levelId);
                        if (level != null)
                        {
                            subMeterToUpdate.Levels.Add(level);
                        }
                    }
                    subMeterToUpdate.Spaces.Clear();
                    foreach (var spaceId in building.SpaceIDs)
                    {
                        var space = _unitOfWork.SpaceRepository.GetById(spaceId);
                        if (space != null)
                        {
                            subMeterToUpdate.Spaces.Add(space);
                        }
                    }

                    #endregion

                    _unitOfWork.Save();
                    return Json(subMeterToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("",
                        "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateMetersDropDownList(subMeterToUpdate.MeterID);
            PopulateDistributionModesDropDownList(subMeterToUpdate.DistributionModeID);
            return View("Edit", subMeterToUpdate);
        }

        // GET: SubMeter/Delete/5
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
            var subMeter =
                _unitOfWork.SubMeterRepository.Get(
                    includeProperties: "Meter, DistributionMode, MeterTypes, Sections, Levels, Spaces")
                    .Single(sm => sm.ID == id);
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
                _unitOfWork.SubMeterRepository.Delete(id);
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

        private void PopulateDistributionModesDropDownList(object selectedDistributionMode = null)
        {
            var distributionModesQuery = from dm in _unitOfWork.DistributionModeRepository.Get() select dm;
            ViewBag.DistributionModeID = new SelectList(distributionModesQuery, "ID", "Mode", selectedDistributionMode);
        }

        private void PopulateMetersDropDownList(object selectedMeter = null)
        {
            var metersQuery = from m in _unitOfWork.MeterRepository.Get() select m;
            ViewBag.MeterID = new SelectList(metersQuery, "ID", "Code", selectedMeter);
        }

        private void UpdateSubMeterMeterTypes(List<string> meterTypes, SubMeter subMeter)
        {
            if (meterTypes == null)
            {
                subMeter.MeterTypes = new List<MeterType>();
                return;
            }
            var selectedMeterTypes = new HashSet<string>(meterTypes);
            var subMeterMeterTypes = new HashSet<int>(subMeter.MeterTypes.Select(mt => mt.ID));
            foreach (var meterType in _unitOfWork.MeterTypeRepository.Get())
            {
                if (selectedMeterTypes.Contains(meterType.ID.ToString()))
                {
                    if (!subMeterMeterTypes.Contains(meterType.ID))
                    {
                        subMeter.MeterTypes.Add(meterType);
                    }
                }
                else
                {
                    if (subMeterMeterTypes.Contains(meterType.ID))
                    {
                        subMeter.MeterTypes.Remove(meterType);
                    }
                }
            }
        }

        [HttpGet]
        public string GetMeterTypesTreeData(int? subMeterId, int? meterId)
        {
            var root = new TreeNode
            {
                id = "root",
                children = {},
                text = "root",
                state = new TreeNodeState {opened = true}
            };

            HashSet<int> selectedMeterTypes = new HashSet<int>();
            if (subMeterId != null)
            {
                var subMeter =
                    _unitOfWork.SubMeterRepository.Get(includeProperties: "MeterTypes")
                        .FirstOrDefault(sm => sm.ID == subMeterId);
                if (subMeter != null)
                {
                    selectedMeterTypes = new HashSet<int>(subMeter.MeterTypes.Select(mt => mt.ID));
                }
            }

            List<MeterType> meterTypes = new List<MeterType>();
            if (meterId != null)
            {
                var meter =
                    _unitOfWork.MeterRepository.Get(includeProperties: "MeterTypes")
                        .FirstOrDefault(m => m.ID == meterId);
                if (meter != null)
                {
                    meterTypes = meter.MeterTypes.ToList();
                }
            }
            else
            {
                meterTypes = _unitOfWork.MeterTypeRepository.Get().ToList();
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
                text = "root",
                state = new TreeNodeState {opened = true}
            };

            HashSet<int> selectedSpacesIDs = new HashSet<int>();
            HashSet<int> selectedLevelsIDs = new HashSet<int>();
            HashSet<int> selectedSectionsIDs = new HashSet<int>();
            if (subMeterId != null)
            {
                var subMeter =
                    _unitOfWork.SubMeterRepository.Get(includeProperties: "Sections, Levels, Spaces")
                        .FirstOrDefault(sm => sm.ID == subMeterId);
                if (subMeter != null)
                {
                    selectedSpacesIDs = new HashSet<int>(subMeter.Spaces.Select(s => s.ID));
                    selectedLevelsIDs = new HashSet<int>(subMeter.Levels.Select(l => l.ID));
                    selectedSectionsIDs = new HashSet<int>(subMeter.Sections.Select(s => s.ID));
                }
            }

            List<Space> meterSpaces = new List<Space>();
            List<Level> meterLevels = new List<Level>();
            List<Section> meterSections = new List<Section>();
            if (meterId != null)
            {
                var meter =
                    _unitOfWork.MeterRepository.Get(includeProperties: "Sections, Levels, Spaces").FirstOrDefault(m => m.ID == meterId);
                if (meter != null)
                {
                    meterSpaces = meter.Spaces.ToList();
                    meterLevels = meter.Levels.ToList();
                    meterSections = meter.Sections.ToList();
                }
            }

            //add all levels for meter's sections
            foreach (var section in meterSections)
            {
                var levelsToAdd = _unitOfWork.LevelRepository.Get().Where(l => l.SectionID == section.ID);
                foreach (var level in levelsToAdd)
                {
                    if (!meterLevels.Contains(level))
                    {
                        meterLevels.Add(level);
                    }
                }
            }
            //add all spaces for meter's levels
            foreach (var level in meterLevels)
            {
                var spacesToAdd = _unitOfWork.SpaceRepository.Get().Where(s => s.LevelID == level.ID);
                foreach (var space in spacesToAdd)
                {
                    if (!meterSpaces.Contains(space))
                    {
                        meterSpaces.Add(space);
                    }
                }
            }

            var sections = _unitOfWork.SectionRepository.Get().ToList();
            if (sections.Any())
            {
                foreach (var section in sections)
                {
                    var sectionNode = new TreeNode();
                    sectionNode.id = section.ID.ToString();
                    sectionNode.text = section.Number;
                    sectionNode.state = new TreeNodeState();
                    sectionNode.state.opened = true;
                    if (meterSections.Contains(section))
                    {
                        if (selectedSectionsIDs.Count > 0 && selectedSectionsIDs.Contains(section.ID))
                        {
                            sectionNode.state.selected = true;
                        }
                    }
                    else
                    {
                        sectionNode.state.disabled = true;
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
                            if (meterLevels.Contains(level))
                            {
                                if (selectedLevelsIDs.Count > 0 && selectedLevelsIDs.Contains(level.ID))
                                {
                                    levelNode.state.selected = true;
                                }
                            }
                            else
                            {
                                levelNode.state.disabled = true;
                            }
                            sectionNode.children.Add(levelNode);
                            var spaces = _unitOfWork.SpaceRepository.Get().Where(s => s.LevelID == level.ID).ToList();
                            if (spaces.Any())
                            {
                                foreach (var space in spaces)
                                {
                                    var spaceNode = new TreeNode();
                                    spaceNode.id = section.ID + "." + level.ID + "." + space.ID;
                                    spaceNode.text = space.Number;
                                    spaceNode.state = new TreeNodeState();
                                    if (meterSpaces.Contains(space))
                                    {
                                        if (selectedSpacesIDs.Count > 0 && selectedSpacesIDs.Contains(space.ID))
                                        {
                                            spaceNode.state.selected = true;
                                        }
                                    }
                                    else
                                    {
                                        spaceNode.state.disabled = true;
                                    }
                                    levelNode.children.Add(spaceNode);
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
