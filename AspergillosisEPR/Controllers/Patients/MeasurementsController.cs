using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models.PatientViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AspergillosisEPR.Controllers.Patients
{
    public class MeasurementsController : PatientBaseController
    {
        public MeasurementsController(AspergillosisContext context) : base(context)
        {
            _context = context;
        }       

        [Route("Measurements/Edit")]
        [HttpGet]
        public IActionResult Edit(int patientId)
        {

            var patient = _context.Patients
                                  .Where(p => p.ID == patientId)
                                  .Include(p => p.PatientMeasurements)
                                  .FirstOrDefault();
            var measurements = patient.PatientMeasurements.OrderByDescending(q => q.DateTaken).ToList();
          
            var patientWithMeasurements = new PatientWithMeasurements();
             if (measurements.FirstOrDefault() != null)
              {
                var firstMeasurement = measurements[0];
                patientWithMeasurements.Height = Convert.ToInt32(firstMeasurement.Height);
            }
      patientWithMeasurements.Patient = patient;
            patientWithMeasurements.Measurements = measurements;
            return PartialView("~/Views/Patients/Measurements/_Edit.cshtml", patientWithMeasurements);
        }


        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Delete Role, Admin Role")]
        [ValidateAntiForgeryToken]
        [Route("measurements/delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var measurement = _context.PatientMeasurements.Where(t => t.ID == id).FirstOrDefault();
            _context.PatientMeasurements.Remove(measurement);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }
    }
}