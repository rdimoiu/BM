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
    public class MeterController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        // GET: Meter
        public ActionResult Index()
        {
            var viewModel = new MeterIndexData
            {
                Meters =
                    _unitOfWork.MeterRepository.Get(
                        includeProperties: "DistributionMode, MeterTypes, Sections, Levels, Spaces")
            };
            return View(viewModel);
        }

        // GET: Meter/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var meter = _unitOfWork.MeterRepository.GetById(id);
            if (meter == null)
            {
                return HttpNotFound();
            }
            return View(meter);
        }

        // GET: Meter/Create
        public ActionResult Create()
        {
            var model = new Meter();
            PopulateDistributionModesDropDownList();
            return View(model);
        }

        // POST: Meter/Create
        [HttpPost]
        public ActionResult CreateMeter(Meter meter)
        {
            //uniqueness condition check
            if (meter.Code != null)
            {
                var duplicateMeter = _unitOfWork.MeterRepository.Get(filter: m => m.Code == meter.Code).FirstOrDefault();
                if (duplicateMeter != null)
                {
                    ModelState.AddModelError("Code", "A meter with this code already exists.");
                    PopulateDistributionModesDropDownList(meter.DistributionModeID);
                    return View("Create", meter);
                }
            }
            if (meter.DistributionModeID != 0)
            {
                var distributionMode = _unitOfWork.DistributionModeRepository.GetById(meter.DistributionModeID);
                if (distributionMode != null)
                {
                    meter.DistributionMode = distributionMode;
                }
            }
            if (meter.MeterTypesSelected != null)
            {
                meter.MeterTypes = new List<MeterType>();
                foreach (var meterTypeSelected in meter.MeterTypesSelected)
                {
                    if (meterTypeSelected != "root")
                    {
                        var meterType = _unitOfWork.MeterTypeRepository.GetById(int.Parse(meterTypeSelected));
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

                var building = Utils.MapSelectedIDsToDatabaseIDs(meter.MeterSLSSelected);
                meter.Sections = new List<Section>();
                foreach (var sectionId in building.SectionIDs)
                {
                    var section = _unitOfWork.SectionRepository.GetById(sectionId);
                    if (section != null)
                    {
                        meter.Sections.Add(section);
                    }
                }
                meter.Levels = new List<Level>();
                foreach (var levelId in building.LevelIDs)
                {
                    var level = _unitOfWork.LevelRepository.GetById(levelId);
                    if (level != null)
                    {
                        meter.Levels.Add(level);
                    }
                }
                meter.Spaces = new List<Space>();
                foreach (var spaceId in building.SpaceIDs)
                {
                    var space = _unitOfWork.SpaceRepository.GetById(spaceId);
                    if (space != null)
                    {
                        meter.Spaces.Add(space);
                    }
                }

                #endregion
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.MeterRepository.Insert(meter);
                    _unitOfWork.Save();
                    return Json(meter.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("",
                        "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateDistributionModesDropDownList(meter.DistributionModeID);
            return View("Create", meter);
        }

        // GET: Meter/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var meter =
                _unitOfWork.MeterRepository.Get(
                    includeProperties: "DistributionMode, MeterTypes, Sections, Levels, Spaces").Single(m => m.ID == id);
            if (meter == null)
            {
                return HttpNotFound();
            }
            PopulateDistributionModesDropDownList(meter.DistributionModeID);
            return View(meter);
        }

        // POST: Meter/Edit/5
        [HttpPost]
        public ActionResult EditMeter(Meter meter)
        {
            var meterToUpdate =
                _unitOfWork.MeterRepository.Get(
                    includeProperties: "DistributionMode, MeterTypes, Sections, Levels, Spaces")
                    .FirstOrDefault(m => m.ID == meter.ID);
            if (meterToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(meterToUpdate, "",
                new[] {"Code", "Details", "InitialIndex", "Defect", "DistributionModeID"}))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateMeter =
                        _unitOfWork.MeterRepository.Get(filter: m => m.Code == meterToUpdate.Code).FirstOrDefault();
                    if (duplicateMeter != null && duplicateMeter.ID != meterToUpdate.ID)
                    {
                        ModelState.AddModelError("", "A meter with this code already exists.");
                        PopulateDistributionModesDropDownList(meterToUpdate.DistributionModeID);
                        return View("Edit", meterToUpdate);
                    }
                    UpdateMeterMeterTypes(meter.MeterTypesSelected, meterToUpdate);

                    #region Update Sections, Levels and Spaces

                    var building = Utils.MapSelectedIDsToDatabaseIDs(meter.MeterSLSSelected);
                    meterToUpdate.Sections.Clear();
                    foreach (var sectionId in building.SectionIDs)
                    {
                        var section = _unitOfWork.SectionRepository.GetById(sectionId);
                        if (section != null)
                        {
                            meterToUpdate.Sections.Add(section);
                        }
                    }
                    meterToUpdate.Levels.Clear();
                    foreach (var levelId in building.LevelIDs)
                    {
                        var level = _unitOfWork.LevelRepository.GetById(levelId);
                        if (level != null)
                        {
                            meterToUpdate.Levels.Add(level);
                        }
                    }
                    meterToUpdate.Spaces.Clear();
                    foreach (var spaceId in building.SpaceIDs)
                    {
                        var space = _unitOfWork.SpaceRepository.GetById(spaceId);
                        if (space != null)
                        {
                            meterToUpdate.Spaces.Add(space);
                        }
                    }

                    #endregion

                    _unitOfWork.Save();
                    return Json(meterToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("",
                        "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateDistributionModesDropDownList(meterToUpdate.DistributionModeID);
            return View("Edit", meterToUpdate);
        }

        // GET: Meter/Delete/5
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
            var meter =
                _unitOfWork.MeterRepository.Get(includeProperties: "DistributionMode, MeterTypes, Spaces")
                    .Single(m => m.ID == id);
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
                _unitOfWork.MeterRepository.Delete(id);
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

        private void UpdateMeterMeterTypes(List<string> meterTypes, Meter meter)
        {
            if (meterTypes == null)
            {
                meter.MeterTypes = new List<MeterType>();
                return;
            }
            var selectedMeterTypes = new HashSet<string>(meterTypes);
            var meterMeterTypes = new HashSet<int>(meter.MeterTypes.Select(mt => mt.ID));
            foreach (var meterType in _unitOfWork.MeterTypeRepository.Get())
            {
                if (selectedMeterTypes.Contains(meterType.ID.ToString()))
                {
                    if (!meterMeterTypes.Contains(meterType.ID))
                    {
                        meter.MeterTypes.Add(meterType);
                    }
                }
                else
                {
                    if (meterMeterTypes.Contains(meterType.ID))
                    {
                        meter.MeterTypes.Remove(meterType);
                    }
                }
            }
        }

        [HttpGet]
        public string GetMeterTypesTreeData(int? meterId)
        {
            var root = new TreeNode
            {
                id = "root",
                children = {},
                text = "root",
                state = new TreeNodeState {opened = true}
            };

            HashSet<int> selectedMeterTypes = new HashSet<int>();
            if (meterId != null)
            {
                var meter =
                    _unitOfWork.MeterRepository.Get(includeProperties: "MeterTypes")
                        .FirstOrDefault(m => m.ID == meterId);
                if (meter != null)
                {
                    selectedMeterTypes = new HashSet<int>(meter.MeterTypes.Select(mt => mt.ID));
                }
            }

            var meterTypes = _unitOfWork.MeterTypeRepository.Get().ToList();
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
        public string GetSpacesTreeData(int? meterId)
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
            if (meterId != null)
            {
                var meter =
                    _unitOfWork.MeterRepository.Get(includeProperties: "Sections, Levels, Spaces")
                        .FirstOrDefault(m => m.ID == meterId);
                if (meter != null)
                {
                    selectedSpacesIDs = new HashSet<int>(meter.Spaces.Select(s => s.ID));
                    selectedLevelsIDs = new HashSet<int>(meter.Levels.Select(l => l.ID));
                    selectedSectionsIDs = new HashSet<int>(meter.Sections.Select(s => s.ID));
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
                            var spaces = _unitOfWork.SpaceRepository.Get().Where(s => s.LevelID == level.ID).ToList();
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
            return new JavaScriptSerializer().Serialize(root);
        }
    }
}
