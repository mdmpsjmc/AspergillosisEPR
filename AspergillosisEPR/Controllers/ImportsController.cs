using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using AspergillosisEPR.Models;
using System.Collections;
using System.Reflection;
using AspergillosisEPR.Data;
using AspergillosisEPR.Helpers;
using System.Threading.Tasks;

namespace AspergillosisEPR.Controllers
{
    [Authorize(Roles="Admin Role")]
    public class ImportsController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly AspergillosisContext _context;
        private SpreadsheetReader _spreadsheetReader;

        public ImportsController(IHostingEnvironment hostingEnvironment, AspergillosisContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult New()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            IFormFile file = Request.Form.Files[0];

            string webRootPath = _hostingEnvironment.WebRootPath;
           
            Action<FileStream, IFormFile, string> readSpreadsheetAction = (stream, formFile, extension) => {
                _spreadsheetReader = new SpreadsheetReader(stream, file, extension, _context);
            };
            FileImporter.Import(file, webRootPath, readSpreadsheetAction);

            var dbImport = new DbImport()
            {
                ImportedDate = DateTime.Now,
                ImportedFileName = file.FileName,
                PatientsCount = _spreadsheetReader.ImportedPatients.Count()
            };
            _context.Add(dbImport);
            SavePatientsInDatabase();
            await _context.SaveChangesAsync();
            return Json(new { result = _spreadsheetReader.ImportedPatients.Count() });
        }       

        private void SavePatientsInDatabase()
        {
            foreach (var newPatient in _spreadsheetReader.ImportedPatients)
            {
                _context.Add(newPatient);
            }
        }

    }
}