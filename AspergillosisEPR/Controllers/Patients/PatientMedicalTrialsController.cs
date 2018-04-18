using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AspergillosisEPR.Controllers.Patients
{
    public class PatientMedicalTrialsController : PatientBaseController
    {
        public PatientMedicalTrialsController(AspergillosisContext context) : base(context)
        {
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Delete Role, Admin Role")]
        [ValidateAntiForgeryToken]
        [Route("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var trial =  _context.PatientMedicalTrials.Where(t => t.ID == id).FirstOrDefault();
            _context.PatientMedicalTrials.Remove(trial);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }
    }
}