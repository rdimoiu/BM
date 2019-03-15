using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using X.PagedList;

namespace BuildingManagement.Controllers
{
    public class SectionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SectionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Section
        public ActionResult Index(int? page, string currentFilter, string searchString, string sortOrder)
        {
            IEnumerable<Section> sections;
            var pageNumber = page ?? 1;
            const int pageSize = 3;
            if (searchString != null)
            {
                pageNumber = 1;
                sections = _unitOfWork.SectionRepository.GetFilteredSectionsIncludingClient(searchString);
            }
            else
            {
                if (currentFilter != null)
                {
                    searchString = currentFilter;
                    sections = _unitOfWork.SectionRepository.GetFilteredSectionsIncludingClient(searchString);
                }
                else
                {
                    sections = _unitOfWork.SectionRepository.GetAllSectionsIncludingClient();
                }
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClientSortParm = string.IsNullOrEmpty(sortOrder) ? "client_desc" : "";
            ViewBag.NumberSortParm = sortOrder == "Number" ? "number_desc" : "Number";
            ViewBag.SurfaceSortParm = sortOrder == "Surface" ? "surface_desc" : "Surface";
            ViewBag.PeopleSortParm = sortOrder == "People" ? "people_desc" : "People";
            sections = _unitOfWork.SectionRepository.OrderSections(sections, sortOrder);
            ViewBag.OnePageOfSections = sections.ToPagedList(pageNumber, pageSize);
            return View(ViewBag.OnePageOfSections);
        }

        // GET: Section/Details/5
        public ActionResult Details(int id)
        {
            var section = _unitOfWork.SectionRepository.Get(id);
            if (section == null)
            {
                return HttpNotFound();
            }
            return View(section);
        }

        // GET: Section/Create
        public ActionResult Create()
        {
            var section = new Section();
            PopulateClientsDropDownList();
            return View(section);
        }

        // POST: Section/Create
        [HttpPost]
        public ActionResult Create(Section section)
        {
            if (ModelState.IsValid)
            {
                //uniqueness condition check
                var duplicateSection = _unitOfWork.SectionRepository.FirstOrDefault(s => s.Number == section.Number && s.ClientID == section.ClientID);
                if (duplicateSection != null)
                {
                    PopulateClientsDropDownList(section.ClientID);
                    return new HttpStatusCodeResult(409, "A section with this number already exists for this client.");
                }
                var client = _unitOfWork.ClientRepository.Get(section.ClientID);
                if (client != null)
                {
                    section.Client = client;
                }
                section.Surface = 0m;
                section.People = 0;
                try
                {
                    _unitOfWork.SectionRepository.Add(section);
                    _unitOfWork.Save();
                    TempData["message"] = $"Section {section.Number} has been created.";
                    return Json(section.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateClientsDropDownList(section.ClientID);
            return View(section);
        }

        // GET: Section/Edit/5
        public ActionResult Edit(int id)
        {
            var section = _unitOfWork.SectionRepository.Get(id);
            if (section == null)
            {
                return HttpNotFound();
            }
            PopulateClientsDropDownList(section.ClientID);
            return View(section);
        }

        // POST: Section/Edit/5
        [HttpPost]
        public ActionResult Edit(Section section)
        {
            var sectionToUpdate = _unitOfWork.SectionRepository.GetSectionIncludingClient(section.ID);
            if (sectionToUpdate == null)
            {
                return HttpNotFound();
            }
            if (TryUpdateModel(sectionToUpdate, "", new[] { "Number", "ClientID" }))
            {
                try
                {
                    //uniqueness condition check
                    var duplicateSection = _unitOfWork.SectionRepository.FirstOrDefault(s => s.Number == sectionToUpdate.Number && s.ClientID == sectionToUpdate.ClientID);
                    if (duplicateSection != null && duplicateSection.ID != sectionToUpdate.ID)
                    {
                        PopulateClientsDropDownList(sectionToUpdate.ClientID);
                        return new HttpStatusCodeResult(409, "A section with this number already exists for this client.");
                    }
                    _unitOfWork.Save();
                    TempData["message"] = $"Section {sectionToUpdate.Number} has been edited.";
                    return Json(sectionToUpdate.ID);
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateClientsDropDownList(sectionToUpdate.ClientID);
            return View(sectionToUpdate);
        }

        // GET: Section/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var section = _unitOfWork.SectionRepository.GetSectionIncludingClient(id);
            if (section == null)
            {
                return HttpNotFound();
            }
            return View(section);
        }

        // POST: Section/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var section = _unitOfWork.SectionRepository.Get(id);
                if (section == null)
                {
                    return HttpNotFound();
                }
                _unitOfWork.SectionRepository.Remove(section);
                _unitOfWork.Save();
                TempData["message"] = $"Section {section.Number} has been deleted.";
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

        private void PopulateClientsDropDownList(object selectedClient = null)
        {
            var clientsQuery = _unitOfWork.ClientRepository.GetAll();
            ViewBag.ClientID = new SelectList(clientsQuery, "ID", "Name", selectedClient);
        }
    }
}
