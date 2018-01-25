using AspergillosisEPR.Data;
using AspergillosisEPR.Lib;
using AspergillosisEPR.Models.PatientViewModels;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static AspergillosisEPR.Services.ViewToString;
namespace AspergillosisEPR.Controllers
{
    public class PatientPdfExportsController : Controller
    {
        private IConverter _pdfConverter;
        private IViewRenderService _htmlRenderService;
        private PatientManager _patientManager;
        private AspergillosisContext _context;
        private IHostingEnvironment _hostingEnvironment;

        public PatientPdfExportsController(IConverter converter, 
                                           IViewRenderService htmlRenderService, 
                                           IHostingEnvironment hostingEnvironment,
                                           AspergillosisContext context)
        {
            _pdfConverter = converter;
            _htmlRenderService = htmlRenderService;
            _context = context;
            _patientManager = new PatientManager(context);
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpPost]
        public async Task<IActionResult> Details(int id, string sgrqChart)
        {
            var patient = await _patientManager.FindPatientWithRelationsByIdAsync(id);
            var patientDetailsViewModel = PatientDetailsViewModel.BuildPatientViewModel(_context, patient);
            var sgrqChartFileName = "SGRQ_Chart_" + id.ToString() + "_";
            patientDetailsViewModel.ShowButtons = false;
            string base64String = sgrqChart.Replace("data:image/png;base64,", String.Empty);
            SaveImage(base64String, sgrqChartFileName);
            patientDetailsViewModel.SgrqImageChartFile = sgrqChartFileName;
            string htmlView = await _htmlRenderService.RenderToStringAsync("Patients/PdfDetails", patientDetailsViewModel);          
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4Plus,
                 },
                Objects = {
                    new ObjectSettings() {
                       PagesCount = true,
                       HtmlContent = htmlView,
                       WebSettings = { DefaultEncoding = "utf-8" },
                       HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
                    }
                }
            };
            byte[] pdf = _pdfConverter.Convert(doc);
            using (FileStream stream = new FileStream(_hostingEnvironment.ContentRootPath + @"\wwwroot\Files\Exported\PDF\" + DateTime.UtcNow.Ticks.ToString() + ".pdf", FileMode.Create))
            {
                stream.Write(pdf, 0, pdf.Length);
            }
            var fileContentResult = new FileContentResult(pdf, "application/pdf");
            return fileContentResult;
        }

        private bool SaveImage(string ImgStr, string ImgName)
        {
            String path = _hostingEnvironment.ContentRootPath + @"\wwwroot\Files\Charts\";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string imageName = ImgName + ".png";
            string imgPath = Path.Combine(path, imageName);

            byte[] imageBytes = Convert.FromBase64String(ImgStr);
            System.IO.File.WriteAllBytes(imgPath, imageBytes);

            return true;
        }
    }

    
}
