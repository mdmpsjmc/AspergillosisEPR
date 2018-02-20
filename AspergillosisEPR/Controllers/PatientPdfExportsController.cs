using AspergillosisEPR.Data;
using AspergillosisEPR.Lib;
using AspergillosisEPR.Lib.Exporters;
using AspergillosisEPR.Models.PatientViewModels;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static AspergillosisEPR.Services.ViewToString;
namespace AspergillosisEPR.Controllers
{
    public class PatientPdfExportsController : PatientExportsController
    {
        private PatientDetailsPdfExporter _pdfConverter;

        public PatientPdfExportsController(IConverter dinkToPdfConverter, 
                                           IViewRenderService htmlRenderService, 
                                           IHostingEnvironment hostingEnvironment, 
                                           AspergillosisContext context) : base(context, hostingEnvironment)
        {
            _pdfConverter = new PatientDetailsPdfExporter(dinkToPdfConverter, htmlRenderService, hostingEnvironment);
            _fileStoragePath = _hostingEnvironment.ContentRootPath + PatientDetailsPdfExporter.EXPORTED_PDFS_DIRECTORY;
        }
        [HttpPost]
        public async Task<IActionResult> Details(int id, string sgrqChart, int igChartsLength)
        {
            PatientDetailsViewModel patientDetailsViewModel = await GetExportViewModel(id);
            if (igChartsLength > 0) AddIgChartsToPatientVM(patientDetailsViewModel, igChartsLength);
            var pdfBytes = _pdfConverter.GenerateDetailsPdf(id, sgrqChart, patientDetailsViewModel);
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
