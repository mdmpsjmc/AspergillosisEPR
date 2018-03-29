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

namespace AspergillosisEPR.Controllers.MedicalTrials
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



    }
}
