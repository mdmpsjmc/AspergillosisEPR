using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Models.SettingsViewModels;
using AspergillosisEPR.Data;

namespace AspergillosisEPR.Controllers
{
    public class SettingsController : Controller
    {
        private AspergillosisContext _context;

        public SettingsController(AspergillosisContext context)
        {

            _context = context;
        }


        public IActionResult Index()
        {
            var settings = new SettingsViewModel();
            var diagnosisCategories = from dc in _context.DiagnosisCategories
                                      orderby dc.CategoryName
                                      select dc;
            var diagnosisTypes = from dt in _context.DiagnosisTypes
                                      orderby dt.Name
                                      select dt;

            var drugs = from d in _context.Drugs
                                 orderby d.Name
                                 select d;
            settings.DiagnosisCategories = diagnosisCategories.ToList();
            settings.DiagnosisTypes = diagnosisTypes.ToList();
            settings.Drugs = drugs.ToList();
            return View(settings);
        }
    }
}