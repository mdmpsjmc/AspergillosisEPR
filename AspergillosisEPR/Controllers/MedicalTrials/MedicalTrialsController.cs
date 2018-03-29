using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Models;
using AspergillosisEPR.Extensions.Validations;
using System.Collections;
using AspergillosisEPR.Helpers;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models.MedicalTrials;
using AspergillosisEPR.Lib;

namespace AspergillosisEPR.Controllers.MedicalTrials
{
    public class MedicalTrialsController : Controller
    {
        private AspergillosisContext _context;
        private DropdownListsResolver _listResolver;

        public MedicalTrialsController(AspergillosisContext context)
        {
            _context = context;
            _listResolver = new DropdownListsResolver(context, ViewBag);
        }

        [Authorize(Roles = "Admin Role, Create Role")]
        public IActionResult New()
        {
            ViewBag.InvestigatorsIds = _listResolver.PopulatePrimaryInvestigatorDropdownList();
            ViewBag.MedicalTrialsTypeIds = _listResolver.PopulateMedicalTrialTypesDropdownList();
            ViewBag.MedicalTrialsStatusesIds = _listResolver.PopulateMedicalTrialStatusesDropdownList();
            return PartialView(@"/Views/MedicalTrials/MedicalTrials/New.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Create Role")]
        public async Task<IActionResult> Create([Bind("Name, Description")] MedicalTrial medicalTrial)
        {
            try
            {
                this.CheckFieldUniqueness(_context.MedicalTrials, "Name", medicalTrial.Name);

                if (ModelState.IsValid)
                {
                    _context.Add(medicalTrial);
                    await _context.SaveChangesAsync();
                    return Json(new { result = "ok" });
                }
                else
                {
                    Hashtable errors = ModelStateHelper.Errors(ModelState);
                    return Json(new { success = false, errors });
                }
            }
            catch (DbUpdateException)
            {
                return null;
            }
        }

        [Authorize(Roles = "Admin Role, Update Role")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalTrial = await _context.MedicalTrials
                                             .AsNoTracking()
                                             .SingleOrDefaultAsync(m => m.ID == id);
            if (medicalTrial == null)
            {
                return NotFound();
            }

            ViewBag.PrincipalInvestigatorsIds = _listResolver
                                                    .PopulatePrimaryInvestigatorDropdownList(medicalTrial.MedicalTrialPrincipalInvestigatorId);
            ViewBag.MedicalTrialsTypeIds = _listResolver
                                                    .PopulateMedicalTrialTypesDropdownList(medicalTrial.MedicalTrialTypeId);
            ViewBag.MedicalTrialsStatusesIds = _listResolver
                                                    .PopulateMedicalTrialStatusesDropdownList(medicalTrial.MedicalTrialStatusId);

            return PartialView(@"/Views/MedicalTrials/MedicalTrials/Edit.cshtml", medicalTrial);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Update Role")]
        public async Task<IActionResult> EditMedicalTrial(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalTrial = await _context.MedicalTrials
                                             .AsNoTracking()
                                             .SingleOrDefaultAsync(m => m.ID == id);
            medicalTrial.Name = Request.Form["Name"];
            medicalTrial.Description = Request.Form["Description"];
            if (TryValidateModel(medicalTrial))
            {
                try
                {
                    _context.MedicalTrials.Update(medicalTrial);
                    _context.SaveChanges();
                }
                catch (DbUpdateException /* ex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            else
            {
                Hashtable errors = ModelStateHelper.Errors(ModelState);
                return Json(new { success = false, errors });
            }

            return Json(new { result = "ok" });
        }


        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin Role, Delete Role")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicalTrial = await _context.MedicalTrials.SingleOrDefaultAsync(p => p.ID == id);
            _context.MedicalTrials.Remove(medicalTrial);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }
    }
}