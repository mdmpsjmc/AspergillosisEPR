using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.EntityFrameworkCore;

namespace AspergillosisEPR.Controllers
{
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
            _context.PatientDrugs.Remove(patientDrug);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }
    }
}