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
            _outputWorkbook = new XSSFWorkbook();
            ISheet currentSheet = _outputWorkbook.CreateSheet("Details");
            SetHorizontalCellValuesFromProperties(patientDetailsVM.Patient, currentSheet);
            CreateSheetNamesFromSelectedTabs(patientDetailsVM);
            byte[] outputBytes = SerializeWorkbook();
            return GetFileContentResult(outputBytes, ".xlsx", EXCEL_2007_CONTENT_TYPE);
        }

        private void CreateSheetNamesFromSelectedTabs(PatientDetailsViewModel patientDetailsVM)
        {
            foreach (var propertyInfo in DetailsDisplayControlProperties())
            {
                var propertyName = propertyInfo.Name.ToString().Replace("Show", "");
                var displayKeyValue = Request.Form[propertyInfo.Name];

                if (displayKeyValue == "on")
                {
                    ISheet currentSheet = _outputWorkbook.CreateSheet(propertyName);
                    var items = GetCollectionFromTabName(patientDetailsVM, propertyName);
                    _outputWorkbook.SetActiveSheet(1); 
                    AddCollectionDataToCurrentSheet(items, currentSheet);
                }
            }
        }

        private void AddCollectionDataToCurrentSheet(List<object> items, ISheet currentSheet)
        {
            if (items.Count == 0) return;
            for(var cursor = 0; cursor < items.Count; cursor++)
            {
                if (cursor == 0)
                {
                    currentSheet = CreateHeaders(items[cursor], currentSheet);
                }
                AddRowFrom(items[cursor], currentSheet, cursor);
            }
        }

        private ISheet CreateHeaders(object item, ISheet currentSheet)
        {
            var currentRow = currentSheet.CreateRow(0);
            var objectProperties = item.GetType().GetProperties();
            for (var cursor = 0; cursor < objectProperties.Length; cursor++)
            {
                var property = objectProperties[cursor];
                var headerCell = currentRow.CreateCell(cursor);
                var cellValue = property.Name;                
                headerCell.SetCellType(CellType.String);
                ApplyBoldCellStyle(headerCell);
                headerCell.SetCellValue(headerValue(cellValue));
            }
            return currentSheet;
        }

        private void AddRowFrom(object item, ISheet currentSheet, int currentCursor)
        {
            var currentRow = currentSheet.CreateRow(currentCursor+1);
            var objectProperties = item.GetType().GetProperties();
            for (var cursor = 0; cursor < objectProperties.Length; cursor++)
            {
                var property = objectProperties[cursor];
                var valueCell = currentRow.CreateCell(cursor);
                var currentType = property.PropertyType;
                if (property.PropertyType == typeof(String))
                {
                    var propertyValue = property.GetValue(item)?.ToString();
                    valueCell.SetCellType(CellType.String);
                    valueCell.SetCellValue(propertyValue);
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    var propertyValue = Convert.ToDateTime(property.GetValue(item));                   
                    valueCell.SetCellValue(propertyValue);                    
                }
                else if (property.PropertyType == typeof(Decimal))
                {
                    var propertyValue = property.GetValue(item);
                    var fromatted = String.Format("{0:0.00}", propertyValue);
                    valueCell.SetCellType(CellType.String);
                    valueCell.SetCellValue(fromatted);
                }
                else if (property.PropertyType == typeof(Int32))
                {
                    var propertyValue = Convert.ToInt32(property.GetValue(item));
                    valueCell.SetCellValue(propertyValue);
                    valueCell.SetCellType(CellType.Numeric);
                }
            }
        }

        public byte[] SerializeWorkbook()
        {
            NpoiMemoryStream ms = new NpoiMemoryStream();
            using (NpoiMemoryStream tempStream = new NpoiMemoryStream())
            {
                tempStream.AllowClose = false;
                _outputWorkbook.Write(tempStream);
                tempStream.Flush();
                tempStream.Seek(0, SeekOrigin.Begin);
                tempStream.AllowClose = true;
                var byteArray = tempStream.ToArray();
                ms.Write(byteArray, 0, byteArray.Length);
                return ms.ToArray();
            }
        }

        public void SetHorizontalCellValuesFromProperties(object objectToQuery, ISheet currentSheet)
        {
            var objectProperties = objectToQuery.GetType().GetProperties();
            for (var cursor = 0; cursor < objectProperties.Length; cursor++)
            {
                string propertyName = objectProperties[cursor].Name;
                if (propertyName.Contains("Id")) continue;
                string propertyValue = objectProperties[cursor].GetValue(objectToQuery)?.ToString();
                if (PropertyInvalid(objectProperties, cursor, propertyValue)) continue;
                if (!PropertyInvalid(objectProperties, cursor, propertyValue))
                {
                    var currentRow = currentSheet.CreateRow(cursor);
                    var labelCell = currentRow.CreateCell(0);
                    ApplyBoldCellStyle(labelCell);
                    labelCell.SetCellValue(headerValue(propertyName));
                    currentRow.CreateCell(1).SetCellValue(propertyValue);
                }
            }
            currentSheet.AutoSizeColumn(0);
            currentSheet.AutoSizeColumn(1);
        }

        private bool PropertyInvalid(System.Reflection.PropertyInfo[] objectProperties, int cursor, string propertyValue)
        {
            return (propertyValue == null) || (propertyValue == "") || (objectProperties[cursor].PropertyType.ToString().Contains("Collection"));
        }

        private string headerValue(string propertyName)
        {
            if (propertyName.Length > 3)
            {
                return string.Join(" ", propertyName.SplitCamelCaseArray());
            } else
            {
                return propertyName;
            }
        }

        private void ApplyBoldCellStyle(ICell cell)
        {
            ICellStyle boldFontCellStyle = _outputWorkbook.CreateCellStyle();
            IFont boldFont = _outputWorkbook.CreateFont();
            boldFont.IsBold = true;
            boldFontCellStyle.SetFont(boldFont);
            cell.CellStyle = boldFontCellStyle;
        }

        private List<object> GetCollectionFromTabName(PatientDetailsViewModel patientDetailsVM, string tabName)
        {
            var allDx = patientDetailsVM.PrimaryDiagnoses.
                                         Concat(patientDetailsVM.SecondaryDiagnoses).
                                         Concat(patientDetailsVM.OtherDiagnoses).
                                         Concat(patientDetailsVM.PastDiagnoses).
                                         ToList();
            var dictionary = new Dictionary<string, List<object>>();
            dictionary.Add("Diagnoses", allDx.ToList<object>());
            dictionary.Add("Drugs", patientDetailsVM.PatientDrugs.ToList<object>());
            dictionary.Add("SGRQ",patientDetailsVM.STGQuestionnaires.ToList<object>());
            dictionary.Add("Ig", patientDetailsVM.PatientImmunoglobulines.ToList<object>());
            return dictionary[tabName];
        }
    }
}