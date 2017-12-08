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

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class DiagnosisTypesController : Controller
    {
        private AspergillosisContext _context;

        public DiagnosisTypesController(AspergillosisContext context)
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
        public async Task<IActionResult> Create([Bind("Name, ShortName")] DiagnosisType diagnosisType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(diagnosisType);
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

            var diagnosisType = await _context.DiagnosisTypes
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == id);
            if (diagnosisType == null)
            {
                return NotFound();
            }
            return PartialView(SetupViewModel(diagnosisType));
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin Role, Update Role")]
        public async Task<IActionResult> EditDiagnosisType(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dbDiagnosisType = await _context.DiagnosisTypes
                                .AsNoTracking()
                                .SingleOrDefaultAsync(m => m.ID == id);
            dbDiagnosisType.Name = Request.Form["Name"];
            dbDiagnosisType.ShortName = Request.Form["ShortName"];
            if (TryValidateModel(dbDiagnosisType))
            {
                try
                {
                    _context.DiagnosisTypes.Update(dbDiagnosisType);
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
            var diagnosisType = await _context.DiagnosisTypes.SingleOrDefaultAsync(p => p.ID == id);
            _context.DiagnosisTypes.Remove(diagnosisType);
            _context.PatientDiagnoses.RemoveRange(_context.PatientDiagnoses.Where(pd => pd.DiagnosisTypeId == id));
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }

        private AddNewItemViewModel SetupViewModel(DiagnosisType diagnosisType = null)
        {
            if (diagnosisType == null)
            {
                diagnosisType = new DiagnosisType();
            }
          
            var addNewItemVM = new AddNewItemViewModel();
            addNewItemVM.ItemKlass = diagnosisType.KlassName;
            addNewItemVM.Controller = GetType().Name.ToString().Replace("Controller", "");
            addNewItemVM.Name = diagnosisType.Name;
            addNewItemVM.ItemId = diagnosisType.ID;
            addNewItemVM.Tab = "diagnosis-types";
            return addNewItemVM;
        }
    }
}