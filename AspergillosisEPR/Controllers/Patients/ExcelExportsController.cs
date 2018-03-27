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
using AspergillosisEPR.Controllers.Patients;

namespace AspergillosisEPR.Controllers.Patients
{
    public class ExcelExportsController : ExportsController
    {
        private static string EXPORTED_EXCEL_DIRECTORY = @"\wwwroot\Files\Exported\Excel\";
        private static string EXCEL_2007_CONTENT_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        
        public ExcelExportsController(AspergillosisContext context,
                                      IHostingEnvironment hostingEnvironment) : base(context, hostingEnvironment)
        {
            _fileStoragePath = _hostingEnvironment.ContentRootPath + EXPORTED_EXCEL_DIRECTORY;
        }

        public async Task<IActionResult> Details(int id)
        {
            PatientDetailsViewModel patientDetailsVM = await GetExportViewModel(id);
            patientDetailsVM.STGQuestionnaires = patientDetailsVM.STGQuestionnaires.OrderBy((q => q.DateTaken)).ToList();
            bool isAnonymous = User.IsInRole("Anonymous Role") && !User.IsInRole("Read Role");
            var exporter = new PatientDetailsExcelExporter(patientDetailsVM, 
                                                           DetailsDisplayControlProperties(), Request.Form,
                                                           _context, isAnonymous);
            return GetFileContentResult(exporter.ToOutputBytes(), ".xlsx", EXCEL_2007_CONTENT_TYPE);
        }       
    }
}