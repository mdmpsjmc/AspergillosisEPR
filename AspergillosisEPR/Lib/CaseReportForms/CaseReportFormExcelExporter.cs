using AspergillosisEPR.Data;
using AspergillosisEPR.Extensions;
using AspergillosisEPR.Models.CaseReportForms;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.CaseReportForms
{
    public class CaseReportFormExcelExporter
    {
        private AspergillosisContext _context;
        private ISheet _currentSheet;
        private XSSFWorkbook _outputWorkbook;

        public CaseReportFormExcelExporter(AspergillosisContext context, 
                                           ISheet currentSheet,
                                           XSSFWorkbook workbook)
        {
            _context = context;
            _currentSheet = currentSheet;
            _outputWorkbook = workbook;
        }

        public void SetValuesFromProperties(CaseReportFormResult resultForm, 
                                            ISheet currentSheet,
                                            int resultFormIndex)
        {
            var objectProperties = resultForm.GetType().GetProperties();
            string propertyValue = null;
            for (var cursor = 0; cursor < objectProperties.Length; cursor++)
            {
                string propertyName = objectProperties[cursor].Name;
                if (propertyName.Contains("Id")) continue;
                if (NameProperties().Contains(propertyName))
                {
                    var resultFormObject = objectProperties[cursor].GetValue(resultForm);

                    propertyValue = objectProperties[cursor].GetValue(resultForm)
                                                            .GetType()
                                                            .GetProperty("Name")
                                                            .GetValue(resultFormObject)
                                                            .ToString();
                }
                else if (propertyName == "Results")
                {
                    propertyValue = ExtractValuesForResultForm(resultForm, 
                                                               currentSheet, 
                                                               objectProperties, 
                                                               propertyValue, 
                                                               cursor, resultFormIndex);
                }
                else
                {
                    propertyValue = objectProperties[cursor].GetValue(resultForm)?.ToString();
                }
                
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

        private string ExtractValuesForResultForm(CaseReportFormResult resultForm, 
                                                  ISheet currentSheet, 
                                                  PropertyInfo[] objectProperties, 
                                                  string propertyValue, 
                                                  int cursor,
                                                  int resultFormIndex)
        {
            var resultsFormObject = (IEnumerable<CaseReportFormPatientResult>)objectProperties[cursor].GetValue(resultForm);
            for (int resultCursor = 0; resultCursor < resultsFormObject.Count(); resultCursor++)
            {
                var result = resultsFormObject.ToList()[resultCursor];
                int rowNumber = resultCursor + cursor + resultFormIndex;
                var currentRow = currentSheet.CreateRow(rowNumber);
                var labelCell = currentRow.CreateCell(0);
                ApplyBoldCellStyle(labelCell);
                labelCell.SetCellValue(result.Field.Label);
                propertyValue = result.DetermineValue(_context);
                currentRow.CreateCell(1).SetCellValue(propertyValue);
            }

            return propertyValue;
        }

        private List<string> NameProperties()
        {
            return new List<string>()
            { "Form", "Category" };
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

    }
}
