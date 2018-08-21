using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using AspergillosisEPR.Helpers;
using Microsoft.AspNetCore.Authorization;
using Audit.Mvc;
using AspergillosisEPR.Models.PatientViewModels;
using System;
using AspergillosisEPR.Lib;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using AspergillosisEPR.Lib.CaseReportForms;
using AspergillosisEPR.Models.CaseReportForms;
using AspergillosisEPR.Models.Patients;

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class PatientsController : Controller
    {
        private readonly AspergillosisContext _context;
        private PatientManager _patientManager;
        private DropdownListsResolver _listResolver;
        private CaseReportFormsDropdownResolver _caseReportFormListResolver;
        private CaseReportFormManager _caseReportFormManager;

        public PatientsController(AspergillosisContext context)
        {
            _patientManager = new PatientManager(context, Request);
            _context = context;
            _listResolver = new DropdownListsResolver(context, ViewBag);
            _caseReportFormListResolver = new CaseReportFormsDropdownResolver(context);
            _caseReportFormManager = new CaseReportFormManager(context);

        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        [Authorize(Roles = ("Admin Role, Create Role"))]
        public IActionResult New()
        {
            _listResolver.PopulateDiagnosisCategoriesDropDownList();
            _listResolver.PopulateDiagnosisTypeDropDownList();
            _listResolver.PopulatePatientStatusesDropdownList();
            ViewBag.CaseReportForms = _caseReportFormListResolver
                                            .PopulateCRFGroupedCategoriesDropdownList();
            return PartialView();
        }

        [Authorize(Roles = ("Admin Role, Read Role"))]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = ("Admin Role, Create Role"))]
        [Audit(EventTypeName = "Patient::Create", IncludeHeaders = true, IncludeModel = true)]
        public IActionResult Create([Bind("LastName,FirstName,DOB,Gender, RM2Number, PatientStatusId, DateOfDeath, GenericNote")]
                                                 Patient patient,
                                                 PatientDiagnosis[] diagnoses,
                                                 PatientDrug[] drugs,
                                                 PatientSTGQuestionnaire[] sTGQuestionnaires,
                                                 PatientImmunoglobulin[] patientImmunoglobulin,
                                                 PatientRadiologyFinding[] patientRadiologyFinding,
                                                 PatientMedicalTrial[] patientMedicalTrial,
                                                 PatientDrugLevel[] drugLevels,
                                                 PatientSurgery[] surgeries,
                                                 PatientAllergicIntoleranceItem[] allergies, 
                                                 CaseReportFormResult[] caseReportFormResult)
        {
            var existingPatient = _context.Patients.FirstOrDefault(x => x.RM2Number == patient.RM2Number);
            patient.CaseReportFormResults = new List<CaseReportFormResult>();
            _patientManager.Request = Request;
            CheckIsUnique(existingPatient);
            if (caseReportFormResult != null && caseReportFormResult.Length > 0 &&  caseReportFormResult[0].Results != null)
            {
                var results = caseReportFormResult[0].Results.ToArray();
                _caseReportFormManager.CreateCaseReportFormForResults(patient, results);
                patient.CaseReportFormResults.Add(caseReportFormResult[0]);
                _caseReportFormManager.LockForm(caseReportFormResult[0].CaseReportFormId);
            }

            _patientManager.AddCollectionsFromFormToPatients(patient, 
                                                             ref diagnoses, 
                                                             ref drugs,
                                                             ref sTGQuestionnaires, 
                                                             patientImmunoglobulin,
                                                             ref patientRadiologyFinding);
            _patientManager.AddMedicalTrials(patient, patientMedicalTrial);
            _patientManager.AddDrugLevels(patient, drugLevels);
            _patientManager.AddPatientSurgeries(patient, surgeries);
            _patientManager.AddPatientAllergiesIntolerances(patient, allergies);
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(patient);
                    _context.SaveChanges();
                    return Json(new { result = "ok" });
                }
                else
                {
                    Hashtable errors = ModelStateHelper.Errors(ModelState);
                    return Json(new { success = false, errors });
                }
            }
            catch (DbUpdateException ex)
            {
                return null;
            }
        }       

        [Authorize(Roles = ("Admin Role, Read Role"))]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _patientManager.FindPatientWithRelationsByIdAsync(id);
            LoadReleatedMedicalTrials(patient);
            LoadRelatedDrugLevels(patient);
            if (patient == null)
            {
                return NotFound();
            }

            var patientDetailsViewModel = PatientDetailsViewModel
                                                .BuildPatientViewModel(_context, patient, _caseReportFormManager);

            return PartialView(patientDetailsViewModel);
        }

        [Authorize(Roles = ("Admin Role, Read Role"))]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> PdfDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _patientManager.FindPatientWithRelationsByIdAsync(id);
            LoadReleatedMedicalTrials(patient);
            if (patient == null)
            {
                return NotFound();
            }

            var patientDetailsViewModel = PatientDetailsViewModel.BuildPatientViewModel(_context, patient, _caseReportFormManager);
            return View(patientDetailsViewModel);
        }

        [Authorize(Roles = ("Admin Role, Update Role"))]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Patient patient = await _patientManager.FindPatientWithRelationsByIdAsync(id);
            LoadReleatedMedicalTrials(patient);
            LoadRelatedDrugLevels(patient);
           
            if (patient == null)
            {
                return NotFound();
            }
            _listResolver.BindSelects(patient);
            _listResolver.BindMedicalTrialsSelects(ViewBag, patient);
            _listResolver.BindDrugLevelSelects(ViewBag, patient);
            _listResolver.BindSurgeriesSelects(ViewBag, patient);
            

            ViewBag.CaseReportForms = (List <IGrouping<string, CaseReportFormResult>>)  _caseReportFormManager
                                      .GetGroupedCaseReportFormsForPatient(patient.ID);
            return PartialView(patient);
        }

        [Authorize(Roles = ("Admin Role, Update Role"))]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(int? id, [Bind("ID,DiagnosisCategoryId,DiagnosisTypeId,Description")] PatientDiagnosis[] diagnoses,
                                                              [Bind("ID,DrugId,StartDate,EndDate")] PatientDrug[] drugs,
                                                              [Bind("ID, ActivityScore, SymptomScore, ImpactScore, TotalScore")] PatientSTGQuestionnaire[] sTGQuestionnaires,
                                                              [Bind("ID, DateTaken, Value, ImmunoglobulinTypeId")] PatientImmunoglobulin[] patientImmunoglobulines,
                                                              [Bind("ID, DateTaken, FindingId, RadiologyTypeId, ChestLocationId, ChestDistributionId, GradeId, TreatmentResponseId, Note")] PatientRadiologyFinding[] radiololgyFindings,
                                                              [Bind("ID, PatientId, MedicalTrialId, PatientMedicalTrialStatusId, IdentifiedDate, ConsentedDate, RecruitedDate, Consented")] PatientMedicalTrial[] patientMedicalTrial,
                                                              [Bind("ID, PatientId, DrugId, UnitOfMeasurementId, DateTaken, DateReceived, ResultValue, ComparisionCharacter")] PatientDrugLevel[] drugLevels,
                                                              [Bind("ID, SurgeryId, PatientId, SurgeryDate, Note")] PatientSurgery[] surgeries,
                                                              [Bind("ID, AllergyIntoleranceItemType, AllergyIntoleranceItemId, IntoleranceType, Severity, Note")] PatientAllergicIntoleranceItem[] allergies,
                                                              CaseReportFormResult[] caseReportFormResult)
        {
            if (id == null)
            {
                return NotFound();
            }
            Patient patientToUpdate = await _patientManager
                                                     .FindPatientWithFirstLevelRelationsByIdAsync(id);

            _patientManager.UpdateDiagnoses(diagnoses, patientToUpdate);
            _patientManager.UpdateDrugs(drugs, patientToUpdate, Request);
            _patientManager.UpdateSGRQ(sTGQuestionnaires, patientToUpdate);
            _patientManager.UpdateImmunoglobines(patientImmunoglobulines, patientToUpdate);
            _patientManager.UpdatePatientRadiology(radiololgyFindings, patientToUpdate);
            _patientManager.UpdatePatientMedicalTrials(patientMedicalTrial, patientToUpdate);
            _patientManager.UpdatePatientDrugLevels(drugLevels, patientToUpdate);
            _patientManager.UpdatePatientSurgeries(surgeries, patientToUpdate);
            _patientManager.UpdatePatientAllergiesIntolerances(allergies, patientToUpdate, Request);

            _caseReportFormManager.UpdateCaseReportFormsForPatient(caseReportFormResult, patientToUpdate);

            _context.Entry(patientToUpdate).State = EntityState.Modified;
            if (await TryUpdateModelAsync<Patient>(patientToUpdate,
               "",
               p => p.FirstName, p => p.LastName, p => p.DOB, p => p.RM2Number,
               p => p.Gender, p => p.PatientStatusId, p => p.DateOfDeath, p => p.GenericNote))               
            {
                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    var message = ex.Message;
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            else
            {
                Hashtable errors = ModelStateHelper.Errors(ModelState);
                return Json(new { success = false, errors });
            }

            return Json(new { result = "ok" });
        }

        [AllowAnonymous]
        public JsonResult HasRM2Number(string RM2Number, int? Id, string initialRM2Number)
        {
            var validateName = _context.Patients.FirstOrDefault(x => x.RM2Number == RM2Number && x.ID != Id);
            if (validateName != null && initialRM2Number == "undefined")
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Admin Role, Delete Role"))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.
                Include(p => p.STGQuestionnaires).
                Include(p => p.PatientRadiologyFindings).
                SingleOrDefaultAsync(p => p.ID == id);
            if (patient.STGQuestionnaires != null)
            {
                _context.PatientSTGQuestionnaires.RemoveRange(patient.STGQuestionnaires);
            }
            if (patient.PatientRadiologyFindings != null)
            {
                _context.PatientRadiologyFindings.RemoveRange(patient.PatientRadiologyFindings);
            }
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }

        private void LoadReleatedMedicalTrials(Patient patient)
        {
            _context.Entry(patient).Collection(c => c.MedicalTrials).Load();
            foreach (var trial in patient.MedicalTrials)
            {
                _context.Entry(trial).Reference(t => t.MedicalTrial).Load();
                _context.Entry(trial).Reference(t => t.PatientMedicalTrialStatus).Load();
                var medicalTrial = trial.MedicalTrial;
                _context.Entry(medicalTrial).Reference(t => t.TrialStatus).Load();
            }
        }

        private void LoadRelatedDrugLevels(Patient patient)
        {
            _context.Entry(patient).Collection(c => c.DrugLevels).Load();
            foreach (var patientDrugLevel in patient.DrugLevels)
            {
                _context.Entry(patientDrugLevel).Reference<Drug>(t => t.Drug).Load();
                _context.Entry(patientDrugLevel).Reference<UnitOfMeasurement>(t => t.UnitOfMeasurement).Load();
            }
        }

        private void CheckIsUnique(Patient existingPatient)
        {
            if (existingPatient != null)
            {
                ModelState.AddModelError("RM2Number", "Patient with this identifier already exists in database");
            }
        }
    }
}