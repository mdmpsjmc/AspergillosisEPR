
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Search;
using AspergillosisEPR.Models;
using System;
using AspergillosisEPR.Models.PatientViewModels;

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class PartialsController : Controller
    {
        private readonly AspergillosisContext _context;

        public PartialsController(AspergillosisContext context)
        {

            _context = context;
        }
        [Authorize(Roles ="Create Role, Admin Role")]
        public IActionResult DiagnosisForm()
        {
            PopulateDiagnosisCategoriesDropDownList();
            PopulateDiagnosisTypeDropDownList();
            return PartialView();
        }

        [Authorize(Roles = "Create Role, Admin Role")]
        public IActionResult DrugForm()
        {
            PopulateDrugsDropDownList();
            PopulateSideEffectsDropDownList();
            return PartialView();
        }

        [Authorize(Roles = "Create Role, Admin Role")]
        public IActionResult STGForm()
        {
            return PartialView();
        }

        [Authorize(Roles = "Update Role, Admin Role")]
        public IActionResult EditDiagnosisForm()
        {
            PopulateDiagnosisCategoriesDropDownList();
            PopulateDiagnosisTypeDropDownList();
            ViewBag.Index = (string)Request.Query["index"];
            return PartialView();
        }

        [Authorize(Roles = "Update Role, Admin Role")]
        public IActionResult EditDrugForm()
        {
            PopulateDrugsDropDownList();
            PopulateSideEffectsDropDownList();
            ViewBag.Index = (string)Request.Query["index"];
            return PartialView();
        }

        [Authorize(Roles = "Update Role, Admin Role")]
        public IActionResult EditSTGForm()
        {
            ViewBag.Index = (string)Request.Query["index"];
            return PartialView();
        }


        [Authorize(Roles = "Read Role")]
        public IActionResult SearchPartial()
        {
            ViewBag.Index = (string)Request.Query["index"];
            CriteriaClassesDropdownList();
            CriteriaMatchesDropdownList();
            PatientFieldsDropdownList();
            var searchVm = new PatientSearchViewModel();
            searchVm.Index = (string)Request.Query["index"];
            return PartialView(searchVm);
        }

        [Authorize(Roles = "Read Role")]
        public IActionResult CriteriaPartial()
        {
            string klassName = Request.Query["searchClass"];
            Type type = Type.GetType("AspergillosisEPR.Models." + klassName);
            var searchVm = new PatientSearchViewModel();
            searchVm.Index = (string) Request.Query["index"];
            dynamic instance = Activator.CreateInstance(type);
            Dictionary<string, string> searchableItems = instance.SearchableFields();
            ViewBag.SearchableItems = new SelectList(searchableItems, "Value", "Key");
            return PartialView(searchVm);
        }


        private void PopulateDiagnosisCategoriesDropDownList(object selectedCategory = null)
        {
            var categoriesQuery = from d in _context.DiagnosisCategories
                                  orderby d.CategoryName
                                  select d;
            ViewBag.DiagnosisCategoryId = new SelectList(categoriesQuery.AsNoTracking(), "ID", "CategoryName", selectedCategory);
        }

        private void PopulateDiagnosisTypeDropDownList(object selectedDiagnosis = null)
        {
            var diagnosisTypesQuery = from d in _context.DiagnosisTypes
                                      orderby d.Name
                                      select d;
            ViewBag.DiagnosisTypeId = new SelectList(diagnosisTypesQuery.AsNoTracking(), "ID", "Name", selectedDiagnosis);
        }

        private void PopulateDrugsDropDownList(object selectedDrug = null)
        {
            var drugsQuery = from d in _context.Drugs
                                      orderby d.Name
                                      select d;
            ViewBag.DrugId = new SelectList(drugsQuery.AsNoTracking(), "ID", "Name", selectedDrug);
        }

        private void PopulateSideEffectsDropDownList(object selectedIds = null)
        {
            var sideEffects = from se in _context.SideEffects
                              orderby se.Name
                              select se;
            ViewBag.SideEffects = new MultiSelectList(sideEffects, "ID", "Name");
        }

        private SelectList CriteriaClassesDropdownList()
        {
            return ViewBag.CriteriaClasses = new SelectList(PatientSearch.CriteriaClasses().OrderBy(x => x.Value), "Value", "Key", "Patient");
        }

        private SelectList CriteriaMatchesDropdownList()
        {
            return ViewBag.CriteriaMatches = new SelectList(PatientSearch.CriteriaMatches().OrderBy(x => x.Value), "Value", "Key", "Exact");
        }

        private SelectList PatientFieldsDropdownList()
        {
            var patient = new Patient();
            return ViewBag.PatientFields = new SelectList(patient.SearchableFields(), "Value", "Key", "RM2Number");
        }
    }


}