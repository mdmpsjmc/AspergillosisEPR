using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using AspergillosisEPR.Lib;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Models.MedicalTrials;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using AspergillosisEPR.Helpers;
using AspergillosisEPR.Models.MedicalTrials;

namespace AspergillosisEPR.Controllers.investigators
{
    public class PrincipalInvestigatorsController : Controller
    {
        private AspergillosisContext _context;
        private DropdownListsResolver _listResolver;

        public PrincipalInvestigatorsController(AspergillosisContext context)
        {
            _context = context;
            _listResolver = new DropdownListsResolver(context, ViewBag);
        }

        [Authorize(Roles = "Admin Role, Create Role")]
        public IActionResult New()
        {
            ViewBag.PersonTitlesIds = _listResolver.PopulatePersonTitlesDropdownList();
            return PartialView(@"/Views/MedicalTrials/PrincipalInvestigators/New.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Create Role")]
        public async Task<IActionResult> Create([Bind("FirstName, LastName, PersonTitleId")] MedicalTrialPrincipalInvestigator investigator)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(investigator);
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

            var investigator = await _context.MedicalTrialsPrincipalInvestigators
                                             .AsNoTracking()
                                             .SingleOrDefaultAsync(m => m.ID == id);
            if (investigator == null)
            {
                return NotFound();
            }
            return PartialView(@"/Views/MedicalTrials/PrincipalInvestigators/Edit.cshtml", investigator);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Update Role")]
        public async Task<IActionResult> EditPrincipalInvestigator(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var investigator = await _context.MedicalTrialsPrincipalInvestigators
                                             .AsNoTracking()
                                             .SingleOrDefaultAsync(m => m.ID == id);

            investigator.FirstName = Request.Form["Name"];
            investigator.LastName = Request.Form["Description"];
            int personTitleId = string.IsNullOrEmpty(Request.Form["PersonTitleId"]) ? 0 : Int32.Parse(Request.Form["PersonTitleId"]);
            investigator.PersonTitleId = personTitleId;

            if (TryValidateModel(investigator))
            {
                try
                {
                    _context.MedicalTrialsPrincipalInvestigators.Update(investigator);
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
            var investigator = await _context.MedicalTrialsPrincipalInvestigators
                                             .SingleOrDefaultAsync(p => p.ID == id);
            _context.MedicalTrialsPrincipalInvestigators.Remove(investigator);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }
    }
}
