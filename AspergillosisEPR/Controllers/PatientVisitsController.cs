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
            var list1 = SaveExamination("SGRQExamination", "PatientSTGQuestionnaireId", patientVisit);
            var list2 = SaveExamination("ImmunologyExamination", "PatientImmunoglobulinId", patientVisit);
            var list3 = SaveExamination("RadiologyExamination", "PatientRadiologyFinidingId", patientVisit);
            var list4 = SaveExamination("MeasurementExamination", "PatientMeasurementId", patientVisit);

            var concatenated = list1.Concat(list2).Concat(list3).Concat(list4);
            if (concatenated.Count() == 0)
            {
                ModelState.AddModelError("Base", "You need to select at least one item from the lists below");
            }
            if (ModelState.IsValid)
            {
                _context.Add(patientVisit);
                await _context.SaveChangesAsync();
            } else
            {
                Hashtable errors = ModelStateHelper.Errors(ModelState);
                return Json(new { success = false, errors });
            }
            return Json(new { result = "ok" });
        }

        public async Task<IActionResult> ExaminationsTabs(int patientId)
        {
            _patientManager = new PatientManager(_context);
            var patient = await _patientManager.FindPatientWithRelationsByIdAsync(patientId);
            if (patient == null)
            {
                return NotFound();
            }
            var patientMeasurements = _context.PatientMeasurements
                                              .Where(pm => pm.PatientId == patient.ID);

            var patientVM = new NewPatientVisitViewModel();
            patientVM.STGQuestionnaires = patient.STGQuestionnaires;
            patientVM.PatientRadiologyFindings = patient.PatientRadiologyFindings;
            patientVM.PatientImmunoglobulines = patient.PatientImmunoglobulines;
            if (patientMeasurements != null)
            {
                patientVM.PatientMeasurements = patientMeasurements.ToList();
            }
            return PartialView(patientVM);
        }

        public IActionResult Details(int id)
        {
            var patientDetailsVM = new PatientVisitDetailsViewModel();
            var patientVisit = _context.PatientVisits
                                       .Include(pv => pv.Patient)
                                       .Where(pv => pv.ID == id)
                                       .SingleOrDefault();

            if (patientVisit == null)
            {
                return NotFound();
            }

            var patientExaminations = _context.PatientExaminations
                                              .Where(pe => pe.PatientVisitId == id)
                                              .GroupBy(pe => pe.Discriminator)
                                              .ToList();
            LoadRelatedData(patientExaminations);
            patientDetailsVM.Patient = patientVisit.Patient;
            patientDetailsVM.VisitDate = patientVisit.VisitDate;
            patientDetailsVM.PatientExaminations = patientExaminations.ToList<dynamic>();
            //patientDetailsVM.PatientMeasurements = patientVisit.PatientMeasurements.ToList();
            return PartialView(patientDetailsVM);
        }

        private void LoadRelatedData(List<IGrouping<string, PatientExamination>> patientExaminations)
        {
            foreach (var group in patientExaminations)
            {
                foreach (PatientExamination examination in group)
                {
                    switch (examination.Discriminator)
                    {
                        case "MeasurementExamination":
                            var measurement = (MeasurementExamination)examination;
                            _context.Entry(measurement).Reference(m => m.PatientMeasurement).Load();
                            break;
                        case "SGRQExamination":
                            var sgrq = (SGRQExamination)examination;
                            _context.Entry(sgrq).Reference(m => m.PatientSTGQuestionnaire).Load();
                            break;
                        case "ImmunologyExamination":
                            var ig = (ImmunologyExamination)examination;
                            _context.Entry(ig).Reference(m => m.PatientImmunoglobulin).Load();
                            _context.Entry(ig.PatientImmunoglobulin).Reference(m => m.ImmunoglobulinType).Load();
                            break;
                        case "RadiologyExamination":
                            var rad = (RadiologyExamination)examination;
                            _context.Entry(rad).Reference(m => m.PatientRadiologyFiniding).Load();
                            _context.Entry(rad.PatientRadiologyFiniding).Reference(m => m.Grade).Load();
                            _context.Entry(rad.PatientRadiologyFiniding).Reference(m => m.RadiologyType).Load();
                            _context.Entry(rad.PatientRadiologyFiniding).Reference(m => m.TreatmentResponse).Load();
                            _context.Entry(rad.PatientRadiologyFiniding).Reference(m => m.ChestDistribution).Load();
                            _context.Entry(rad.PatientRadiologyFiniding).Reference(m => m.ChestLocation).Load();
                            _context.Entry(rad.PatientRadiologyFiniding).Reference(m => m.Finding).Load();
                            break;
                    }
                }
            }
        }

        private List<PatientExamination> SaveExamination(string klass, string propertyName, PatientVisit patientVisit)
        {
            var selected = Request.Form.Keys.Where(k => k.Contains(klass)).ToList();
            var savedItems = new List<PatientExamination>();
            for (var cursor = 0; cursor < selected.Count; cursor++)
            {
                var itemId = Request.Form[klass + "[" + cursor + "].ID"];
                var isChecked = Request.Form[klass + "[" + cursor + "].Selected"];
                if (isChecked == "on")
                {
                    Type examinationType = Type.GetType("AspergillosisEPR.Models." + klass);
                    var examination = (PatientExamination)Activator.CreateInstance(examinationType);
                    examination.PatientVisit = patientVisit;
                    PropertyInfo property = examination.GetType().GetProperty(propertyName);
                    property.SetValue(examination, Int32.Parse(itemId));
                    examination.PatientVisitId = patientVisit.ID;
                    _context.PatientExaminations.Add(examination);
                    savedItems.Add(examination);
                }
            }
            return savedItems;
        }


    }
}