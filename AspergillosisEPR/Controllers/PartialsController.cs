
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
using AspergillosisEPR.Lib.CaseReportForms;
using Microsoft.Extensions.Configuration;
using AspergillosisEPR.Extensions;
using AspergillosisEPR.Models.Patients;

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class PartialsController : Controller
    {
        private readonly AspergillosisContext _context;
        private readonly DropdownListsResolver _listResolver;
        private readonly CaseReportFormsDropdownResolver _caseReportFormListResolver;
        private readonly IConfiguration _configuration;

        public PartialsController(AspergillosisContext context, IConfiguration configuration)
        {
            _listResolver = new DropdownListsResolver(context, ViewBag);
            _caseReportFormListResolver = new CaseReportFormsDropdownResolver(context);
            _configuration = configuration;
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
        public IActionResult PulmonaryFunctionTest()
        {
            ViewBag.PulmonaryFunctionTestIds = _listResolver.PopulatePFTsDropDownList();
            return PartialView();
        }

        [Authorize(Roles = "Create Role, Admin Role")]
        public IActionResult WeightHeight()
        {
          return PartialView();
        }


        [Authorize(Roles = "Create Role, Admin Role")]
        public IActionResult MRCScore()
        {
          return PartialView();
        }

       [Authorize(Roles = "Update Role, Admin Role")]
        public IActionResult EditMRCScore()
        {
          ViewBag.PFTIds = _listResolver.PopulatePFTsDropDownList();
          ViewBag.Index = (string)Request.Query["index"];
          return PartialView();
        }

    [Authorize(Roles = "Update Role, Admin Role")]
        public IActionResult EditPulmonaryFunctionTest()
        {
            ViewBag.PFTIds = _listResolver.PopulatePFTsDropDownList();
            ViewBag.Index = (string)Request.Query["index"];
            return PartialView();
        }

        [Authorize(Roles = "Update Role, Admin Role")]
        public IActionResult EditWeightHeight()
        {
         ViewBag.Index = (string)Request.Query["index"];
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


        [Authorize(Roles = "Create Role, Admin Role")]
        public IActionResult SurgeryForm()
        {
            ViewBag.SurgeryId = _listResolver.PopulateSurgeryDropdownList();
            return PartialView();
        }

        [Authorize(Roles = "Update Role, Admin Role")]
        public IActionResult EditPatientSurgeryForm()
        {
            ViewBag.SurgeryId = _listResolver.PopulateSurgeryDropdownList();            
            ViewBag.Index = (string)Request.Query["index"];
            return PartialView();
        }

        [Authorize(Roles = "Update Role, Admin Role")]
        public IActionResult EditPatientAllergyIntoleranceForm()
        {
            ViewBag.DrugId = _listResolver.DrugsDropDownList();
            ViewBag.FoodId = _listResolver.FoodsDropdownList();
            ViewBag.SideEffects = _listResolver.PopulateSideEffectsDropDownList(null);
            ViewBag.Index = (string)Request.Query["index"];
            return PartialView();
        }


        [Authorize(Roles = "Create Role, Admin Role")]
        public IActionResult PatientAllergyIntoleranceForm()
        {
            ViewBag.DrugId = _listResolver.DrugsDropDownList();
            ViewBag.SideEffects = _listResolver.PopulateSideEffectsDropDownList(null);
            ViewBag.Index = (string)Request.Query["index"];
            return PartialView();
        }

        [Authorize(Roles = "Update Role, Admin Role")]
        public IActionResult ByName(string partialName)
        {
            ViewBag.Index = (string)Request.Query["index"];
            switch (partialName)
            {
                case "Drug":
                    ViewBag.ItemId = _listResolver.DrugsDropDownList();
                    break;
                case "Other":
                    ViewBag.ItemId = _listResolver.OtherAllergicItemList();
                    break;
                case "Food":
                    ViewBag.ItemId = _listResolver.FoodsDropdownList();
                    break;
                case "Fungi":
                    ViewBag.ItemId = _listResolver.FungiDropdownList();
                    break;
            }
            return PartialView(@"/Views/Partials/ByName/_"+partialName.FirstCharacterToUpper() + ".cshtml");
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
            Type type = Type.GetType("AspergillosisEPR.Models.Patients." + klassName);
            if (type == null)
            {
                type = Type.GetType("AspergillosisEPR.Models." + klassName);
            }
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
            ViewBag.renderGroupedSelect = false;
            string searchIfGreaterThan  = _configuration.GetSection("turnNativeDropdownSelectIntoSearchableWhenMoreThanItems").Value ;
            ViewBag.TurnIntoSearchableSelect = Int32.Parse(searchIfGreaterThan);
            string klass = Request.Query["klass"];
            ViewBag.Index = (string)Request.Query["index"];
            string field = Request.Query["field"];
            var groupedSelects = new string[] { "AllergyIntoleranceItemId" };
            if (groupedSelects.Contains(klass)) ViewBag.renderGroupedSelect = true;
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
                case "GradeId":
                case "TreatmentResponseId":
                case "ChestLocationId":
                case "ChestDistributionId":
                case "RadiologyTypeId":
                case "FindingId":
                    var radiologyCollection = klass.Replace("Id", String.Empty);
                    ViewBag.SearchSelect = _listResolver.PopulateRadiologyDropdownList(radiologyCollection);
                    break;
                case "MedicalTrialId":
                    ViewBag.SearchSelect = _listResolver.PouplateMedicalTrialsDropdownList();
                    break;
                case "PatientMedicalTrialStatusId":
                    ViewBag.SearchSelect = _listResolver.PopulatePatientMedicalTrialsStatusesDropdownList();
                    break;
                case "SurgeryId":
                    ViewBag.SearchSelect = _listResolver.PopulateSurgeryDropdownList();
                    break;
                case "Severity":
                    ViewBag.SearchSelect = new SelectList(PatientAllergicIntoleranceItem.Severities());
                    break;
                case "AllergyIntoleranceItemId":
                    ViewBag.SearchSelect = _listResolver.GroupedSelectForIntolerances();
                    break;
                case "SmokingStatusId":
                    ViewBag.SearchSelect = _listResolver.PopulateSmokingStatusesDropdownList();
                    break;
            }
            return PartialView();
        }

        public IActionResult CaseReportFormModal()
        {
            var gropupedCategoriesList = _caseReportFormListResolver
                                                    .PopulateCRFGroupedCategoriesDropdownList();
            ViewBag.CaseReportForms = gropupedCategoriesList;
            ViewBag.Index = (string)Request.Query["index"];
            return PartialView();
        }

        public IActionResult PatientMedicalTrialForm()
        {
            ViewBag.MedicalTrialsIds = _listResolver.PouplateMedicalTrialsDropdownList();
            ViewBag.PatientMedicalTrialStatusesIds = _listResolver.PopulatePatientMedicalTrialsStatusesDropdownList();
            return PartialView();
        }

        [Authorize(Roles = "Update Role, Admin Role")]
        public IActionResult EditPatientMedicalTrialForm()
        {
            ViewBag.Index = (string)Request.Query["index"];
            ViewBag.MedicalTrialsIds = _listResolver.PouplateMedicalTrialsDropdownList();
            ViewBag.PatientMedicalTrialStatusesIds = _listResolver.PopulatePatientMedicalTrialsStatusesDropdownList();
            return PartialView();
        }

        [Authorize(Roles = "Create Role, Admin Role")]
        public IActionResult DrugLevelForm()
        {
            ViewBag.Index = (string)Request.Query["index"];
            ViewBag.UnitId = _listResolver.PouplateUnitsDropdownList();
            ViewBag.DrugId = _listResolver.DrugsDropDownList();
            return PartialView();
        }

        [Authorize(Roles = "Update Role, Admin Role")]
        public IActionResult EditDrugLevelForm()
        {
            ViewBag.Index = (string)Request.Query["index"];
            ViewBag.UnitId = _listResolver.PouplateUnitsDropdownList();
            ViewBag.DrugId = _listResolver.DrugsDropDownList();
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