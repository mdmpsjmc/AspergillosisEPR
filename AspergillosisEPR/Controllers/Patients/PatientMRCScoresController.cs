using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models.PatientViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspergillosisEPR.Controllers.Patients
{
    public class PatientMRCScores : PatientBaseController
    {
        public PatientMRCScores(AspergillosisContext context) : base(context)
        {
            _context = context;
        }

        [Route("mrcscores/Edit")]
        [HttpGet]
        public IActionResult Edit(int patientId)
        {
          var patient = _context.Patients
                            .Where(p => p.ID == patientId)
                            .Include(p => p.PatientMRCScores)
                            .FirstOrDefault();
          var scores = patient.PatientMRCScores.OrderByDescending(q => q.DateTaken).ToList();

          var patientWithScores = new PatientWithScores();
          
          patientWithScores.Patient = patient;
          patientWithScores.Scores = scores;
          return PartialView("~/Views/Patients/MRCScores/_Edit.cshtml", patientWithScores);
        }


       [HttpPost, ActionName("Delete")]
       [Authorize(Roles = "Delete Role, Admin Role")]
       [ValidateAntiForgeryToken]
       [Route("mrcscores/delete/{id:int}")]
       public async Task<IActionResult> Delete(int id)
        {
          var score = _context.PatientMRCScores.Where(t => t.ID == id).FirstOrDefault();
          _context.PatientMRCScores.Remove(score);
          await _context.SaveChangesAsync();
          return Json(new { ok = "ok" });
        }

        [Route("MRCScores")]
        public IActionResult Details(int patientId)
        {
            var patient = _context.Patients
                                  .Where(p => p.ID == patientId)
                                  .Include(p => p.PatientMRCScores)                                    
                                  .FirstOrDefault();
            return PartialView("~/Views/Patients/MRCScores/_Details.cshtml", patient);
        }     
    }
}
