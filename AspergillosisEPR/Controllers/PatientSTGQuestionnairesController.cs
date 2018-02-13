using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Models;
using AspergillosisEPR.Helpers;
using System.Collections;
using System.Linq;

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

        [Authorize(Roles = ("Admin Role, Create Role"))]
        [HttpPost]
        public IActionResult Create(int patientId, PatientSTGQuestionnaire sgrq)
        {
            var patient = _context.Patients.Where(p => p.ID == patientId).SingleOrDefault();
            if (patient == null)
            {
                return NotFound();
            }

            sgrq.PatientId = patient.ID;            
            if (ModelState.IsValid)
            {
                _context.Add(sgrq);
                _context.SaveChanges();

            }
            else
            {
                Hashtable errors = ModelStateHelper.Errors(ModelState);
                return StatusCode(422, Json(new { success = false, errors }));
            }
            var questionnaires = _context.PatientSTGQuestionnaires.
                                          Where(pm => pm.PatientId == patientId).
                                          OrderByDescending(pm => pm.DateTaken).
                                          ToList();
            return PartialView(questionnaires);
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