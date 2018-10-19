using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Reporting
{
    public class CPAMortalityAuditReportBuilder
    {
        private AspergillosisContext _context;
        private XSSFWorkbook _outputWorkbook;
        private XSSFWorkbook _inputWorkbook;
        private FileStream _stream;
        private IFormFile _file;
        private List<string> _outputSheetNames = new List<string>()
        {
            "Demographics and dates",
            "Diagnoses",
            "SGRQ",
            "MRC",
            "Weight",
            "PFT",
            "Aspergillus F IgG",
            "Total IgE",
            "C-Reactive Protein",
            "Heamoglobin", 
            "WBC",
            "Lymphocytes"
        };

        public CPAMortalityAuditReportBuilder(AspergillosisContext context, 
                                              FileStream inputFileStream,
                                              IFormFile formFile)
        {
            _context = context;            
            _outputWorkbook = new XSSFWorkbook();
            _stream = inputFileStream;
            _file = formFile;
        }

        public CPAMortalityAuditReportBuilder(AspergillosisContext context,
                                            FileStream inputFileStream)
        {
            _context = context;
            _outputWorkbook = new XSSFWorkbook();
            _stream = inputFileStream;
        }

        public byte[] Build()
        {
            var ids = GetPatientIdentifiers();
            var patients = _context.Patients
                                   .Where(p => ids.Contains(p.RM2Number))
                                   .OrderByDescending(p => p.LastName)
                                   .ToList();
            BuildDemographicsTab(patients);
            BuildDiagnosesTab(patients);
            return SerializeWorkbook();
        }

        private void BuildDiagnosesTab(List<Patient> patients)
        {
            ISheet currentSheet = _outputWorkbook.CreateSheet(_outputSheetNames[1]);
            BuildDiagnosisHeaders(currentSheet);
        }

        private void BuildDiagnosisHeaders(ISheet currentSheet)
        {
            var headersRow = currentSheet.CreateRow(0);
            var headers = new List<string>()
            {
                "RM2",
                "Forename",
                "Surname",
                "DiagnosisName",
                "DiagnosisNote", 
                "IsCCPA",
                "IsCFPA",
                "IsAspergilloma",
                "IsBillateral",
                "IsRheumathoidArthritis",
                "IsHIV",
                "IsRenalFailure",
                "IsDiabetes",
                "IsScleroderma",
                "IsGPA",
                "IsChurgStrauss",
                "IsSLE"
            };
            for (int cursor = 0; cursor < headers.Count; cursor++)
            {
                var header = headers[cursor];
                var headerCell = headersRow.CreateCell(cursor);
                headerCell.SetCellType(CellType.String);
                ApplyBoldCellStyle(headerCell);
                headerCell.SetCellValue(header);
            }
        }

        private void BuildDemographicsTab(List<Patient> patients)
        {
            ISheet currentSheet = _outputWorkbook.CreateSheet(_outputSheetNames[0]);
            BuildDemographicsHeaders(currentSheet);
            for(int patientCursor = 0; patientCursor < patients.Count(); patientCursor++)
            {
                var nextToHeaderRow = patientCursor + 1;
                var currentPatient = patients.ToList()[patientCursor];
                var currentRow = currentSheet.CreateRow(nextToHeaderRow);
                _context.Entry(currentPatient).Collection(p => p.PatientNACDates).Load();
                currentRow.CreateCell(0).SetCellValue(currentPatient.RM2Number);
                currentRow.CreateCell(1).SetCellValue(currentPatient.FirstName);
                currentRow.CreateCell(2).SetCellValue(currentPatient.LastName);
                currentRow.CreateCell(3).SetCellValue(currentPatient.Age());
                currentRow.CreateCell(4).SetCellValue(currentPatient.Gender);
                currentRow.CreateCell(5).SetCellValue(currentPatient.PostCode);
                currentRow.CreateCell(6).SetCellValue(Math.Round(currentPatient.DistanceFromWythenshawe,2).ToString() + "m");
                var date = currentPatient.PatientNACDates.FirstOrDefault();
                if (date != null)
                {
                    currentRow.CreateCell(7).SetCellValue(date.FirstSeenAtNAC.ToString("dd/MM/yyyy"));
                    if (date.LastObservationPoint != null) currentRow.CreateCell(8).SetCellValue(date.LastObservationPoint.Value.ToString("dd/MM/yyyy"));
                    if (date.ProbableStartOfDisease != null) currentRow.CreateCell(9).SetCellValue(date.ProbableStartOfDisease.Value.ToString("dd/MM/yyyy"));
                    if (date.DefiniteStartOfDisease != null) currentRow.CreateCell(10).SetCellValue(date.DefiniteStartOfDisease.Value.ToString("dd/MM/yyyy"));
                    if (date.DateOfDiagnosis != null) currentRow.CreateCell(11).SetCellValue(date.DateOfDiagnosis.Value.ToString("dd/MM/yyyy"));
                }
                if (currentPatient.DateOfDeath != null)
                {
                    currentRow.CreateCell(12).SetCellValue(currentPatient.DateOfDeath.Value.ToString("dd/MM/yyyy"));
                }
            }
        }

        private void BuildDemographicsHeaders(ISheet currentSheet)
        {            
            var headersRow = currentSheet.CreateRow(0);
            var headers = new List<string>()
            {
                "RM2",
                "Forename",
                "Surname",
                "Age",
                "Sex",
                "Postcode",
                "DistanceFromWythenshawe",
                "FirstSeenAtNAC", 
                "LastObservationPoint",
                "ProbableStartOfDisease",
                "DefiniteStartOfDisease",
                "DateOfDiagnosis",
                "DateOfDeath"
            };
            for (int cursor = 0; cursor < headers.Count; cursor++)
            {  
                var header = headers[cursor];
                var headerCell = headersRow.CreateCell(cursor);
                headerCell.SetCellType(CellType.String);
                ApplyBoldCellStyle(headerCell);
                headerCell.SetCellValue(header);
            }
        }

        private List<string> GetPatientIdentifiers()
        {
            if (_file != null) _file.CopyTo(_stream);
            _stream.Position = 0;
            _inputWorkbook = new XSSFWorkbook(_stream);
            var identifiers = new List<string>();
            var sheet = _inputWorkbook.GetSheetAt(0);
            for(var cursor = 0; cursor < sheet.PhysicalNumberOfRows; cursor++)
            {
                var currentRow = sheet.GetRow(cursor);
                var rm2Number = currentRow.Cells[0].ToString().Trim();
                identifiers.Add(rm2Number);
            }
            return identifiers;
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
