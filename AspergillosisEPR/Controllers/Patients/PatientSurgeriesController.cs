using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AspergillosisEPR.Controllers.Patients
{
    [Route("Patients/{patientId:int}/Surgeries")]
    public class PatientSurgeriesController : PatientBaseController
    {
        public PatientSurgeriesController(AspergillosisContext context) : base(context)
        {
        }


        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Delete Role, Admin Role")]
        [ValidateAntiForgeryToken]
        [Route("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var surgery = _context.PatientSurgeries.Where(t => t.ID == id).FirstOrDefault();
            _context.PatientSurgeries.Remove(surgery);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }
    }
}