using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.SettingsViewModels;
using AspergillosisEPR.Lib;
using System.Collections;
using AspergillosisEPR.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class RadiologyResultsController : Controller
    {

        private AspergillosisContext _context;
        private DropdownListsResolver _listResolver;

        public RadiologyResultsController(AspergillosisContext context)
        {
            _context = context;
            _listResolver = new DropdownListsResolver(context, ViewBag);
        }

        [Authorize(Roles = "Admin Role, Create Role")]
        public IActionResult New()
        {
            ViewBag.RadiologyResultName = _listResolver.RadiologyResultDropdownList();
            ViewBag.RadiologyResultOptions = _listResolver.RadiologyResultOptionsDropdownList();
            return PartialView(new NewRadiologyResultViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Create Role")]
        public async Task<IActionResult> Create([Bind("Name, IsMultiple, RadiologyFindingSelectId, RadiologyFindingSelectOptionsIds")]
                                                 NewRadiologyResultViewModel newRadiologyResultViewModel)
        {
            try
            {                
                if (ModelState.IsValid)
                {
                    foreach(var id in newRadiologyResultViewModel.RadiologyFindingSelectOptionsIds)
                    {
                        var radiologyResult = new RadiologyResult();
                        radiologyResult.RadiologyFindingSelectOptionId = id;
                        radiologyResult.RadiologyFindingSelectId = newRadiologyResultViewModel.RadiologyFindingSelectId;
                        radiologyResult.IsMultiple = newRadiologyResultViewModel.IsMultiple;
                        _context.Add(radiologyResult);
                    }
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