using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using AspergillosisEPR.Models;
using System.Collections;
using System.Reflection;
using AspergillosisEPR.Data;
using Microsoft.EntityFrameworkCore;

namespace AspergillosisEPR.Controllers
{
    [Authorize(Roles="Admin Role")]
    public class ImportsController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly AspergillosisContext _context;
        private List<Patient> _importedPatients;
        private Hashtable _dictonary;
        private List<string> _headers;

        public ImportsController(IHostingEnvironment hostingEnvironment, AspergillosisContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
            _importedPatients = new List<Patient>();
            _dictonary = DbImport.HeadersDictionary();
            _headers = new List<string>();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult New()
        {
            return View();
        }

        public IActionResult Create()
        {
            IFormFile file = Request.Form.Files[0];
            string folderName = "Upload";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (file.Length > 0)
            {
                string fileExtension = Path.GetExtension(file.FileName).ToLower();
                string fullPath = Path.Combine(newPath, file.FileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    ReadDatabaseFile(stream, file, fileExtension);
                }
            }
 
            return Json(new { result = _importedPatients.Count() });
        }

        private void ReadDatabaseFile(FileStream stream, IFormFile file, string fileExtension)
        {
            Patient patient;
            ISheet currentSheet;
            file.CopyTo(stream);
            stream.Position = 0;
            if (fileExtension == ".xls")
            {
                HSSFWorkbook workbook = new HSSFWorkbook(stream);
                currentSheet = workbook.GetSheetAt(0);
            }
            else
            {
                XSSFWorkbook workbook = new XSSFWorkbook(stream);
                currentSheet = workbook.GetSheetAt(0);
            }

            IRow headerRow = currentSheet.GetRow(0); //Get Header Row
            int cellCount = headerRow.LastCellNum;

            GetSpreadsheetHeaders(headerRow);
            
            for (int rowsCursor = (currentSheet.FirstRowNum + 1); rowsCursor <= currentSheet.LastRowNum; rowsCursor++) 
            {
                patient = new Patient();
                IRow row = currentSheet.GetRow(rowsCursor);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                patient  = ReadRowCells(patient, row, cellCount);
                _importedPatients.Add(patient);
            }
            SavePatientsInDatabase();
        }

        
        private Patient ReadRowCells(Patient patient, IRow row, int cellCount)
        {
         for (int j = row.FirstCellNum; j < cellCount; j++)
          {
           if (row.GetCell(j) != null)
           {
             string rowValue = row.GetCell(j).ToString();
             string header = _headers.ElementAt(j);
             string newObjectFields = (string)_dictonary[header];
              if (newObjectFields != null)
              {
                  string[] fields = newObjectFields.Split("|");
                  foreach (string field in fields)
                   {
                    var klassAndField = field.Split(".");
                    switch (klassAndField[0])
                    {
                      case "Patient":
                         Type type = patient.GetType();
                          PropertyInfo propertyInfo = type.GetProperty(klassAndField[1]);
                          if (propertyInfo.PropertyType == typeof(DateTime)) // convert to date if its a date
                          {
                            DateTime dateRowValue = row.GetCell(j).DateCellValue;
                            propertyInfo.SetValue(patient, Convert.ChangeType(dateRowValue, propertyInfo.PropertyType), null);
                           }
                           else
                            {
                             propertyInfo.SetValue(patient, Convert.ChangeType(rowValue, propertyInfo.PropertyType), null);
                            }
                           break;          
                    }
                  }
              }
            }
         }
         return patient; 
        }

        private void GetSpreadsheetHeaders(IRow headerRow)
        {
            for (int headerCellCursor = 0; headerCellCursor < headerRow.LastCellNum; headerCellCursor++)
            {
                ICell headerCell = headerRow.GetCell(headerCellCursor);
                if (headerCell == null || string.IsNullOrWhiteSpace(headerCell.ToString())) continue;
                _headers.Add(headerCell.ToString());
            }
        }

        private void SavePatientsInDatabase()
        {
            foreach (var newPatient in _importedPatients)
            {
                _context.Add(newPatient);
                _context.SaveChanges();
            }
        }

    }
}