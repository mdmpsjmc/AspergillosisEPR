using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models;
using System.Collections;
using AspergillosisEPR.Helpers;
using AspergillosisEPR.Lib;

namespace AspergillosisEPR.Controllers
{
    public class PatientsImmunoglobulinesController : Controller
    {
        private readonly AspergillosisContext _context;
        private readonly DropdownListsResolver _listResolver;

        public PatientsImmunoglobulinesController(AspergillosisContext context)
        {
            _context = context;
            _listResolver = new DropdownListsResolver(context, ViewBag);

        }


        public IActionResult New()
        {
            ViewBag.ImmunoglobulinTypeId = _listResolver.ImmunoglobinTypesDropdownList();
            return PartialView();
        }

        [Authorize(Roles = ("Admin Role, Create Role"))]
        [HttpPost]
        public IActionResult Create(int patientId, PatientImmunoglobulin patientIg)
        {
            var patient = _context.Patients.Where(p => p.ID == patientId).SingleOrDefault();
            if (patient == null)
            {
                return NotFound();
            }

            patientIg.PatientId = patient.ID;
            if (ModelState.IsValid)
            {
                _context.Add(patientIg);
                _context.SaveChanges();

            }
            else
            {
                Hashtable errors = ModelStateHelper.Errors(ModelState);
                return StatusCode(422, Json(new { success = false, errors }));
            }
            var igs = _context.PatientImmunoglobulins.
                                          Include(pi => pi.ImmunoglobulinType).
                                          Where(pm => pm.PatientId == patientId).
                                          OrderByDescending(pm => pm.DateTaken).
                                          ToList();
            ViewBag.SelectedIg = new List<int>();
            return PartialView(igs);
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