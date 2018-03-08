using AspergillosisEPR.Controllers.Patients;
using AspergillosisEPR.Data;
using AspergillosisEPR.Lib;
using AspergillosisEPR.Lib.Exporters;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.PatientViewModels;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static AspergillosisEPR.Services.ViewToString;
namespace AspergillosisEPR.Controllers.Patients
{
    [Route("patients/{id:int}/exports/pdf")]
    public class PdfExportsController : ExportsController
    {
        private PatientDetailsPdfExporter _pdfConverter;
        private UserManager<ApplicationUser> _userManager;

        public PdfExportsController(IConverter dinkToPdfConverter, 
                                    IViewRenderService htmlRenderService, 
                                    IHostingEnvironment hostingEnvironment, 
                                    ApplicationDbContext _appContext, 
                                    UserManager<ApplicationUser> userManager,
                                    AspergillosisContext context) : base(context, hostingEnvironment)
        {
            _pdfConverter = new PatientDetailsPdfExporter(dinkToPdfConverter, htmlRenderService, hostingEnvironment);
            _fileStoragePath = _hostingEnvironment.ContentRootPath + PatientDetailsPdfExporter.EXPORTED_PDFS_DIRECTORY;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> Details(int id, string sgrqChart, int igChartsLength)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var rolesCount = _userManager.GetRolesAsync(user).Result.Count;
            PatientDetailsViewModel patientDetailsViewModel = await GetExportViewModel(id);
            patientDetailsViewModel.ShowDetails = rolesCount > 1;
            if (igChartsLength > 0) AddIgChartsToPatientVM(patientDetailsViewModel, igChartsLength);
            var pdfBytes = _pdfConverter.GenerateDetailsPdf(id, sgrqChart, patientDetailsViewModel);
            return GetFileContentResult(pdfBytes.Result, ".pdf", "application/pdf");
        }

        [HttpPost]
        [Route("visit")]
        public IActionResult VisitDetails(int id, bool exportCharts, 
                                          bool otherVisits, string sgrqChart)
        {
            var patientVisitManager = new PatientVisitManager(_context, ViewBag);
            var patientVisit = patientVisitManager.GetPatientVisitById(id);
            var patientDetailsVM = PatientVisitDetailsViewModel.BuildPatientVisitDetailsVM(patientVisitManager, patientVisit);
            ViewBag.ShowButtons = false;
            patientDetailsVM.ShowOtherVisits = otherVisits;
            var pdfBytes = _pdfConverter.GenerateVisitDetailsPdf(patientDetailsVM);
            return GetFileContentResult(pdfBytes.Result, ".pdf", "application/pdf");
        }

        private void AddIgChartsToPatientVM(PatientDetailsViewModel patientDetailsViewModel, int chartsCount)
        {
            var igCharts = new List<PatientIgChart>();
            for(var cursor = 0; cursor < chartsCount; cursor++)
            {
                var igChart = new PatientIgChart();
                igChart.PngImage = Request.Form["PatientCharts[" + cursor + "].Image"].ToString().
                                                                                       Replace("data:image/png;base64,", String.Empty);
                if (igChart.PngImage.Length < 25) continue;
                igChart.FileName = "Ig_Chart_" + cursor + "_" + patientDetailsViewModel.Patient.ID.ToString() + "_";
                igCharts.Add(igChart);
            }
            patientDetailsViewModel.IgCharts = igCharts;
        }
    }    
}
