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
            _context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT Patients ON;");
            IFormFile file = Request.Form.Files[0];
            Hashtable dictionary = DbImport.HeadersDictionary();
            var importedPatients = new List<Patient>();
            Patient patient;
            DiagnosisType diagnosisType;
            DiagnosisCategory diagnosisCategory;
            string folderName = "Upload";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            //StringBuilder sb = new StringBuilder();
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (file.Length > 0)
            {
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                ISheet currentSheet;
                string fullPath = Path.Combine(newPath, file.FileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (sFileExtension == ".xls")
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
                    var headers = new List<string>();
                    for (int headerCellCursor = 0; headerCellCursor < cellCount; headerCellCursor++)
                    {
                        ICell headerCell = headerRow.GetCell(headerCellCursor);
                        if (headerCell == null || string.IsNullOrWhiteSpace(headerCell.ToString())) continue;
                        headers.Add(headerCell.ToString());
                    }
                    for (int rowsCursor = (currentSheet.FirstRowNum + 1); rowsCursor <= currentSheet.LastRowNum; rowsCursor++) //Read Excel File
                    {
                        IRow row = currentSheet.GetRow(rowsCursor);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                            {
                                patient = new Patient();
                                string rowValue = row.GetCell(j).ToString();
                                string header = headers.ElementAt(j);
                                string newObjectFields = (string) dictionary[header];
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
                                                } else
                                                {
                                                    propertyInfo.SetValue(patient, Convert.ChangeType(rowValue, propertyInfo.PropertyType), null);
                                                }
                                                break;
                                        }

                                    }
                                }
                                importedPatients.Add(patient);
                                _context.Add(patient);
                                await _context.SaveChangesAsync();
                            }                            
                        }
                        
                    }
                    //sb.Append("</table>");                    
                }
            }
            _context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT  [dbo].[Patients] OFF");
            return Json(new { result = importedPatients.Count() });
        }
    }
}