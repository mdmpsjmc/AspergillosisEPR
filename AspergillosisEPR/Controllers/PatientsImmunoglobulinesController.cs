using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AspergillosisEPR.Controllers
{
    public class PatientsImmunoglobulinesController : Controller
    {
        private readonly AspergillosisContext _context;

        public PatientsImmunoglobulinesController(AspergillosisContext context)
        {
            _context = context;
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Delete Role, Admin Role")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ig = await _context.PatientImmunoglobulins.SingleOrDefaultAsync(pd => pd.ID == id);
            _context.PatientImmunoglobulins.Remove(ig);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }
    }
}