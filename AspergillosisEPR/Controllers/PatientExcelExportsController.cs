using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models.PatientViewModels;
using Microsoft.AspNetCore.Hosting;
using NPOI.XSSF.UserModel;
using System.IO;
using AspergillosisEPR.Lib;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.Collections;
using AspergillosisEPR.Extensions;
using System;
using System.Linq;
using AspergillosisEPR.Lib.Exporters;

namespace AspergillosisEPR.Controllers
{
    public class PatientExcelExportsController : PatientExportsController
    {
        private static string EXPORTED_EXCEL_DIRECTORY = @"\wwwroot\Files\Exported\Excel\";
        private static string EXCEL_2007_CONTENT_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        
        public PatientExcelExportsController(AspergillosisContext context,
                                             IHostingEnvironment hostingEnvironment) : base(context, hostingEnvironment)
        {
            _fileStoragePath = _hostingEnvironment.ContentRootPath + EXPORTED_EXCEL_DIRECTORY;
        }

        public async Task<IActionResult> Details(int id)
        {

            PatientDetailsViewModel patientDetailsVM = await GetExportViewModel(id);
            var exporter = new PatientDetailsExcelExporter(patientDetailsVM, DetailsDisplayControlProperties(), Request.Form);
            return GetFileContentResult(exporter.ToOutputBytes(), ".xlsx", EXCEL_2007_CONTENT_TYPE);
        }
    }
}