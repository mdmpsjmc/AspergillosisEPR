using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Models.SettingsViewModels;
using AspergillosisEPR.Extensions.Validations;
using System.Collections;
using AspergillosisEPR.Helpers;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Lib;
using AspergillosisEPR.Models;

namespace AspergillosisEPR.Controllers
{
    public class RadiologyController : Controller
    {
        private AspergillosisContext _context;
        private RadiologyModelResolver _modelResolver;

        public RadiologyController(AspergillosisContext context)
        {
            _context = context;            
        }

        [Authorize(Roles = "Admin Role, Create Role")]
        public IActionResult New()
        {
            return PartialView(new RadiologyViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Create Role")]
        public async Task<IActionResult> Create([Bind("Name")] RadiologyViewModel radiologyViewModel)
        {
            try
            {
                string klassName = Request.Query["klass"];
                _modelResolver = new RadiologyModelResolver(klassName, radiologyViewModel.Name);
                dynamic modelToSave = _modelResolver.Resolve();

                this.CheckFieldUniqueness(_context.Findings, "Name", radiologyViewModel.Name);
                if (ModelState.IsValid)
                {
                    
                    _context.Add(modelToSave);
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