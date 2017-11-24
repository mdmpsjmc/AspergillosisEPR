using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Models.SettingsViewModels;
using AspergillosisEPR.Models;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Helpers;
using System.Collections;
using AspergillosisEPR.Data;
using System.Threading.Tasks;
using System.Linq;

namespace AspergillosisEPR.Controllers
{
    public class SideEffectsController : Controller
    {
        private AspergillosisContext _context;

        public SideEffectsController(AspergillosisContext context)
        {
            _context = context;
        }

        public IActionResult New()
        {

            return PartialView(SetupViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] SideEffect sideEffect)
        {
            try
            {
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
    }
}