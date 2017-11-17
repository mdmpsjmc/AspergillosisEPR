using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.EntityFrameworkCore;


namespace AspergillosisEPR.Controllers
{
    public class PatientDiagnosesController : Controller
    {
        private readonly AspergillosisContext _context;

        public PatientDiagnosesController(AspergillosisContext context)
        {

            _context = context;
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientDiagnosis = await _context.PatientDiagnoses.SingleOrDefaultAsync(pd => pd.ID == id);
            _context.PatientDiagnoses.Remove(patientDiagnosis);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }
    }
}