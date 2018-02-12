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
            SaveCollectionFor("SGRQExamination", "PatientSTGQuestionnaireId", patientVisit);
            SaveCollectionFor("ImmunologyExamination", "PatientImmunoglobulinId", patientVisit);
            SaveCollectionFor("RadiologyExamination", "PatientRadiologyFinidingId", patientVisit);
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
            var patientVM = new NewPatientVisitViewModel();
            patientVM.STGQuestionnaires = patient.STGQuestionnaires;
            patientVM.PatientRadiologyFindings = patient.PatientRadiologyFindings;
            patientVM.PatientImmunoglobulines = patient.PatientImmunoglobulines;
            return PartialView(patientVM);
        }

        private void SaveSGRQExaminationsFor(PatientVisit patientVisit)
        {
            var sgrqSelected = Request.Form.Keys.Where(k => k.Contains("SGRQ")).ToList();
            var sgrqExaminations = new List<SGRQExamination>();
            for (var sgCursor = 0; sgCursor < sgrqSelected.Count; sgCursor++)
            {
                var itemId = Request.Form["SGRQuestionnaire[" + sgCursor + "].ID"];
                var isChecked = Request.Form["SGRQuestionnaire[" + sgCursor + "].Selected"];
                if (isChecked == "on")
                {
                    var sgrqExamination = new SGRQExamination();
                    sgrqExamination.PatientVisit = patientVisit;
                    sgrqExamination.PatientSTGQuestionnaireId = Int32.Parse(itemId);
                    sgrqExamination.PatientVisitId = patientVisit.ID;
                    _context.SGRQExaminations.Add(sgrqExamination);
                }
            }
        }

        private void SaveImmunologyFor(PatientVisit patientVisit)
        {
            var igSelected = Request.Form.Keys.Where(k => k.Contains("Immunology")).ToList();
            for (var igCursor = 0; igCursor < igSelected.Count; igCursor++)
            {
                var itemId = Request.Form["Immunology[" + igCursor + "].ID"];
                var isChecked = Request.Form["Immunology[" + igCursor + "].Selected"];
                if (isChecked == "on")
                {
                    var igExamination = new ImmunologyExamination();
                    igExamination.PatientVisit = patientVisit;
                    igExamination.PatientSTGQuestionnaireId = Int32.Parse(itemId);
                    igExamination.PatientVisitId = patientVisit.ID;
                    _context.ImmunologyExaminations.Add(igExamination);
                }
            }
        }

        private void SaveCollectionFor(string klass, string propertyName, PatientVisit patientVisit)
        {
            var selected = Request.Form.Keys.Where(k => k.Contains(klass)).ToList();
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
                }
            }
        }


    }
}