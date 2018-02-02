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

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class PatientsController : Controller
    {
        private readonly AspergillosisContext _context;
        private PatientManager _patientManager;
        private DropdownListsResolver _listResolver;


        public PatientsController(AspergillosisContext context)
        {
            _patientManager = new PatientManager(context);
            _context = context;
            _listResolver = new DropdownListsResolver(context, ViewBag);
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        [Authorize(Roles =("Admin Role, Create Role"))]
        public IActionResult New()
        {
            _listResolver.PopulateDiagnosisCategoriesDropDownList();
            _listResolver.PopulateDiagnosisTypeDropDownList();
            _listResolver.PopulatePatientStatusesDropdownList();
            _listResolver.PopulateRadiologyDropdownList("RadiologyType");
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
        public IActionResult Create([Bind("LastName,FirstName,DOB,Gender, RM2Number, PatientStatusId, DateOfDeath")] Patient patient,
                                                 PatientDiagnosis[] diagnoses,
                                                 PatientDrug[] drugs,
                                                 PatientSTGQuestionnaire[] sTGQuestionnaires,
                                                 PatientImmunoglobulin[] patientImmunoglobulin,
                                                 PatientRadiologyFinding[] patientRadiologyFinding)
        {
            var existingPatient = _context.Patients.FirstOrDefault(x => x.RM2Number == patient.RM2Number);
            if (existingPatient != null)
            {
                ModelState.AddModelError("RM2Number", "Patient with this RM2 Number already exists in database");
            }
            AddCollectionsFromFormToPatients(patient, ref diagnoses, ref drugs, 
                                                      ref sTGQuestionnaires, patientImmunoglobulin, 
                                                      ref patientRadiologyFinding);
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

            if (patient == null)
            {
                return NotFound();
            }

            var patientDetailsViewModel = PatientDetailsViewModel.BuildPatientViewModel(_context, patient);
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

            if (patient == null)
            {
                return NotFound();
            }

            var patientDetailsViewModel = PatientDetailsViewModel.BuildPatientViewModel(_context, patient);
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
            if (patient == null)
            {
                return NotFound();
            }
            _listResolver.BindSelects(patient);
            return PartialView(patient);
        }


        [Authorize(Roles = ("Admin Role, Update Role"))]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(int? id, [Bind("ID,DiagnosisCategoryId,DiagnosisTypeId,Description")] PatientDiagnosis[] diagnoses,
                                                              [Bind("ID,DrugId,StartDate,EndDate")] PatientDrug[] drugs,
                                                              [Bind("ID, ActivityScore, SymptomScore, ImpactScore, TotalScore")] PatientSTGQuestionnaire[] sTGQuestionnaires,
                                                              [Bind("ID, DateTaken, Value, ImmunoglobulinTypeId")] PatientImmunoglobulin[] patientImmunoglobulines)
        {
            if (id == null)
            {
                return NotFound();
            }
            Patient patientToUpdate = await _patientManager.FindPatientWithFirstLevelRelationsByIdAsync(id);

            _context.Entry(patientToUpdate).State = EntityState.Modified;

            _patientManager.UpdateDiagnoses(diagnoses, patientToUpdate);
            _patientManager.UpdateDrugs(drugs, patientToUpdate, Request);
            _patientManager.UpdateSGRQ(sTGQuestionnaires, patientToUpdate);
            _patientManager.UpdateImmunoglobines(patientImmunoglobulines, patientToUpdate);
            
            if (await TryUpdateModelAsync<Patient>(patientToUpdate,
                "",
                p => p.FirstName, p => p.LastName, p => p.DOB, p => p.RM2Number,
                p => p.Gender, p => p.PatientStatusId, p => p.DateOfDeath))
            {
                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException /* ex */)
                {
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

        private void AddSideEffectsToDrugs(PatientDrug[] drugs)
        {
            for (var cursor = 0; cursor < Request.Form["Drugs.index"].ToList().Count; cursor++)
            {
                string stringIndex = Request.Form["Drugs.index"][cursor];
                string sideEffectsIds = Request.Form["Drugs[" + stringIndex + "].SideEffects"];
                var sideEffects = _context.SideEffects.Where(se => sideEffectsIds.Contains(se.ID.ToString()));
                foreach (var sideEffect in sideEffects)
                {
                    PatientDrugSideEffect drugSideEffect = new PatientDrugSideEffect();
                    drugSideEffect.PatientDrug = drugs[cursor];
                    drugSideEffect.SideEffect = sideEffect;
                    drugs[cursor].SideEffects.Add(drugSideEffect);
                }
            }
        }

        private void AddCollectionsFromFormToPatients(Patient patient, ref PatientDiagnosis[] diagnoses, 
                                                                       ref PatientDrug[] drugs, ref PatientSTGQuestionnaire[] sTGQuestionnaires, 
                                                                       PatientImmunoglobulin[] patientImmunoglobulin, 
                                                                       ref PatientRadiologyFinding[] patientRadiologyFinding)
        {
            sTGQuestionnaires = sTGQuestionnaires.Where(q => q != null).ToArray();
            diagnoses = diagnoses.Where(d => d != null).ToArray();
            drugs = drugs.Where(dr => dr != null).ToArray();
            patientRadiologyFinding = patientRadiologyFinding.Where(rf => rf != null).ToArray();

            patient.PatientDiagnoses = diagnoses;
            patient.PatientDrugs = drugs;
            patient.STGQuestionnaires = sTGQuestionnaires;
            patient.PatientImmunoglobulines = patientImmunoglobulin;
            patient.PatientRadiologyFindings = patientRadiologyFinding;
            AddSideEffectsToDrugs(drugs);
        }
    }
}