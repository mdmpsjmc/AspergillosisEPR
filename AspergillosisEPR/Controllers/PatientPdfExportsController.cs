using AspergillosisEPR.Data;
using AspergillosisEPR.Lib;
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
        private IConverter _pdfConverter;
        private IViewRenderService _htmlRenderService;

        private static string SAVED_CHARTS_DIRECTORY = @"\wwwroot\Files\Charts\";
        private static string EXPORTED_PDFS_DIRECTORY = @"\wwwroot\Files\Exported\PDF\";

        public PatientPdfExportsController(IConverter converter, 
                                           IViewRenderService htmlRenderService, 
                                           IHostingEnvironment hostingEnvironment, 
                                           AspergillosisContext context) : base(context, hostingEnvironment)
        {
            _pdfConverter = converter;
            _htmlRenderService = htmlRenderService;
            _fileStoragePath = _hostingEnvironment.ContentRootPath + EXPORTED_PDFS_DIRECTORY;
        }
        [HttpPost]
        public async Task<IActionResult> Details(int id, string sgrqChart)
        {
            PatientDetailsViewModel patientDetailsViewModel = await GetExportViewModel(id);
            ProcessPdfChart(id, sgrqChart, patientDetailsViewModel);
            string htmlView = await _htmlRenderService.RenderToStringAsync("/Views/Patients/PdfDetails.cshtml", patientDetailsViewModel);
            return GeneratePdfFromHtml(htmlView);
        }

        private FileContentResult GeneratePdfFromHtml(string htmlView)
        {
            var htmlToPdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4
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
            byte[] outputFileBytes = _pdfConverter.Convert(htmlToPdfDocument);
            FileContentResult fileContentResult = GetFileContentResult(outputFileBytes, ".pdf", "application/pdf");
            return fileContentResult;
        }        

        private void ProcessPdfChart(int id, string sgrqChart, PatientDetailsViewModel patientDetailsViewModel)
        {
            var sgrqChartFileName = "SGRQ_Chart_" + id.ToString() + "_";
            patientDetailsViewModel.ShowButtons = false;
            string base64String = sgrqChart.Replace("data:image/png;base64,", String.Empty);
            SavePNGChart(base64String, sgrqChartFileName);
            patientDetailsViewModel.SgrqImageChartFile = sgrqChartFileName;
        }       

        private bool SavePNGChart(string ImgStr, string ImgName)
        {
            String path = _hostingEnvironment.ContentRootPath + SAVED_CHARTS_DIRECTORY;

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
