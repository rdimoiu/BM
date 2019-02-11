using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BuildingManagement.DAL;
using BuildingManagement.Models;
using BuildingManagement.ViewModels;

namespace BuildingManagement.Controllers
{
    public class CostsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CostsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        // GET: Costs
        public ActionResult Index()
        {
            CostsIndexData costsIndexData = new CostsIndexData
            {
                //DiscountMonth = DateTime.Now,
                Client = new Client(),
                Section = new Section(),
                Spaces = new List<Space>(),
                Costs = new List<Cost>(),
                Invoices = new List<Invoice>(),
                Services = new List<Service>()
            };
            PopulateClientsDropDownList();
            PopulateSectionsDropDownList();
            return View(costsIndexData);
        }

        private void PopulateClientsDropDownList(object selectedClient = null)
        {
            var clientsQuery = from c in _unitOfWork.ClientRepository.GetAll() select c;
            ViewBag.ClientID = new SelectList(clientsQuery, "ID", "Name", selectedClient);
        }

        private void PopulateSectionsDropDownList(object selectedSection = null)
        {
            var sectionsQuery = from s in _unitOfWork.SectionRepository.GetAll() select s;
            ViewBag.SectionID = new SelectList(sectionsQuery, "ID", "Number", selectedSection);
        }

        [HttpPost]
        public ActionResult GetSectionsByClient(int? clientId, int? sectionId)
        {
            var list = new List<SelectListItem>();
            var sections = clientId != null ? _unitOfWork.SectionRepository.Find(s => s.ClientID == clientId).ToList() : _unitOfWork.SectionRepository.GetAll().ToList();
            foreach (var section in sections)
            {
                if (clientId != null && sectionId != null && sectionId == section.ID)
                {
                    list.Add(new SelectListItem { Value = section.ID.ToString(), Text = section.Number, Selected = true });
                }
                else
                {
                    list.Add(new SelectListItem { Value = section.ID.ToString(), Text = section.Number });
                }
            }
            return Json(list);
        }
    }
}