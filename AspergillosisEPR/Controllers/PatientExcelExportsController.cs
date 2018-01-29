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
            currentSheet.AutoSizeColumn(0);
            currentSheet.AutoSizeColumn(1);
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
    }
}