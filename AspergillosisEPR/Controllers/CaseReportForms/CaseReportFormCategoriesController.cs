using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Models.CaseReportForms;
using AspergillosisEPR.Extensions.Validations;
using AspergillosisEPR.Data;
using AspergillosisEPR.Helpers;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models.CaseReportForms.ViewModels;

namespace AspergillosisEPR.Controllers.CaseReportForms
{
    public class CaseReportFormCategoriesController : Controller
    {
        private AspergillosisContext _context;

        public CaseReportFormCategoriesController(AspergillosisContext context)
        {
            _context = context;
        }

        public IActionResult New()
        {
            var viewModel = new CaseReportFormCategoryViewModel();
            viewModel.FormAction = "Create";
            return PartialView(@"~/Views/CaseReportForms/CaseReportFormCategories/New.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Create Role")]
        public async Task<IActionResult> Create(CaseReportFormCategory formCategory)
        {
            try
            {
                formCategory.Name = Request.Form["Name"];
                if (ModelState.IsValid)
                {
                    _context.Add(formCategory);
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

            var foundItem = _context.CaseReportFormCategories
                                    .Where(crfc => crfc.ID == id)
                                    .FirstOrDefault();

            if (foundItem == null)
            {
                return NotFound();
            }
            var viewModel = new CaseReportFormCategoryViewModel();
            viewModel.FormAction = "Edit";
            viewModel.Name = foundItem.Name;
            return PartialView(@"~/Views/CaseReportForms/CaseReportFormCategories/New.cshtml", viewModel);
        }

        [HttpPost, ActionName("Edit")]
        [Authorize(Roles = "Admin Role, Update Role")]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategoryItem(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var foundItem = _context.CaseReportFormCategories
                                  .Where(crfc => crfc.ID == id)
                                  .FirstOrDefault();

            if (foundItem == null)
            {
                return NotFound();
            }
            foundItem.Name = Request.Form["Name"];
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
            var foundItem = _context.CaseReportFormCategories
                                    .Where(crfc => crfc.ID == id)
                                    .FirstOrDefault();
            _context.Remove(foundItem);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }
    }
}