using AspergillosisEPR.Data;
using AspergillosisEPR.Models.PatientViewModels;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static AspergillosisEPR.Services.ViewToString;

namespace AspergillosisEPR.Lib.Exporters
{
    public class PatientDetailsPdfExporter
    {
        public static string EXPORTED_PDFS_DIRECTORY = @"\wwwroot\Files\Exported\PDF\";
        private IConverter _pdfConverter;
        private IViewRenderService _htmlRenderService;
        private string _fileStoragePath;


        public PatientDetailsPdfExporter(IConverter converter,
                                         IViewRenderService htmlRenderService,
                                         IHostingEnvironment hostingEnvironment) 
        {
            _pdfConverter = converter;
            _htmlRenderService = htmlRenderService;
            _fileStoragePath = hostingEnvironment.ContentRootPath + EXPORTED_PDFS_DIRECTORY;
        }

        public async Task<byte[]> GenerateDetailsPdf(int id, string sgrqChart, PatientDetailsViewModel patientDetailsViewModel)
        {
            ProcessSGRQChart(id, sgrqChart, patientDetailsViewModel);
            ProcessIgCharts(patientDetailsViewModel);
            string htmlView = await _htmlRenderService.RenderToStringAsync("/Views/Patients/PdfDetails.cshtml", patientDetailsViewModel);
            return GeneratePdfFromHtml(htmlView);
        }

        public async Task<byte[]> GenerateVisitDetailsPdf(PatientVisitDetailsViewModel patientVisitDetailsViewModel)
        {
            string htmlView = await _htmlRenderService.RenderToStringAsync("/Views/PatientVisits/PdfVisitDetails.cshtml", patientVisitDetailsViewModel);
            return GeneratePdfFromHtml(htmlView);
        }

        private void ProcessIgCharts(PatientDetailsViewModel patientDetailsViewModel)
        {
            if (patientDetailsViewModel.IgCharts == null) return;
            foreach(var igChart in patientDetailsViewModel.IgCharts)
            {                
                SavePNGChart(igChart.PngImage, igChart.FileName);
            }
        }

        private void ProcessSGRQChart(int id, string sgrqChart, PatientDetailsViewModel patientDetailsViewModel)
        {
            if (sgrqChart == null) return;
            var sgrqChartFileName = "SGRQ_Chart_" + id.ToString() + "_";
            patientDetailsViewModel.ShowButtons = false;
            string base64String = sgrqChart.Replace("data:image/png;base64,", String.Empty);
            SavePNGChart(base64String, sgrqChartFileName);
            patientDetailsViewModel.SgrqImageChartFile = sgrqChartFileName;
        }

       
        private bool SavePNGChart(string ImgStr, string ImgName)
        {
            if (!Directory.Exists(_fileStoragePath))
            {
                Directory.CreateDirectory(_fileStoragePath);
            }
            string imageName = ImgName + ".png";
            string imgPath = Path.Combine(_fileStoragePath, imageName);

            byte[] imageBytes = Convert.FromBase64String(ImgStr);
            System.IO.File.WriteAllBytes(imgPath, imageBytes);

            return true;
        }

        private byte[] GeneratePdfFromHtml(string htmlView)
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
            return outputFileBytes;
        }
    }
}
