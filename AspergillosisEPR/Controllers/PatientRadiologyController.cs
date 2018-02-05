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
    public class PatientRadiologyController : Controller
    {
        private readonly AspergillosisContext _context;

        public PatientRadiologyController(AspergillosisContext context)
        {
            _context = context;
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Delete Role, Admin Role")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var radiology = await _context.PatientRadiologyFindings.SingleOrDefaultAsync(pd => pd.ID == id);
            _context.PatientRadiologyFindings.Remove(radiology);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }
    }
}