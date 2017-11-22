using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Models.SettingsViewModels;
using AspergillosisEPR.Models;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Helpers;
using System.Collections;
using AspergillosisEPR.Data;
using System.Threading.Tasks;

namespace AspergillosisEPR.Controllers
{
    public class DiagnosisTypesController : Controller
    {
        private AspergillosisContext _context;

        public DiagnosisTypesController(AspergillosisContext context)
        {
            _context = context;
        }

        public IActionResult New()
        {
            var diagnosisType = new DiagnosisType();
            var addNewItemVM = new AddNewItemViewModel();
            addNewItemVM.ItemKlass = diagnosisType.KlassName;
            addNewItemVM.Controller = GetType().Name.ToString().Replace("Controller", "");
            return PartialView(addNewItemVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] DiagnosisType diagnosisType)
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
    }
}