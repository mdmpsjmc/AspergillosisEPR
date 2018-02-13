using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Models;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Helpers;
using System.Collections;

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class PatientMeasurementsController : Controller
    {
        private AspergillosisContext _context;

        public PatientMeasurementsController(AspergillosisContext context)
        {
            _context = context;
        }

        public IActionResult New()
        {
            return PartialView();
        }

        [Authorize(Roles = ("Admin Role, Create Role"))]
        [HttpPost]
        public IActionResult Create(int patientId, PatientMeasurement measurementExamination)
        {
            var patient = _context.Patients.Where(p => p.ID == patientId).SingleOrDefault();
            if (patient == null)
            {
                return NotFound();
            }

            measurementExamination.PatientId = patient.ID;
            CheckIfAtLeastHeightOrWeightArePresent(measurementExamination);
            if (ModelState.IsValid)
            {
                _context.Add(measurementExamination);
                _context.SaveChanges();
                
            } else
            {
                Hashtable errors = ModelStateHelper.Errors(ModelState);
                return StatusCode(422, Json(new { success = false, errors }));
            }
            var patientMeasurments = _context.PatientMeasurements.
                                          Where(pm => pm.PatientId == patientId).
                                          OrderByDescending(pm => pm.DateTaken).
                                          ToList();
            return PartialView(patientMeasurments);
        }

        private void CheckIfAtLeastHeightOrWeightArePresent(PatientMeasurement measurementExamination)
        {
            if (measurementExamination.Weight == null && measurementExamination.Height == null)
            {
                ModelState.AddModelError("Error", "One of the fields - weight or height must be present");
            }
        }
    }
}