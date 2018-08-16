using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using AspergillosisEPR.Models.Reporting;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Reporting
{
    public class SGRQReportBuilder
    {
        private AspergillosisContext _context;
        private XSSFWorkbook _outputWorkbook;
        private Report _report;
        private static string DEFAULT_SHEET_NAME = "Summary";
        private static List<string> _headers = new List<string>() {
            "RM2", "Date Taken", "Weight", "Total Score"
        };

        public SGRQReportBuilder(AspergillosisContext context, Report report)
        {
            _context = context;
            _report = report;
            _outputWorkbook = new XSSFWorkbook();
        }

        public byte[] Build()
        {
            ISheet currentSheet = _outputWorkbook.CreateSheet(DEFAULT_SHEET_NAME);
            var currentRow = currentSheet.CreateRow(0);
            BuildHeaders(currentRow);
            int totalQuestionnaires = 0;
            for(int patientCursor = 0; patientCursor < GetData().Count; patientCursor++)
            {
                var patient = GetData()[patientCursor];
                var filteredQuestionnaires = FilteredQuestionnaries(patient.STGQuestionnaires.ToList());
                var countedQuestionnaires = filteredQuestionnaires.Count;
                for (int questionnaireCursor = 0; questionnaireCursor < countedQuestionnaires; questionnaireCursor++)
                {
                    var dataRow = currentSheet.CreateRow(totalQuestionnaires + 1);
                    var questionnaire = filteredQuestionnaires[questionnaireCursor];
                    var cell0 = dataRow.CreateCell(0);
                    cell0.SetCellValue(patient.RM2Number);
                    var cell1 = dataRow.CreateCell(1);
                    cell1.SetCellValue(questionnaire.DateTaken.ToString("dd/MM/yyyy"));
                    var cell2 = dataRow.CreateCell(2);
                    cell2.SetCellType(CellType.String);
                    cell2.SetCellValue(FindMeasurementByDate(questionnaire.DateTaken));
                    var cell3 = dataRow.CreateCell(3);
                    cell3.SetCellValue(questionnaire.TotalScore.ToString());
                    totalQuestionnaires = totalQuestionnaires + 1;
                }
            }
            return SerializeWorkbook();
        }

        private List<PatientSTGQuestionnaire> FilteredQuestionnaries(List<PatientSTGQuestionnaire> allQuestionnaires)
        {
            var questionnaires = allQuestionnaires.OrderByDescending(q => q.DateTaken)
                                                  .ToList()
                                                  .Where(m => m.DateTaken > _report.StartDate && m.DateTaken <= _report.EndDate)
                                                  .ToList();
            return questionnaires;
        }

        private string FindMeasurementByDate(DateTime dateTaken)
        {
            var patient = GetData().Where(p => p.PatientMeasurements.Any(q => q.DateTaken == dateTaken))
                                   .FirstOrDefault();
            if (patient != null)
            {
                var measurement = patient.PatientMeasurements.Where(m => m.DateTaken == dateTaken).FirstOrDefault();
                if (measurement != null)
                {
                    return measurement.Weight.ToString();
                }
                else return null;
            }
            else return null;            
        }

        private void BuildHeaders(IRow currentRow)
        {
            for (int cursor = 0; cursor < _headers.Count; cursor++)
            {
                var header = _headers[cursor];
                var headerCell = currentRow.CreateCell(cursor);
                headerCell.SetCellType(CellType.String);
                ApplyBoldCellStyle(headerCell);
                headerCell.SetCellValue(header);
            }
        }

        private List<Patient> GetData()
        {
            var patientIds = _context.PatientReportItems
                                     .Where(r => r.ReportId == _report.ID)
                                     .Select(r => r.PatientId)
                                     .ToList();

            var patients = _context.Patients
                                   .Where(p => patientIds.Contains(p.ID))
                                   .Include(p => p.STGQuestionnaires)
                                   .Include(p => p.PatientMeasurements)
                                   .Where(p => p.STGQuestionnaires.Any(m => m.DateTaken > _report.StartDate && m.DateTaken <= _report.EndDate))                                   
                                   .ToList();
            return patients;
        }

        private void ApplyBoldCellStyle(ICell cell)
        {
            ICellStyle boldFontCellStyle = _outputWorkbook.CreateCellStyle();
            IFont boldFont = _outputWorkbook.CreateFont();
            boldFont.IsBold = true;
            boldFontCellStyle.SetFont(boldFont);
            cell.CellStyle = boldFontCellStyle;
        }

        private byte[] SerializeWorkbook()
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
    }
}
