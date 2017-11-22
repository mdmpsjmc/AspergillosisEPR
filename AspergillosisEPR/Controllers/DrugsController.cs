using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Models.SettingsViewModels;
using AspergillosisEPR.Models;

namespace AspergillosisEPR.Controllers
{
    public class DrugsController : Controller
    {
        public IActionResult New()
        {
            var drug = new Drug();
            var addNewItemVM = new AddNewItemViewModel();
            addNewItemVM.ItemKlass = drug.KlassName;
            addNewItemVM.Controller = GetType().Name.ToString().Replace("Controller", "");
            return PartialView(addNewItemVM);
        }
    }
}