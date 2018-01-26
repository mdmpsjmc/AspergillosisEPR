using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models.PatientViewModels;
using Microsoft.AspNetCore.Hosting;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Http;

namespace AspergillosisEPR.Controllers
{
    public class PatientExcelExportsController : PatientExportsController
    {
        private static string EXPORTED_EXCEL_DIRECTORY = @"\wwwroot\Files\Exported\Excel\";
        private static string EXCEL_2007_CONTENT_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        private XSSFWorkbook _outputWorkbook;

        public PatientExcelExportsController(AspergillosisContext context, 
                                             IHostingEnvironment hostingEnvironment) : base(context, hostingEnvironment)
        {
            _fileStoragePath = _hostingEnvironment.ContentRootPath + EXPORTED_EXCEL_DIRECTORY;           
        }

        public async Task<IActionResult> Details(int id)
        {

            PatientDetailsViewModel patientDetailsVM = await GetExportViewModel(id);
            var names = GetSheetsNames();
            _outputWorkbook = new XSSFWorkbook();
            byte[] outputBytes = SerializeWorkbook();


            return GetFileContentResult(outputBytes, ".xlsx", EXCEL_2007_CONTENT_TYPE);
        }

        private List<string> GetSheetsNames()
        {
            var names = new List<string>();
            foreach(var propertyInfo in DetailsDisplayControlProperties())
            {
                var propertyName = propertyInfo.Name.ToString().Replace("Show", "");
                names.Add(propertyName);
            }
            return names;
        }

        public byte[] SerializeWorkbook()
        {
            MemoryStream ms = new MemoryStream();
            using (MemoryStream tempStream = new MemoryStream())
            {
                _outputWorkbook.Write(tempStream);
                var byteArray = tempStream.ToArray();
                ms.Write(byteArray, 0, byteArray.Length);
                return ms.ToArray();
            }
        }
    }
}