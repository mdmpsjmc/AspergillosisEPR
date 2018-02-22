using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using AspergillosisEPR.Lib;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.PatientViewModels;
using AspergillosisEPR.Helpers;
using System.Collections;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace AspergillosisEPR.Controllers
{
    public class PatientVisitsController : Controller
    {
        private PatientManager _patientManager;
        private PatientVisitManager _patientVisitManager;
        private AspergillosisContext _context;

        public PatientVisitsController(AspergillosisContext context)
        {
            _context = context;
            _patientManager = new PatientManager(_context);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult New()
        {
            return PartialView();
        }

        [Authorize(Roles = ("Admin Role, Create Role"))]
        [HttpPost]
        public async Task<IActionResult> Create(PatientVisit patientVisit)
        {
            _patientVisitManager = new PatientVisitManager(_context, ViewBag, Request.Form);

            InitViewBags();

            var itemsToSave = _patientVisitManager.SavePatientExaminationsForVisit(patientVisit);

            if (itemsToSave.Count() == 0)
            {
                ModelState.AddModelError("Base", "You need to select at least one item from the lists below");
            }
            if (ModelState.IsValid)
            {
                _context.Add(patientVisit);
                _context.PatientExaminations.AddRange(itemsToSave);
                await _context.SaveChangesAsync();
            }
            else
            {
                Hashtable errors = ModelStateHelper.Errors(ModelState);
                return Json(new { success = false, errors });
            }
            return Json(new { result = "ok" });
        }

        public IActionResult Details(int id)
        {
            _patientVisitManager = new PatientVisitManager(_context, ViewBag);
            var patientDetailsVM = new PatientVisitDetailsViewModel();
            var patientVisit = _patientVisitManager.GetPatientVisitById(id);

            if (patientVisit == null)
            {
                return NotFound();
            }

            var patientExaminations = _patientVisitManager.GetPatientExaminationsForVisitWithRelatedData(id);
            var otherVisits = _patientVisitManager.GetVisitsWithRelatedDataExcluding(patientVisit);

            patientDetailsVM.Patient = patientVisit.Patient;
            patientDetailsVM.VisitDate = patientVisit.VisitDate;
            patientDetailsVM.PatientExaminations = patientExaminations;
            patientDetailsVM.OtherVisits = otherVisits;

            return PartialView(patientDetailsVM);
        }

        [Authorize(Roles = ("Admin Role, Edit Role"))]
        public IActionResult Edit(int? id)
        {
            _patientVisitManager = new PatientVisitManager(_context, ViewBag);
            var patientVisit = _patientVisitManager.GetPatientVisitById(id);

            if (patientVisit == null)
            {
                return NotFound();
            }
            NewPatientVisitViewModel patientVM;
            List<IGrouping<string, PatientExamination>> patientExaminations;
            GetPatientExaminationsWithVisitViewModel(id.Value, patientVisit, out patientVM, out patientExaminations);
            return PartialView(patientVM);
        }

        [HttpPost]
        [ActionName("Edit")]
        public IActionResult EditPatientVisit(int id)
        {
            _patientVisitManager = new PatientVisitManager(_context, ViewBag, Request.Form);
            var patientVisit = _patientVisitManager.GetPatientVisitById(id);

            NewPatientVisitViewModel patientVM;
            List<IGrouping<string, PatientExamination>> patientExaminations;
            GetPatientExaminationsWithVisitViewModel(id, patientVisit, out patientVM, out patientExaminations);

            _patientVisitManager.UpdateSelectedItemsForPatientVisit(patientVisit);                           

            if (TryValidateModel(patientVisit))
            {
                _context.SaveChanges();
                return Json("oK");
            } else
            {
                return Json("oK");
            }            
        }        

        public async Task<IActionResult> ExaminationsTabs(int patientId)
        {
            _patientVisitManager = new PatientVisitManager(_context, ViewBag);
            var patientVM = await BuildPatientVisitVM(patientId);
            InitViewBags();
            return PartialView(patientVM);
        }

        private void GetPatientExaminationsWithVisitViewModel(int id, PatientVisit patientVisit,             
            out NewPatientVisitViewModel patientVM, out List<IGrouping<string, PatientExamination>> patientExaminations)
        {
            patientVM = BuildPatientVisitVM(patientVisit.PatientId, patientVisit.VisitDate).Result;
            patientVM.Patient = _context.Patients.Where(p => p.ID == patientVisit.PatientId).SingleOrDefault();
            patientExaminations = _patientVisitManager.GetPatientExaminationsForVisitWithRelatedData(id);
            SelectObjectsForVisit(patientVM, patientExaminations);
        }

        private void SelectObjectsForVisit(NewPatientVisitViewModel patientVM, 
                List<IGrouping<string, PatientExamination>> patientExaminations)
        {
            InitViewBags();
            foreach (var group in patientExaminations)
            {
                foreach (PatientExamination examination in group)
                {
                    switch (examination.Discriminator)
                    {
                        case "MeasurementExamination":
                            var measurement = (MeasurementExamination)examination;
                            ViewBag.SelectedMeasurements.Add(examination.PatientMeasurementId);
                            break;
                        case "SGRQExamination":
                            var sgrq = (SGRQExamination)examination;
                            ViewBag.SelectedSGRQ.Add(examination.PatientSTGQuestionnaireId);
                            break;
                        case "ImmunologyExamination":
                            var ig = (ImmunologyExamination)examination;
                            ViewBag.SelectedIg.Add(examination.PatientImmunoglobulinId);
                            break;
                        case "RadiologyExamination":
                            var rad = (RadiologyExamination)examination;
                            ViewBag.SelectedRadiology.Add(examination.PatientRadiologyFinidingId);
                            break;
                    }
                }
            }
        }

        private void InitViewBags()
        {
            ViewBag.SelectedMeasurements = new List<int>();
            ViewBag.SelectedSGRQ = new List<int>();
            ViewBag.SelectedIg = new List<int>();
            ViewBag.SelectedRadiology = new List<int>();
        }

        private async Task<NewPatientVisitViewModel> BuildPatientVisitVM(int patientId, object visitDate = null)
        {
            _patientManager = new PatientManager(_context);
            var patient = await _patientManager.FindPatientWithRelationsByIdAsync(patientId);
            if (patient == null)
            {
                return null;
            }
            var patientMeasurements = _context.PatientMeasurements
                                              .Where(pm => pm.PatientId == patient.ID);

            var patientVM = new NewPatientVisitViewModel();
            if (visitDate != null)
            {
                DateTime patientVisitDate = (DateTime)visitDate;
                patientVM.VisitDate = DateHelper.DateTimeToUnixTimestamp(patientVisitDate).ToString();
            }
            patientVM.PatientId = patient.ID;
            patientVM.STGQuestionnaires = patient.STGQuestionnaires;
            patientVM.PatientRadiologyFindings = patient.PatientRadiologyFindings;
            patientVM.PatientImmunoglobulines = patient.PatientImmunoglobulines;
            if (patientMeasurements != null)
            {
                patientVM.PatientMeasurements = patientMeasurements.ToList();
            }
            return patientVM;
        }        
    }
}