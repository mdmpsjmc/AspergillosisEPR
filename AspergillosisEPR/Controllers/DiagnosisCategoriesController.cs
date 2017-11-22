using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.SettingsViewModels;
using System.Threading.Tasks;
using System.Collections;
using AspergillosisEPR.Helpers;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Data;

namespace AspergillosisEPR.Controllers
{
    public class DiagnosisCategoriesController : Controller
    {
        private AspergillosisContext _context;

        public DiagnosisCategoriesController(AspergillosisContext context)
        {
            _context = context;
        }

        public IActionResult New()
        {
            var diagnosisCategory = new DiagnosisCategory();
            var addNewItemVM = new AddNewItemViewModel();
            addNewItemVM.ItemKlass = diagnosisCategory.KlassName;
            addNewItemVM.Controller = GetType().Name.ToString().Replace("Controller", "");
            return PartialView(addNewItemVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] DiagnosisCategory diagnosisCategory)
        {
            try
            {
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
    }
}