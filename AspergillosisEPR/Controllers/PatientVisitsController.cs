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
using System.Globalization;
using AspergillosisEPR.Extensions.Validations;
using AspergillosisEPR.Models.Patients;

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

            this.CheckIfAtLeastOneIsSelected(itemsToSave.Count);

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

            var patientVisit = _patientVisitManager.GetPatientVisitById(id);

            if (patientVisit == null)
            {
                return NotFound();
            }

            var patientDetailsVM = PatientVisitDetailsViewModel.
                                        BuildPatientVisitDetailsVM(_patientVisitManager, patientVisit);
            ViewBag.ShowButtons = true;
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

            this.CheckVisitDate(patientVisit);

            NewPatientVisitViewModel patientVM;
            List<IGrouping<string, PatientExamination>> patientExaminations;
            GetPatientExaminationsWithVisitViewModel(id, patientVisit, out patientVM, out patientExaminations);

            var selectedItems = _patientVisitManager.UpdateSelectedItemsForPatientVisit(patientVisit);
            this.CheckIfAtLeastOneIsSelected(selectedItems.Count);

            if (TryValidateModel(patientVisit))
            {
                _context.SaveChanges();
                return Json("ok");
            }
            else
            {
                Hashtable errors = ModelStateHelper.Errors(ModelState);
                return Json(new { success = false, errors });
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        [Authorize(Roles = "Delete Role, Admin Role")]
        public async Task<IActionResult> DeleteConfirmedAsync(int id)
        {
            var patientVisit = await _context.PatientVisits.SingleOrDefaultAsync(pv => pv.ID == id);
            _context.PatientVisits.Remove(patientVisit);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }

        public async Task<IActionResult> ExaminationsTabs(int patientId)
        {
            _patientVisitManager = new PatientVisitManager(_context, ViewBag);
            var patientVM = await NewPatientVisitViewModel.BuildPatientVisitVM(_context, patientId);
            InitViewBags();
            return PartialView(patientVM);
        }

        private void GetPatientExaminationsWithVisitViewModel(int id, 
                                                              PatientVisit patientVisit,             
                                                              out NewPatientVisitViewModel patientVM, 
                                                              out List<IGrouping<string, PatientExamination>> patientExaminations)
        {
            patientVM = NewPatientVisitViewModel.BuildPatientVisitVM(_context, patientVisit.PatientId, patientVisit.VisitDate).Result;
            patientVM.Patient = _context.Patients.Where(p => p.ID == patientVisit.PatientId).SingleOrDefault();
            patientExaminations = _patientVisitManager.GetPatientExaminationsForVisitWithRelatedData(patientVisit.ID);
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
    }
}