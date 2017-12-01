using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Models.SettingsViewModels;
using AspergillosisEPR.Models;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Helpers;
using System.Collections;
using AspergillosisEPR.Data;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class PatientDrugsController : Controller
    {
        private readonly AspergillosisContext _context;

        public PatientDrugsController(AspergillosisContext context)
        {
            _context = context;
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientDrug = await _context.PatientDrugs.SingleOrDefaultAsync(pd => pd.ID == id);
            _context.PatientDrugSideEffects.RemoveRange(_context.PatientDrugSideEffects.Where(pdse => pdse.PatientDrugId == id));
            _context.PatientDrugs.Remove(patientDrug);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }
    }
}