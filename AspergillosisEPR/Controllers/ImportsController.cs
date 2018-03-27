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
using AspergillosisEPR.Lib;
using System.Threading.Tasks;
using AspergillosisEPR.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using AspergillosisEPR.Lib.Importers;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Models.Patients;

namespace AspergillosisEPR.Controllers
{
    [Authorize(Roles = "Admin Role")]
    public class ImportsController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly AspergillosisContext _context;
        private Importer _importer;
        private ImporterResolver _importerResolver;

        public ImportsController(IHostingEnvironment hostingEnvironment, AspergillosisContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
            _importerResolver = new ImporterResolver(_context);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult New()
        {
            PopulateDbImportTypesDropdownList();
            return View();
        }

        public async Task<IActionResult> Create()
        {
            IFormFile file = Request.Form.Files[0];

            string webRootPath = _hostingEnvironment.WebRootPath;
            int importerId = Int32.Parse(Request.Form["DbImportTypeId"]);
            var dbImporterType = _importerResolver.GetDbImporterTypeById(importerId);
            
            Action<FileStream, IFormFile, string> readFileAction = (stream, formFile, extension) => {
                _importer = _importerResolver.Resolve(dbImporterType.ImporterClass, new object[] { stream, file, extension, _context });
            };
            FileImporter.Import(file, webRootPath, readFileAction);

            var dbImport = new DbImport()
            {
                ImportedDate = DateTime.Now,
                ImportedFileName = file.FileName,
                DbImportTypeId = importerId,
                PatientsCount = _importer.Imported.Count()
            };
            _context.Add(dbImport);
            SaveItemsInDatabase();
            await _context.SaveChangesAsync();
            return Json(new { result = _importer.Imported.Count() });
        }

        private void SaveItemsInDatabase()
        {
            foreach (var record in _importer.Imported)
            {
                if ((record.ID != 0) && (record.ID > 0))
                {
                    _context.Attach(record);
                    if (record.GetType() != typeof(PatientImmunoglobulin))
                    {
                       _context.Entry(record).State = EntityState.Modified;
                    }
                }
                else if (record.ID < 0)
                {
                    _context.Add(record);
                }
                else
                {
                    _context.Add(record);
                }                
            }
        }

        private void PopulateDbImportTypesDropdownList()
        {
            var dbImportTypes = from importType in _context.DBImportTypes
                                orderby importType.Name
                                select importType;
            var selectList = new SelectList(dbImportTypes, "ID", "Name");
            ViewBag.ImportTypes = selectList;
        }

        
    }
}