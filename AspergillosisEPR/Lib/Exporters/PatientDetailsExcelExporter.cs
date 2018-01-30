using AspergillosisEPR.Extensions;
using AspergillosisEPR.Models.PatientViewModels;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Exporters
{
    public class PatientDetailsExcelExporter
    {
        private XSSFWorkbook _outputWorkbook;
        private PatientDetailsViewModel _patientDetailsVM;
        private List<PropertyInfo> _controlDisplayProperties;
        private IFormCollection _form;
        public static string EXPORTED_EXCEL_DIRECTORY = @"\wwwroot\Files\Exported\Excel\";
        public static string EXCEL_2007_CONTENT_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public PatientDetailsExcelExporter(PatientDetailsViewModel patientDetailsViewModel, 
                                      List<PropertyInfo> controlDisplayProperties, IFormCollection form)
        {
            _outputWorkbook = new XSSFWorkbook();
            _patientDetailsVM = patientDetailsViewModel;
            _controlDisplayProperties = controlDisplayProperties;
            _form = form;
        }

        public byte[] ToOutputBytes()
        {
            ISheet currentSheet = _outputWorkbook.CreateSheet("Details");
            SetHorizontalCellValuesFromProperties(_patientDetailsVM.Patient, currentSheet);
            CreateSheetNamesFromSelectedTabs();
            return SerializeWorkbook();
        }

        private void CreateSheetNamesFromSelectedTabs()
        {
            foreach (var propertyInfo in _controlDisplayProperties)
            {
                var propertyName = propertyInfo.Name.ToString().Replace("Show", "");
                var displayKeyValue = _form[propertyInfo.Name];

                if (displayKeyValue == "on")
                {
                    ISheet currentSheet = _outputWorkbook.CreateSheet(propertyName);
                    var items = GetCollectionFromTabName(propertyName);
                    AddCollectionDataToCurrentSheet(items, currentSheet);
                }
            }
        }

        private void AddCollectionDataToCurrentSheet(List<object> items, ISheet currentSheet)
        {
            if (items.Count == 0) return;
            for (var cursor = 0; cursor < items.Count; cursor++)
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
            var currentRow = currentSheet.CreateRow(currentCursor + 1);
            var objectProperties = item.GetType().GetProperties();
            for (var cursor = 0; cursor < objectProperties.Length; cursor++)
            {
                var property = objectProperties[cursor];
                var valueCell = currentRow.CreateCell(cursor);
                var currentType = property.PropertyType;

                SetCellValueFromProperty(item, property, valueCell);
            }
        }

        private void SetCellValueFromProperty(object item, PropertyInfo property, ICell valueCell)
        {
            

            switch (Type.GetTypeCode(property.PropertyType))
            {
                case TypeCode.Decimal:
                    var propertyValue = property.GetValue(item);
                    var fromatted = String.Format("{0:0.00}", propertyValue);
                    valueCell.SetCellType(CellType.String);
                    valueCell.SetCellValue(fromatted);
                    break;

                case TypeCode.Int32:
                    var integerPropertyValue = Convert.ToInt32(property.GetValue(item));
                    valueCell.SetCellValue(integerPropertyValue);
                    valueCell.SetCellType(CellType.Numeric);
                    break;
                case TypeCode.String:
                    var stringPropertyValue = property.GetValue(item)?.ToString();
                    valueCell.SetCellType(CellType.String);
                    valueCell.SetCellValue(stringPropertyValue);
                    break;
                case TypeCode.DateTime:
                    var datePropertyValue = Convert.ToDateTime(property.GetValue(item));
                    valueCell.SetCellValue(datePropertyValue.ToString("dd/MM/yyyy"));
                    break;
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

        private bool PropertyInvalid(PropertyInfo[] objectProperties, int cursor, string propertyValue)
        {
            return (propertyValue == null) || (propertyValue == "") || (objectProperties[cursor].PropertyType.ToString().Contains("Collection"));
        }

        private string headerValue(string propertyName)
        {
            if (propertyName.Length > 3)
            {
                return string.Join(" ", propertyName.SplitCamelCaseArray());
            }
            else
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

        private List<object> GetCollectionFromTabName(string tabName)
        {
            var allDx = _patientDetailsVM.PrimaryDiagnoses.
                                         Concat(_patientDetailsVM.SecondaryDiagnoses).
                                         Concat(_patientDetailsVM.OtherDiagnoses).
                                         Concat(_patientDetailsVM.PastDiagnoses).
                                         ToList();
            var dictionary = new Dictionary<string, List<object>>();
            dictionary.Add("Diagnoses", allDx.ToList<object>());
            dictionary.Add("Drugs", _patientDetailsVM.PatientDrugs.ToList<object>());
            dictionary.Add("SGRQ", _patientDetailsVM.STGQuestionnaires.ToList<object>());
            dictionary.Add("Ig", _patientDetailsVM.PatientImmunoglobulines.ToList<object>());
            return dictionary[tabName];
        }
    }
}
