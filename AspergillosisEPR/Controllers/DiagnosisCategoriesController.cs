using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.SettingsViewModels;

namespace AspergillosisEPR.Controllers
{
    public class DiagnosisCategoriesController : Controller
    {
        public IActionResult New()
        {
            var diagnosisCategory = new DiagnosisCategory();
            var addNewItemVM = new AddNewItemViewModel();
            addNewItemVM.ItemKlass = diagnosisCategory.KlassName;
            addNewItemVM.Controller = GetType().Name.ToString().Replace("Controller", "");
            return PartialView(addNewItemVM);
        }


    }
}