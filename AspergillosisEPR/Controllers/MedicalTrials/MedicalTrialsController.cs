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
        public async Task<IActionResult> Create(MedicalTrial medicalTrial)
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
        public async Task<IActionResult> Edit(int? id, MedicalTrial medicalTrial)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbMedicalTrial = await _context.MedicalTrials
                                               .AsNoTracking()
                                               .SingleOrDefaultAsync(m => m.ID == id);
            if (dbMedicalTrial == null)
            {
                return NotFound();
            }

            ViewBag.InvestigatorsIds = _listResolver
                                              .PopulatePrimaryInvestigatorDropdownList(dbMedicalTrial.MedicalTrialPrincipalInvestigatorId);
            ViewBag.MedicalTrialsTypeIds = _listResolver
                                              .PopulateMedicalTrialTypesDropdownList(dbMedicalTrial.MedicalTrialTypeId);
            ViewBag.MedicalTrialsStatusesIds = _listResolver
                                              .PopulateMedicalTrialStatusesDropdownList(dbMedicalTrial.MedicalTrialStatusId);

            return PartialView(@"/Views/MedicalTrials/MedicalTrials/Edit.cshtml", dbMedicalTrial);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Update Role")]
        public async Task<IActionResult> EditMedicalTrial(int? id, MedicalTrial medicalTrial)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbMedicalTrial = await _context.MedicalTrials
                                               .AsNoTracking()
                                               .SingleOrDefaultAsync(m => m.ID == id);

            UpdateMedicalTrial(dbMedicalTrial, medicalTrial);
            if (TryValidateModel(dbMedicalTrial))
            {
                try
                {
                    _context.MedicalTrials.Update(dbMedicalTrial);
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

        private void UpdateMedicalTrial(MedicalTrial dbMedicalTrial, 
                                        MedicalTrial medicalTrial)
        {
            dbMedicalTrial.Description = medicalTrial.Description;
            dbMedicalTrial.MedicalTrialPrincipalInvestigatorId = medicalTrial.MedicalTrialPrincipalInvestigatorId;
            dbMedicalTrial.MedicalTrialStatusId = medicalTrial.MedicalTrialStatusId;
            dbMedicalTrial.MedicalTrialTypeId = medicalTrial.MedicalTrialTypeId;
            dbMedicalTrial.Name = medicalTrial.Name;
            dbMedicalTrial.Number = medicalTrial.Number;
            dbMedicalTrial.RandDNumber = medicalTrial.RandDNumber;
            dbMedicalTrial.RECNumber = medicalTrial.RECNumber;
        }

    }
}