
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
using AspergillosisEPR.Lib.Search;
using AspergillosisEPR.Lib;

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class PartialsController : Controller
    {
        private readonly AspergillosisContext _context;
        private readonly DropdownListsResolver _listResolver;

        public PartialsController(AspergillosisContext context)
        {
            _listResolver = new DropdownListsResolver(context, ViewBag);
            _context = context;
        }
        [Authorize(Roles ="Create Role, Admin Role")]
        public IActionResult DiagnosisForm()
        {
            _listResolver.PopulateDiagnosisCategoriesDropDownList();
            _listResolver.PopulateDiagnosisTypeDropDownList();
            return PartialView();
        }

        [Authorize(Roles = "Create Role, Admin Role")]
        public IActionResult DrugForm()
        {
            ViewBag.DrugId = _listResolver.DrugsDropDownList();
            ViewBag.SideEffects = _listResolver.PopulateSideEffectsDropDownList(null);
            return PartialView();
        }

        [Authorize(Roles = "Create Role, Admin Role")]
        public IActionResult STGForm()
        {
            return PartialView();
        }

        [Authorize(Roles = "Create Role, Admin Role")]
        public IActionResult IgForm()
        {
            ViewBag.ImmunoglobulinTypeId = _listResolver.ImmunoglobinTypesDropdownList();
            return PartialView();
        }

        [Authorize(Roles = "Create Role, Admin Role")]
        public IActionResult RadiologyForm()
        {

            ViewBag.RadiologyTypeId = _listResolver.PopulateRadiologyDropdownList("RadiologyType");
            ViewBag.ChestLocationId = _listResolver.PopulateRadiologyDropdownList("ChestLocation");
            ViewBag.ChestDistributionId = _listResolver.PopulateRadiologyDropdownList("ChestDistribution");
            ViewBag.GradeId = _listResolver.PopulateRadiologyDropdownList("Grade");
            ViewBag.TreatmentResponseId = _listResolver.PopulateRadiologyDropdownList("TreatmentResponse");
            ViewBag.FindingId = _listResolver.PopulateRadiologyDropdownList("Finding");
            return PartialView();
        }

        [Authorize(Roles = "Update Role, Admin Role")]
        public IActionResult EditDiagnosisForm()
        {
            _listResolver.PopulateDiagnosisCategoriesDropDownList();
            _listResolver.PopulateDiagnosisTypeDropDownList();
            ViewBag.Index = (string)Request.Query["index"];
            return PartialView();
        }

        [Authorize(Roles = "Update Role, Admin Role")]
        public IActionResult EditDrugForm()
        {
            ViewBag.DrugId = _listResolver.DrugsDropDownList();
            ViewBag.SideEffects = _listResolver.PopulateSideEffectsDropDownList(null);
            ViewBag.Index = (string)Request.Query["index"];
            return PartialView();
        }

        [Authorize(Roles = "Update Role, Admin Role")]
        public IActionResult EditSTGForm()
        {
            ViewBag.Index = (string)Request.Query["index"];
            return PartialView();
        }

        [Authorize(Roles = "Update Role, Admin Role")]
        public IActionResult EditIgForm()
        {
            ViewBag.Index = (string)Request.Query["index"];
            ViewBag.ImmunoglobulinTypeId = _listResolver.ImmunoglobinTypesDropdownList();
            return PartialView();
        }

        [Authorize(Roles = "Update Role, Admin Role")]
        public IActionResult EditRadiologyForm()
        {
            ViewBag.Index = (string)Request.Query["index"];
            ViewBag.RadiologyTypeId = _listResolver.PopulateRadiologyDropdownList("RadiologyType");
            ViewBag.ChestLocationId = _listResolver.PopulateRadiologyDropdownList("ChestLocation");
            ViewBag.ChestDistributionId = _listResolver.PopulateRadiologyDropdownList("ChestDistribution");
            ViewBag.GradeId = _listResolver.PopulateRadiologyDropdownList("Grade");
            ViewBag.TreatmentResponseId = _listResolver.PopulateRadiologyDropdownList("TreatmentResponse");
            ViewBag.FindingId = _listResolver.PopulateRadiologyDropdownList("Finding");
            return PartialView();
        }

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

        public IActionResult SearchCriteria()
        {
            var searchVm = new PatientSearchViewModel();
            searchVm.Index = (string)Request.Query["index"];
            ViewBag.CriteriaItems = DropdownSearchHelper.CriteriaMatchesDropdownList(Request.Query["fieldType"]);
            return PartialView(searchVm);
        }

        public IActionResult SearchSelectPartial()
        {
            string klass = Request.Query["klass"];
            ViewBag.Index = (string)Request.Query["index"];
            switch (klass)
            {
                case "DrugId":
                    ViewBag.SearchSelect = ViewBag.DrugId = _listResolver.DrugsDropDownList();
                    break;
                case "DiagnosisTypeId":
                    _listResolver.PopulateDiagnosisTypeDropDownList();
                    break;
                case "DiagnosisCategoryId":
                    _listResolver.PopulateDiagnosisCategoriesDropDownList();
                    break;
                case "PatientStatusId":
                    _listResolver.PopulatePatientStatusesDropDownList();
                    break;
            }
            return PartialView();
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