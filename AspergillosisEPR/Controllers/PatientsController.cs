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
                                                 PatientImmunoglobulin[] patientImmunoglobulin)
        {
            var existingPatient = _context.Patients.FirstOrDefault(x => x.RM2Number == patient.RM2Number);
            if (existingPatient != null)
            {
                ModelState.AddModelError("RM2Number", "Patient with this RM2 Number already exists in database");
            }
            sTGQuestionnaires = sTGQuestionnaires.Where(q => q != null).ToArray();
            diagnoses = diagnoses.Where(d => d != null).ToArray();
            drugs = drugs.Where(dr => dr != null).ToArray();

            patient.PatientDiagnoses = diagnoses;
            patient.PatientDrugs = drugs;
            patient.STGQuestionnaires = sTGQuestionnaires;
            patient.PatientImmunoglobulines = patientImmunoglobulin;
            
            for(var cursor = 0; cursor < Request.Form["Drugs.index"].ToList().Count; cursor++)
            {
                string stringIndex = Request.Form["Drugs.index"][cursor];
                string sideEffectsIds = Request.Form["Drugs[" + stringIndex + "].SideEffects"];
                var sideEffects = _context.SideEffects.Where(se => sideEffectsIds.Contains(se.ID.ToString()));
                foreach(var sideEffect in sideEffects)
                {
                    PatientDrugSideEffect drugSideEffect = new PatientDrugSideEffect();
                    drugSideEffect.PatientDrug = drugs[cursor];
                    drugSideEffect.SideEffect = sideEffect;
                    drugs[cursor].SideEffects.Add(drugSideEffect);
                }                
            }
            try
            {
                if (ModelState.IsValid)
                {                  
                    _context.Add(patient);
                    _context.SaveChanges();
                   return Json(new { result = "ok" });
                }else
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

            var patientDetailsViewModel = BuildPatientViewModel(patient);
            return PartialView(patientDetailsViewModel);
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
                SingleOrDefaultAsync(p => p.ID == id);
            if (patient.STGQuestionnaires != null)
            {
                _context.PatientSTGQuestionnaires.RemoveRange(patient.STGQuestionnaires);
            }
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }

        private PatientDetailsViewModel BuildPatientViewModel(Patient patient)
        {
            var primaryDiagnosis = _context.DiagnosisCategories.Where(dc => dc.CategoryName == "Primary").FirstOrDefault();
            var secondaryDiagnosis = _context.DiagnosisCategories.Where(dc => dc.CategoryName == "Secondary").FirstOrDefault();
            var otherDiagnosis = _context.DiagnosisCategories.Where(dc => dc.CategoryName == "Other").FirstOrDefault();
            var underlyingDiagnosis = _context.DiagnosisCategories.Where(dc => dc.CategoryName == "Underlying diagnosis").FirstOrDefault();
            var pastDiagnosis = _context.DiagnosisCategories.Where(dc => dc.CategoryName == "Past Diagnosis").FirstOrDefault();

            var patientDetailsViewModel = new PatientDetailsViewModel();

            patientDetailsViewModel.Patient = patient;

            if (primaryDiagnosis != null)
            {
                patientDetailsViewModel.PrimaryDiagnoses = patient.PatientDiagnoses.
                                                                    Where(pd => pd.DiagnosisCategoryId == primaryDiagnosis.ID).
                                                                    ToList();
            }
            if (secondaryDiagnosis != null)
            {
                patientDetailsViewModel.SecondaryDiagnoses = patient.PatientDiagnoses.
                                                                    Where(pd => pd.DiagnosisCategoryId == secondaryDiagnosis.ID).
                                                                    ToList();
            }
            if (otherDiagnosis != null)
            {
                patientDetailsViewModel.OtherDiagnoses = patient.PatientDiagnoses.
                                                                 Where(pd => pd.DiagnosisCategoryId == otherDiagnosis.ID).
                                                                 ToList();
            }
            if (underlyingDiagnosis != null)
            {
                patientDetailsViewModel.UnderlyingDiseases = patient.PatientDiagnoses.
                                                                    Where(pd => pd.DiagnosisCategoryId == underlyingDiagnosis.ID).
                                                                    ToList();
            }

            if (pastDiagnosis != null)
            {
                patientDetailsViewModel.PastDiagnoses = patient.PatientDiagnoses.
                                                                    Where(pd => pd.DiagnosisCategoryId == pastDiagnosis.ID).
                                                                    ToList();
            }
            patientDetailsViewModel.PatientDrugs = patient.PatientDrugs;
            patientDetailsViewModel.STGQuestionnaires = patient.STGQuestionnaires;
            patientDetailsViewModel.PatientImmunoglobulines = patient.PatientImmunoglobulines;
            return patientDetailsViewModel;
        }
    }
}