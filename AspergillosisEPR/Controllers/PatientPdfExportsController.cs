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
    public class PatientPdfExportsController : Controller
    {
        private IConverter _pdfConverter;
        private IViewRenderService _htmlRenderService;
        private PatientManager _patientManager;
        private AspergillosisContext _context;
        private IHostingEnvironment _hostingEnvironment;

        private static string SAVED_CHARTS_DIRECTORY = @"\wwwroot\Files\Charts\";
        private static string EXPORTED_PDFS_DIRECTORY = @"\wwwroot\Files\Exported\PDF\";

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
            SavePNGChart(base64String, sgrqChartFileName);
            patientDetailsViewModel.SgrqImageChartFile = sgrqChartFileName;
            SetItemsToShowInPdf(patientDetailsViewModel);

            string htmlView = await _htmlRenderService.RenderToStringAsync("Patients/PdfDetails", patientDetailsViewModel);
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
            byte[] pdf = _pdfConverter.Convert(htmlToPdfDocument);
            using (FileStream stream = new FileStream(_hostingEnvironment.ContentRootPath + EXPORTED_PDFS_DIRECTORY + DateTime.UtcNow.Ticks.ToString() + ".pdf", FileMode.Create))
            {
                stream.Write(pdf, 0, pdf.Length);
            }
            var fileContentResult = new FileContentResult(pdf, "application/pdf");
            return fileContentResult;
        }

        private void SetItemsToShowInPdf(PatientDetailsViewModel patientDetailsViewModel)
        {
            var displayControlKeys = Request.Form.Keys.Where(k => k.Contains("Show")).ToList();
            var displayControlProps = typeof(PatientDetailsViewModel).GetProperties().
                                                                      Where(p => p.Name.ToString().Contains("Show")).
                                                                      ToList();
            foreach (var key in displayControlProps)
            {
                var displayKeyValue = Request.Form[key.Name];
                var propertyInfo = patientDetailsViewModel.GetType().GetProperty(key.Name);
                if (displayKeyValue == "on")
                {
                    propertyInfo.SetValue(patientDetailsViewModel, true);
                }
                else
                {
                    propertyInfo.SetValue(patientDetailsViewModel, false);
                }
            }
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
