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
using AspergillosisEPR.Extensions.Validations;

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class DrugsController : Controller
    {
        private AspergillosisContext _context;

        public DrugsController(AspergillosisContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin Role, Create Role")]
        public IActionResult New()
        {
            
            return PartialView(SetupViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Create Role")]
        public async Task<IActionResult> Create([Bind("Name")] Drug drug)
        {
            try
            {
                ValidationExtensions.CheckFieldUniqueness(this, _context.Drugs, "Name", drug.Name);

                if (ModelState.IsValid)
                {
                    _context.Add(drug);
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

            var drug = await _context.Drugs
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync(m => m.ID == id);
            if (drug == null)
            {
                return NotFound();
            }
            return PartialView(SetupViewModel(drug));
        }

        [HttpPost, ActionName("Edit")]
        [Authorize(Roles = "Admin Role, Update Role")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDrug(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drug = await _context.Drugs
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == id);
            drug.Name = Request.Form["Name"];
            if (TryValidateModel(drug))
            {
                try
                {
                    _context.Drugs.Update(drug);
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
            var drug = await _context.Drugs.SingleOrDefaultAsync(p => p.ID == id);
            _context.Drugs.Remove(drug);
            _context.PatientDrugs.RemoveRange(_context.PatientDrugs.Where(pd => pd.DrugId == id));
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }

        private AddNewItemViewModel SetupViewModel(Drug drug = null)
        {
            if (drug == null)
            {
                drug = new Drug();
            }

            var addNewItemVM = new AddNewItemViewModel();
            addNewItemVM.ItemKlass = drug.KlassName;
            addNewItemVM.Controller = GetType().Name.ToString().Replace("Controller", "");
            addNewItemVM.Name = drug.Name;
            addNewItemVM.ItemId = drug.ID;
            addNewItemVM.Tab = "drugs";
            return addNewItemVM;
        }
    }


   
}