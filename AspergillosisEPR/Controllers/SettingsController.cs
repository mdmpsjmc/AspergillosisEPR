using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Models.SettingsViewModels;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Authorization;

namespace AspergillosisEPR.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private AspergillosisContext _context;

        public SettingsController(AspergillosisContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin Role, Read Role")]
        public IActionResult Index()
        {
            var settings = new SettingsViewModel();
            var diagnosisCategories = from dc in _context.DiagnosisCategories
                                      orderby dc.CategoryName
                                      select dc;

            var drugs = from d in _context.Drugs
                        orderby d.Name
                        select d;

            settings.DiagnosisCategories = diagnosisCategories.ToList();
            settings.Drugs = drugs.ToList();
            return View(settings);
        }
    }
}