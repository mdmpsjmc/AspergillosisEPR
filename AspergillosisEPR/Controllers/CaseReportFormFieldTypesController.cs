using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Models.CaseReportForms.ViewModels;
using AspergillosisEPR.Models.CaseReportForms;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Data;
using System.Collections;
using AspergillosisEPR.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AspergillosisEPR.Controllers
{
    public class CaseReportFormFieldTypesController : Controller
    {
        private AspergillosisContext _context;

        public CaseReportFormFieldTypesController(AspergillosisContext context)
        {
            _context = context;
        }

        public IActionResult New()
        {
            var viewModel = new CaseReportFormFieldTypeViewModel();
            viewModel.FormAction = "Create";
            return PartialView(@"~/Views/CaseReportForms/CaseReportFormFieldTypes/New.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Create Role")]
        public async Task<IActionResult> Create(CaseReportFormFieldType fieldType)
        {
            try
            {
                fieldType.Name = Request.Form["Name"];
                if (ModelState.IsValid)
                {
                    _context.Add(fieldType);
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

            var foundItem = _context.CaseReportFormFieldTypes
                                    .Where(crfc => crfc.ID == id)
                                    .FirstOrDefault();

            if (foundItem == null)
            {
                return NotFound();
            }
            var viewModel = new CaseReportFormFieldTypeViewModel();
            viewModel.FormAction = "Edit";
            viewModel.Name = foundItem.Name;
            return PartialView(@"~/Views/CaseReportForms/CaseReportFormFieldTypes/New.cshtml", viewModel);
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
            var foundItem = _context.CaseReportFormFieldTypes
                                    .Where(crfft => crfft.ID == id)
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
            var foundItem = _context.CaseReportFormFieldTypes
                                    .Where(crfc => crfc.ID == id)
                                    .FirstOrDefault();
            _context.Remove(foundItem);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }
    }
}