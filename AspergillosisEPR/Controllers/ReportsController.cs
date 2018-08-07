using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Lib;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models.Reporting;
using Microsoft.AspNetCore.Authorization;
using AspergillosisEPR.Helpers;
using System.Collections;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspergillosisEPR.Controllers
{
    public class ReportsController : Controller
    {
        private readonly DropdownListsResolver _dropdownResolver;
        private readonly AspergillosisContext _context;

        public ReportsController(AspergillosisContext aspergillosisContext)
        {
            _dropdownResolver = new DropdownListsResolver(aspergillosisContext, ViewBag);
            _context = aspergillosisContext;
        }

        public IActionResult Index()
        {
            ViewBag.ReportTypesIds = _dropdownResolver.PopulateReportTypesDropdownList();
            return View();
        }

        public IActionResult Details()
        {
            string partialName = Request.Path.ToString().Split("/")[3];
            var reportType = _context.ReportTypes.FirstOrDefault(rt => rt.Discriminator == partialName);

            return PartialView(@"/Views/Reports/ReportTypes/_" + partialName + ".cshtml");
        }

        [Authorize(Roles = ("Admin Role, Reporting Role"))]
        public IActionResult Create(Report report)
        {
            var reportType = _context.ReportTypes
                                    .FirstOrDefault(rt => rt.Discriminator == Request.Form["ReportTypeID"]);
            if (reportType == null)
            {
                return Json(new { success = false });
            }
            report.ReportTypeId = reportType.ID;
            if (ModelState.IsValid)
            {
                _context.Reports.Update(report);
                _context.SaveChanges();
                return Json(new { result = "ok" });

            } else
            {
                Hashtable errors = ModelStateHelper.Errors(ModelState);
                return Json(new { success = false, errors });
            }

        }
    }
}
