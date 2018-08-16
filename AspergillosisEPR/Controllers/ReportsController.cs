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
using Microsoft.Extensions.Primitives;
using AspergillosisEPR.Lib.Reporting;
using AspergillosisEPR.Controllers.Patients;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AspergillosisEPR.Controllers
{

    [Route("reports")]
    public class ReportsController : ExportsController
    {
        private readonly DropdownListsResolver _dropdownResolver;
        private readonly AspergillosisContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private static string EXCEL_2007_CONTENT_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private static string EXPORTED_REPORTS_DIRECTORY = @"\wwwroot\Files\Exported\Excel\";

        public ReportsController(AspergillosisContext context, IHostingEnvironment hostingEnvironment) : base(context, hostingEnvironment)
        {
            _dropdownResolver = new DropdownListsResolver(context, ViewBag);
            _hostingEnvironment = hostingEnvironment;
            _fileStoragePath = _hostingEnvironment.ContentRootPath + EXPORTED_REPORTS_DIRECTORY;
            _context = context;
        }

        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("New")]
        public IActionResult New()
        {
            ViewBag.ReportTypesIds = _dropdownResolver.PopulateReportTypesDropdownList();
            return View();
        }

        [Route("Details")]
        public IActionResult Details(string partialName)
        {
            //string partialName = Request.Path.ToString().Split("/")[3];
            var reportType = _context.ReportTypes.FirstOrDefault(rt => rt.Discriminator == partialName);

            return PartialView(@"/Views/Reports/ReportTypes/_" + partialName + ".cshtml");
        }

        [Authorize(Roles = ("Admin Role, Reporting Role"))]
        [Route("Create")]
        [HttpPost]
        public IActionResult Create([Bind("ID, StartDate, EndDate, PatientIds")]Report report)
        {
            var reportType = _context.ReportTypes
                                     .FirstOrDefault(rt => rt.Discriminator == Request.Form["ReportTypeID"]);
            StringValues patientIds;
            if (reportType == null)
            {
                return Json(new { success = false });
            }
            var reportItems = new List<PatientReportItem>();
            if (!string.IsNullOrEmpty(Request.Form["PatientIds"])) patientIds = Request.Form["PatientIds"];
            if (patientIds.ToList()[0] == "null") ModelState.AddModelError("Base", "You need to add at least one patient to this report");
            if (patientIds.ToList()[0] != "null" && !string.IsNullOrEmpty(patientIds))
            {
                var idsToFind = patientIds.ToString()
                                          .Split(",")
                                          .Select(id => Int32.Parse(id))
                                          .ToList();
               
                var patients = _context.Patients.Where(p => idsToFind.Contains(p.ID));
                foreach(var patient in patients)
                {
                    var patientReportItem = new PatientReportItem()
                    {
                        PatientId = patient.ID
                    };
                    reportItems.Add(patientReportItem);
                }
            }
            report.ReportTypeId = reportType.ID;
            report.PatientReportItems = reportItems;

            if (ModelState.IsValid)
            {
                _context.Reports.Update(report);
                _context.SaveChanges();
                return Json(new { success = true, id = report.ID });

            }
            else
            {
                Hashtable errors = ModelStateHelper.Errors(ModelState);
                return Json(new { success = false, errors });
            }
        }

        [Route("generate/{id:int}")]
        public IActionResult Generate(int id)
        {
            var report = _context.Reports
                                 .Where(r => r.ID == id)
                                 .FirstOrDefault();

            var reportBuilder = new SGRQReportBuilder(_context, report);
            var byteData = reportBuilder.Build();
            return GetFileContentResult(byteData, ".xlsx", EXCEL_2007_CONTENT_TYPE);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Delete Role, Admin Role")]
        [ValidateAntiForgeryToken]
        [Route("delete/{id:int}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.Reports.SingleOrDefaultAsync(r => r.ID == id);
            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();
            return Json(new { ok = "ok" });
        }
    }
}
