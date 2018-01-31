using AspergillosisEPR.Data;
using AspergillosisEPR.Lib;
using AspergillosisEPR.Lib.Exporters;
using AspergillosisEPR.Models.PatientViewModels;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public async Task<IActionResult> Details(int id, string sgrqChart)
        {
            PatientDetailsViewModel patientDetailsViewModel = await GetExportViewModel(id);
            var pdfBytes = _pdfConverter.GenerateDetailsPdf(id, sgrqChart, patientDetailsViewModel);
            return GetFileContentResult(pdfBytes.Result, ".pdf", "application/pdf");
        }
    }    
}
