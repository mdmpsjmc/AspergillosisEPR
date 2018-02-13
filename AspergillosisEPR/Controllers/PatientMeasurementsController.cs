using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Models;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Authorization;

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
        
            if (ModelState.IsValid)
            {
                _context.Add(measurementExamination);
                _context.SaveChanges();
                
            }
            var patientMeasurments = _context.PatientMeasurements.
                                          Where(pm => pm.PatientId == patientId).
                                          OrderBy(pm => pm.DateTaken).
                                          ToList();
            return PartialView(patientMeasurments);
        }
    }
}