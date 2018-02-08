using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Models.PatientViewModels;

namespace AspergillosisEPR.Lib.Exporters
{
    public class PatientDetailsExcelChartGenerator
    {
        private byte[] _spreadsheetToAppendChart;
        public string IgChartName { get; set; }
        public string SGRQChartName { get; set; }
        private ExcelWorksheet _worksheet;
        private ExcelPackage _package;
        private int _lastSGRQChartDataRowIndex;
        private List<string> _chartNames;
        private PatientDetailsViewModel _patientDetailsVM;

        public PatientDetailsExcelChartGenerator(byte[] documentByteArray, 
                                                 List<string> chartNames, 
                                                 PatientDetailsViewModel patientDetailsViewModel)
        {
            _spreadsheetToAppendChart = documentByteArray;
            _chartNames = chartNames;
            _patientDetailsVM = patientDetailsViewModel;
            _lastSGRQChartDataRowIndex = patientDetailsViewModel.STGQuestionnaires.Count + 1;
            SGRQChartName = chartNames.Where(n => n.Contains("SGRQ")).FirstOrDefault();
            IgChartName = chartNames.Where(n => n.Contains("Ig")).FirstOrDefault();
        }

        public byte[] GenerateDocumentWithChart()
        {
            ByteArrayToExcelPackage();
            CreatePatientsSGRQChart();
            CreatePatientsIgCharts();
            return SerializeWorkbook();          
        }

        private void CreatePatientsSGRQChart()
        {
            if (SGRQChartName == null) return;
            _worksheet = _package.Workbook.Worksheets[SGRQChartName];
            var chart = (ExcelLineChart)_worksheet.Drawings.AddChart(SGRQChartName + "_Chart", eChartType.Line3D);
            chart.SetSize(800, 600);
            chart.SetPosition(10, 500);
            chart.Title.Text = SGRQChartName;
            AddPatientDetailsSGRQSeries(chart);
        }

        public void CreatePatientsIgCharts()
        {
            if (IgChartName == null) return;
            _worksheet = _package.Workbook.Worksheets[IgChartName];
            AddPatientDetailsIgSeries();
        }

        private void AddPatientDetailsIgSeries()
        {
            var groupedIgSeries = _patientDetailsVM.PatientImmunoglobulines.
                                                    GroupBy(pi => pi.ImmunoglobulinTypeId).
                                                    ToList();
            int startRowIndex = 0;
            int endRowIndex = 0;
            for(var cursor = 0; cursor < groupedIgSeries.Count; cursor++)
            {
                var chart = (ExcelLineChart)_worksheet.Drawings.AddChart(IgChartName + cursor + "_Chart", eChartType.Line3D);
                chart.SetSize(400, 300);
                chart.SetPosition(10+(cursor*300), 500);
               
               var groupNumber = cursor + 1;               
               var listGroup = groupedIgSeries[cursor];
               var listGroupCount = listGroup.Count();
               
               if (cursor == 0)
                {
                    startRowIndex = groupNumber + 1;
                    endRowIndex = groupNumber + listGroupCount;
                }
                else
                {
                    startRowIndex = startRowIndex + groupedIgSeries[cursor - 1].Count();
                    endRowIndex = endRowIndex + listGroupCount;
                }
                
                chart.Series.Add(ExcelRange.GetAddress(startRowIndex, 4, endRowIndex, 4),
                               ExcelRange.GetAddress(startRowIndex, 3, endRowIndex, 3));
                string title = _worksheet.Cells["B" + startRowIndex.ToString()].
                                                         GetValue<string>();
                chart.Series[0].Header = title;
                chart.Title.Text = title;
            }                                  
        }

        private void AddPatientDetailsSGRQSeries(ExcelLineChart chart)
        {
            var seriesLabel = ExcelRange.GetAddress(2, 6, _lastSGRQChartDataRowIndex, 6);
            var addressesColumn = new string[]{ "B", "C", "D", "E" };
            for(var cursor = 0; cursor < 4; cursor++)
            {
                var fromColumn = cursor + 2;
                chart.Series.Add(ExcelRange.GetAddress(2, fromColumn, _lastSGRQChartDataRowIndex, fromColumn), seriesLabel);
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
