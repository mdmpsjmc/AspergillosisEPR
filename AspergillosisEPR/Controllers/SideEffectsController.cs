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
    public class SideEffectsController : Controller
    {
        private AspergillosisContext _context;

        public SideEffectsController(AspergillosisContext context)
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
        public async Task<IActionResult> Create([Bind("Name")] SideEffect sideEffect)
        {
            try
            {
                ValidationExtensions.CheckFieldUniqueness(this, _context.SideEffects, "Name", sideEffect.Name);
                if (ModelState.IsValid)
                {
                    _context.Add(sideEffect);
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

            var sideEffect = await _context.SideEffects
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync(m => m.ID == id);
            if (sideEffect == null)
            {
                return NotFound();
            }
            return PartialView(SetupViewModel(sideEffect));
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

            var sideEffect = await _context.SideEffects
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == id);
            sideEffect.Name = Request.Form["Name"];
            if (TryValidateModel(sideEffect))
            {
                try
                {
                    _context.SideEffects.Update(sideEffect);
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
            var sideEffect = await _context.SideEffects.SingleOrDefaultAsync(p => p.ID == id);
            _context.SideEffects.Remove(sideEffect);
            _context.PatientDrugSideEffects.RemoveRange(_context.PatientDrugSideEffects.Where(pd => pd.SideEffectId == id));
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }

        private AddNewItemViewModel SetupViewModel(SideEffect sideEffect = null)
        {
            if (sideEffect == null)
            {
                sideEffect = new SideEffect();
            }

            var addNewItemVM = new AddNewItemViewModel();
            addNewItemVM.ItemKlass = sideEffect.KlassName;
            addNewItemVM.Controller = GetType().Name.ToString().Replace("Controller", "");
            addNewItemVM.Name = sideEffect.Name;
            addNewItemVM.ItemId = sideEffect.ID;
            addNewItemVM.Tab = "side-effects";
            return addNewItemVM;
        }


        private void CheckIfNameIsUnique(SideEffect sideEffect)
        {
            var existingItem = _context.SideEffects.FirstOrDefault(x => x.Name == sideEffect.Name);
            if (existingItem != null)
            {
                ModelState.AddModelError("sideEffect.Name", "Side effect with this name already exists in database");
            }
        }
    }
}