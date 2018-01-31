using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Exporters
{
    public class PatientDetailsExcelChartGenerator
    {
        private byte[] _spreadsheetToAppendChart;
        private string _sheetChartName;
        private ExcelWorksheet _worksheet;
        private ExcelPackage _package;
        private int _lastChartDataRowIndex;

        public PatientDetailsExcelChartGenerator(byte[] documentByteArray, string sheetChartName, int lastChartDataRowIndex)
        {
            _spreadsheetToAppendChart = documentByteArray;
            _sheetChartName = sheetChartName;
            _lastChartDataRowIndex = lastChartDataRowIndex;
        }

        public byte[] GenerateDocumentWithChart()
        {
            ByteArrayToExcelPackage();
            CreatePatientDetailsChart();
            return SerializeWorkbook();          
        }

        private void CreatePatientDetailsChart()
        {
            _worksheet = _package.Workbook.Worksheets[_sheetChartName];
            var chart = (ExcelLineChart)_worksheet.Drawings.AddChart(_sheetChartName + "_Chart", eChartType.Line3D);
            chart.SetSize(800, 600);
            chart.SetPosition(10, 500);
            chart.Title.Text = _sheetChartName;
            AddPatientDetailsSeries(chart);
        }

        private void AddPatientDetailsSeries(ExcelLineChart chart)
        {
            var seriesLabel = ExcelRange.GetAddress(2, 6, _lastChartDataRowIndex, 6);
            var addressesColumn = new string[]{ "B", "C", "D", "E" };
            for(var cursor = 0; cursor < 4; cursor++)
            {
                var fromColumn = cursor + 2;
                chart.Series.Add(ExcelRange.GetAddress(2, fromColumn, _lastChartDataRowIndex, fromColumn), seriesLabel);
                var addressLetter = addressesColumn[cursor];
                chart.Series[cursor].Header = _worksheet.Cells[addressLetter + "1"].GetValue<string>();
            }            
        }

        private ExcelPackage ByteArrayToExcelPackage()
        {
            using (MemoryStream memStream = new MemoryStream(_spreadsheetToAppendChart))
            {
                memStream.Seek(0, SeekOrigin.Begin);
                _package = new ExcelPackage(memStream);
                return _package;
            }
        }

        private byte[] SerializeWorkbook()
        {
            return _package.GetAsByteArray();
        }
    }
}
