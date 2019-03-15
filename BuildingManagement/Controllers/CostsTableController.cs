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
    public class CostsTableController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CostsTableController(IUnitOfWork unitOfWork)
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
                Services = new List<Service>(),
                Cols = new Dictionary<string, string>(),
                Rows = new Dictionary<string, Dictionary<string, string>>()
            };

            if (discountMonth != null && sectionId != null)
            {
                costsIndexData.Section = _unitOfWork.SectionRepository.GetSectionIncludingClientAndServices((int)sectionId);
                //foreach (var service in costsIndexData.Section.Services)
                //{
                //    costsIndexData.Services.Add(service);
                //}
                costsIndexData.Spaces = new List<Space>();
                costsIndexData.Services = new List<Service>();
                var levels = _unitOfWork.LevelRepository.GetLevelsIncludingServicesBySection((int)sectionId).ToList();
                foreach (var level in levels)
                {
                    //foreach (var service in level.Services)
                    //{
                    //    costsIndexData.Services.Add(service);
                    //}
                    var spaces = _unitOfWork.SpaceRepository.GetSpacesIncludingServicesByLevel(level.ID).ToList();
                    foreach (var space in spaces)
                    {
                        costsIndexData.Spaces.Add(space);
                        foreach (var service in space.Services)
                        {
                            if (!costsIndexData.Services.Contains(service))
                            {
                                costsIndexData.Services.Add(service);
                            }
                        }
                    }
                }

                costsIndexData.Rows = new Dictionary<string, Dictionary<string, string>>();
                costsIndexData.Cols = new Dictionary<string, string>();

                costsIndexData.Cols.Add("Space", "");
                costsIndexData.Cols.Add("SpaceType", "");
                costsIndexData.Cols.Add("SubClient", "");
                costsIndexData.Cols.Add("Surface", "");
                costsIndexData.Cols.Add("People", "");

                //foreach (var service in totalServices)
                //{
                //    costsIndexData.Cols.Add(service.Name, "");
                //}
                //foreach (var space in totalSpaces)
                //{
                //    costsIndexData.Rows.Add(space.Number + " " + space.Level.Number, costsIndexData.Cols);
                //}

                //Console.WriteLine(costsIndexData.Rows["AP 2.1 Nivel 2 (AP)"]["1.1"]);

                foreach (var space in costsIndexData.Spaces)
                {
                    var col = new Dictionary<string, string>();
                    col.Add("Space", space.Number + " " + space.Level.Number);
                    col.Add("SpaceType", space.SpaceType.Type);
                    col.Add("SubClient", space.SubClient.Name);
                    col.Add("Surface", space.Surface.ToString());
                    col.Add("People", space.People.ToString());
                    
                    foreach (var service in costsIndexData.Services)
                    {
                        if (!costsIndexData.Cols.ContainsKey(service.Name))
                        {
                            costsIndexData.Cols.Add(service.Name, "1");
                        }
                        var cost = _unitOfWork.CostRepository.FirstOrDefault(c => c.ServiceID == service.ID && c.SpaceID == space.ID);
                        if (cost != null)
                        {
                            col.Add(service.Name, cost.Value.ToString());
                        }
                        else
                        {
                            col.Add(service.Name, "");
                        }
                    }
                    costsIndexData.Rows.Add(space.Number + " " + space.Level.Number, col);
                }


                //costsIndexData.DiscountMonth = (DateTime)discountMonth;

                //var invoiceTypes = _unitOfWork.InvoiceTypeRepository.GetAll();
                //foreach (var invoiceType in invoiceTypes)
                //{
                //    var invoices = _unitOfWork.InvoiceRepository.GetAll().Where(i => i.DiscountMonth.Month == ((DateTime)discountMonth).Month && i.InvoiceTypeID == invoiceType.ID).ToList();
                //    foreach (var invoice in invoices)
                //    {
                //        var services = _unitOfWork.ServiceRepository.GetAll().Where(s => s.InvoiceID == invoice.ID).ToList();

                //    }
                //}
            }

            PopulateClientsDropDownList();
            PopulateSectionsDropDownList();
            return View(costsIndexData);
        }

        private void PopulateClientsDropDownList(object selectedClient = null)
        {
            var clientsQuery = from c in _unitOfWork.ClientRepository.GetAll().ToList() select c;
            ViewBag.ClientID = new SelectList(clientsQuery, "ID", "Name", selectedClient);
        }

        private void PopulateSectionsDropDownList(object selectedSection = null)
        {
            var sectionsQuery = from s in _unitOfWork.SectionRepository.GetAll().ToList() select s;
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