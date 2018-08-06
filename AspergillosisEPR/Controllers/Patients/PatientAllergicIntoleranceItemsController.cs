using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AspergillosisEPR.Controllers.Patients
{
    [Route("Patients/{patientId:int}/PatientAllergicIntoleranceItems")]
    public class PatientAllergicIntoleranceItemsController : PatientBaseController
    {
        public PatientAllergicIntoleranceItemsController(AspergillosisContext context) : base(context)
        {
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Delete Role, Admin Role")]
        [ValidateAntiForgeryToken]
        [Route("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var allergicIntolerance = _context.PatientAllergicIntoleranceItems.Where(t => t.ID == id).FirstOrDefault();
            _context.PatientAllergicIntoleranceItems.Remove(allergicIntolerance);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }
    }
}