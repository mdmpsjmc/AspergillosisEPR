using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Models.SettingsViewModels;
using AspergillosisEPR.Models;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Helpers;
using System.Collections;
using AspergillosisEPR.Data;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using AspergillosisEPR.Extensions;
using AspergillosisEPR.Extensions.Validations;

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class DiagnosisCategoriesController : Controller
    {
        private AspergillosisContext _context;

        public DiagnosisCategoriesController(AspergillosisContext context)
        {
            _context = context;
        }
        [Authorize(Roles ="Admin Role, Create Role")]
        public IActionResult New()
        {            
            return PartialView(SetupViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Admin Role, Create Role")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryName")] DiagnosisCategory diagnosisCategory)
        {
            try
            {
                ValidationExtensions.CheckFieldUniqueness(this, _context.DiagnosisCategories, "CategoryName", diagnosisCategory.CategoryName);
                if (ModelState.IsValid)
                {
                    _context.Add(diagnosisCategory);
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

            var diagnosisCategory = await _context.DiagnosisCategories
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync(m => m.ID == id);
            if (diagnosisCategory == null)
            {
                return NotFound();
            }
            return PartialView(SetupViewModel(diagnosisCategory));
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Update Role")]
        public async Task<IActionResult> EditDiagnosisCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbDiagnosisCategory = await _context.DiagnosisCategories
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == id);
            dbDiagnosisCategory.CategoryName = Request.Form["Name"];
            if (TryValidateModel(dbDiagnosisCategory))
            {
                try
                {
                    _context.DiagnosisCategories.Update(dbDiagnosisCategory);
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
            var diagnosisCategory = await _context.DiagnosisCategories.SingleOrDefaultAsync(p => p.ID == id);
            _context.DiagnosisCategories.Remove(diagnosisCategory);
            _context.PatientDiagnoses.RemoveRange(_context.PatientDiagnoses.Where(pd => pd.DiagnosisCategoryId == id));
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }

        private AddNewItemViewModel SetupViewModel(DiagnosisCategory diagnosisCategory = null)
        {
            if (diagnosisCategory == null)
            {
                diagnosisCategory = new DiagnosisCategory();
            }

            var addNewItemVM = new AddNewItemViewModel();
            addNewItemVM.ItemKlass = diagnosisCategory.KlassName;
            addNewItemVM.Controller = GetType().Name.ToString().Replace("Controller", "");
            addNewItemVM.Name = diagnosisCategory.CategoryName;
            addNewItemVM.ItemId = diagnosisCategory.ID;
            addNewItemVM.Tab = "diagnosis-categories";
            return addNewItemVM;
        }
    }
}