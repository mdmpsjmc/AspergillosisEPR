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
using Microsoft.Extensions.Configuration;
using AspergillosisEPR.Models.AspergillosisViewModels;

namespace AspergillosisEPR.Controllers
{
    [Authorize(Roles = "Admin Role")]
    public class ImportsController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly AspergillosisContext _context;
        private readonly IConfiguration _configuration;
        private Importer _importer;
        private ImporterResolver _importerResolver;
        private readonly PASDbContext _pasContext;
        private readonly ExternalImportDbContext _externalImportDbContext;

        public ImportsController(IHostingEnvironment hostingEnvironment, 
                                 AspergillosisContext context,
                                 PASDbContext pasContext, 
                                 ExternalImportDbContext externalImportDbContext,
                                 IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
            _pasContext = pasContext;
            _configuration = configuration;
            _externalImportDbContext = externalImportDbContext;
            _importerResolver = new ImporterResolver(_context);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult New()
        {
            string batchClinicalLettersDir = _hostingEnvironment.ContentRootPath + _configuration.GetSection("clinicLettersDirectory").Value.ToString();
            var batchUploadVM = new BatchUploadViewModel();
            batchUploadVM.ClinicLettersBatchDirectory = batchClinicalLettersDir;
            batchUploadVM.ClinicLettersPdfDirectory = _configuration.GetSection("DataDirectory").Value;
            PopulateDbImportTypesDropdownList();
            return View(batchUploadVM);
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

        [HttpPost]
        public async Task<IActionResult> ExternalDatabaseICD10Diagnosis()
        {
            var allPatients = _context.Patients;
            foreach(var patient in allPatients)
            {
                var icd10Diagnoses = _externalImportDbContext.Diagnoses.Where(d => d.RM2Number.Equals("RM2" + patient.RM2Number));
                if (!icd10Diagnoses.Any()) continue;
                foreach(var diagnosis in icd10Diagnoses)
                {
                    var icd10Diagnosis = new PatientICD10Diagnosis();
                    icd10Diagnosis.DiagnosisCode = diagnosis.DiagnosisCode;
                    icd10Diagnosis.DiagnosisDescription = diagnosis.DiagnosisDescription;
                    icd10Diagnosis.DiagnosisDate = diagnosis.DiagnosisDate;
                    icd10Diagnosis.PatientId = patient.ID;
                    icd10Diagnosis.OriginalImportId = diagnosis.ID;
                    await _context.PatientICD10Diagnoses.AddAsync(icd10Diagnosis);                    
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
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