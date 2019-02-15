using System;
using System.Collections;
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
        public ActionResult Index(DateTime? discountMonth, int? sectionId)
        {
            CostsIndexData costsIndexData = new CostsIndexData
            {
                //DiscountMonth = DateTime.Now,
                Client = new Client(),
                Section = new Section(),
                Spaces = new List<Space>(),
                Costs = new List<Cost>(),
                Invoices = new List<Invoice>(),
                Services = new HashSet<Service>()
            };

            if (discountMonth != null && sectionId != null)
            {
                costsIndexData.DiscountMonth = (DateTime)discountMonth;
                costsIndexData.Invoices = _unitOfWork.InvoiceRepository.GetAll().Where(i => i.DiscountMonth.Month == ((DateTime)discountMonth).Month);

                foreach (var x in costsIndexData.Invoices)
                {
                    var service1 =
                        _unitOfWork.ServiceRepository.GetServiceIncludingSpacesAndCosts(x.Services.FirstOrDefault().ID);
                }
                

                costsIndexData.Section = _unitOfWork.SectionRepository.GetSectionIncludingClientAndServices((int)sectionId);
                foreach (var service in costsIndexData.Section.Services)
                {
                    costsIndexData.Services.Add(service);
                }
                var levels = _unitOfWork.LevelRepository.GetLevelsIncludingServicesBySection((int)sectionId).ToList();
                foreach (var level in levels)
                {
                    foreach (var service in level.Services)
                    {
                        costsIndexData.Services.Add(service);
                    }
                    var spaces = _unitOfWork.SpaceRepository.GetSpacesIncludingServicesByLevel(level.ID).ToList();
                    foreach (var space in spaces)
                    {
                        costsIndexData.Spaces.Add(space);
                        foreach (var service in space.Services)
                        {
                            costsIndexData.Services.Add(service);
                        }
                    }
                }

                costsIndexData.Rows = new Dictionary<string, Dictionary<string, string>>();
                costsIndexData.Cols = new Dictionary<string, string>();
                foreach (var service in costsIndexData.Services)
                {
                    costsIndexData.Cols.Add(service.Name, "");
                }
                foreach (var space in costsIndexData.Spaces)
                {
                    costsIndexData.Rows.Add(space.Number + " " + space.Level.Number, costsIndexData.Cols);
                }
            }

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