using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class PatientSTGQuestionnairesController : Controller
    {
        private readonly AspergillosisContext _context;

        public PatientSTGQuestionnairesController(AspergillosisContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin Role, Create Role")]
        public IActionResult New()
        {
            return PartialView();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Delete Role, Admin Role")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientDiagnosis = await _context.PatientSTGQuestionnaires.SingleOrDefaultAsync(pd => pd.ID == id);
            _context.PatientSTGQuestionnaires.Remove(patientDiagnosis);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }
    }
}