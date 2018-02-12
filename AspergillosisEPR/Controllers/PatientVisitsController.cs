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
            SaveSGRQExaminationsFor(patientVisit);
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

    }
}