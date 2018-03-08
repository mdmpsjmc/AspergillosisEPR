using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using AspergillosisEPR.Lib.CaseReportForms;
using AspergillosisEPR.Models.CaseReportForms.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Collections;
using AspergillosisEPR.Helpers;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models.CaseReportForms;

namespace AspergillosisEPR.Controllers.CaseReportForms
{
    public class CaseReportFormOptionGroupsController : Controller
    {
        private CaseReportFormsDropdownResolver _resolver;
        private AspergillosisContext _context;

        public CaseReportFormOptionGroupsController(AspergillosisContext context)
        {
            _context = context;
            _resolver = new CaseReportFormsDropdownResolver(context);
        }

        public IActionResult New(int id)
        {
            var viewModel = new CaseReportFormOptionGroupViewModel();
            viewModel.FormAction = "Create";
            return PartialView(@"~/Views/CaseReportForms/CaseReportFormOptionGroups/New.cshtml", viewModel);
        }

        public IActionResult Show(int id)
        {
            ViewBag.SectionOptions = _resolver.PopulateCRFOptionGroupChoicesDropdownList(id);
            return PartialView(@"~/Views/CaseReportForms/CaseReportFormOptionGroups/Show.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Create Role")]
        public async Task<IActionResult> Create(CaseReportFormOptionGroupViewModel optionGroupVM)
        {
            try
            {  
                if (ModelState.IsValid)
                {
                    var optionGroup = new CaseReportFormOptionGroup();
                    optionGroup.Name = optionGroupVM.Name;
                    optionGroup.Choices = new List<CaseReportFormOptionChoice>();
                    foreach (string choiceName in optionGroupVM.Options)
                    {
                        var optionChoice = new CaseReportFormOptionChoice();
                        optionChoice.Name = choiceName;
                        optionChoice.CaseReportFormOptionGroup = optionGroup;
                        optionGroup.Choices.Add(optionChoice);
                    }

                    _context.Add(optionGroup);
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
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foundItem = _context.CaseReportFormOptionGroups
                                    .Where(crfopg => crfopg.ID == id)
                                    .FirstOrDefault();
            var options = _context.CaseReportFormOptionChoices
                                  .Where(crogc => crogc.CaseReportFormOptionGroupId == foundItem.ID)
                                  .Select(crogc => crogc.Name)
                                  .ToArray();

            if (foundItem == null)
            {
                return NotFound();
            }
            var viewModel = new CaseReportFormOptionGroupViewModel();
            viewModel.FormAction = "Edit";
            viewModel.Name = foundItem.Name;
            viewModel.Options = options;
            return PartialView(@"~/Views/CaseReportForms/CaseReportFormOptionGroups/New.cshtml", viewModel);
        }

        [HttpPost, ActionName("Edit")]
        [Authorize(Roles = "Admin Role, Update Role")]
        [ValidateAntiForgeryToken]
        public IActionResult EditOptionGroupItem(int? id, CaseReportFormOptionGroupViewModel optionGroup)
        {
            if (id == null)
            {
                return NotFound();
            }
            var foundItem = _context.CaseReportFormOptionGroups
                                    .Include(crfog => crfog.Choices)    
                                    .Where(crfc => crfc.ID == id)
                                    .FirstOrDefault();

            if (foundItem == null)
            {
                return NotFound();
            }
            var optionsNames = optionGroup.Options;
            var dbOptionsChoices = new List<string>();
            var dbOptionsItems = new List<CaseReportFormOptionChoice>();
            foreach(string optionName in optionsNames)
            {
                var optionChoice = _context.CaseReportFormOptionChoices
                                           .Where(crfoc => crfoc.Name.Contains(optionName))
                                           .FirstOrDefault();
                if (optionChoice != null)
                {
                    dbOptionsChoices.Add(optionChoice.Name);
                    dbOptionsItems.Add(optionChoice);
                }
            }
            var toDeleteOptions = dbOptionsChoices.Except(optionsNames);
            var toInsertOptions = optionsNames.Except(dbOptionsChoices);
            
            optionGroup.ID = foundItem.ID;

            if (TryValidateModel(foundItem))
            {
                try
                {
                    _context.Update(foundItem);
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
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Delete Role")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var foundItem = _context.CaseReportFormOptionGroups
                                    .Where(crfog => crfog.ID == id)
                                    .FirstOrDefault();
            _context.Remove(foundItem);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }

    }
}