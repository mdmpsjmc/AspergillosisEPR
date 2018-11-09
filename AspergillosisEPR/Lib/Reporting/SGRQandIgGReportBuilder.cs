using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
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
    public class SGRQandIgGReportBuilder
    {
        private AspergillosisContext _context;
        private XSSFWorkbook _outputWorkbook;
        private XSSFWorkbook _inputWorkbook;
        private FileStream _stream;
        private IFormFile _file;
        private List<string> _outputSheetNames = new List<string>()
        {
            "SGRQ",
            "Aspergillus F IgG",            
        };

        public SGRQandIgGReportBuilder(AspergillosisContext context,
                                              FileStream inputFileStream,
                                              IFormFile formFile)
        {
            _context = context;
            _outputWorkbook = new XSSFWorkbook();
            _stream = inputFileStream;
            _file = formFile;
        }

        public SGRQandIgGReportBuilder(AspergillosisContext context,
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
                                   .Include(p => p.STGQuestionnaires)
                                   .Include(p => p.PatientImmunoglobulines)
                                    .ThenInclude(p => p.ImmunoglobulinType)                                 
                                   .OrderBy(p => p.RM2Number)
                                   .ToList();    
            BuildSGRQTab(patients);
            BuildIgGTab(patients);
            return SerializeWorkbook();
        }

     
    
        private void BuildIgGTab(List<Patient> patients)
        {
            ISheet currentSheet = _outputWorkbook.CreateSheet(_outputSheetNames[1]);
            int itemCount = patients.Select(p => p.PatientImmunoglobulines
                                                   .Where(ig => ig.ImmunoglobulinType.Name.Equals("Aspergillus F IgG")).Count())
                                                   .OrderByDescending(d => d)
                                                   .First();
            BuildIgGHeaders(currentSheet, itemCount);
            for (int patientCursor = 0; patientCursor < patients.Count(); patientCursor++)
            {
                int cellIndex = 0;
                var nextToHeaderRow = patientCursor + 1;
                var currentPatient = patients.ToList()[patientCursor];
                var currentRow = currentSheet.CreateRow(nextToHeaderRow);
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.RM2Number);
                cellIndex++;
                var items = currentPatient.PatientImmunoglobulines
                                          .Where(ig => ig.ImmunoglobulinType.Name.Equals("Aspergillus F IgG"))
                                          .OrderByDescending(q => q.DateTaken)
                                          .ToList();
                if (items.Count == 0) continue;
                for (int index = 0; index < items.Count(); index++)
                {
                    var currentItem = items[index];
                    if (currentItem.DateTaken != null)
                    {
                        var dateTaken = currentItem.DateTaken.ToString("dd/MM/yyyy");
                        currentRow.CreateCell(cellIndex).SetCellValue(dateTaken);
                        cellIndex++;
                    }
                    else
                    {
                        currentRow.CreateCell(cellIndex).SetCellValue("");
                        cellIndex++;
                    }

                    currentRow.CreateCell(cellIndex).SetCellValue(currentItem.Value.ToString());
                    cellIndex++;
                }
            }
        }

        private void BuildIgGHeaders(ISheet currentSheet, int itemCount)
        {
            var headersRow = currentSheet.CreateRow(0);
            var headers = new List<string>()
            {
                "RM2",              
            };

            var repeatItems = new List<string>()
            {
                "DateTaken",  "Result"
            };
            for (int cursor = 1; cursor < itemCount + 1; cursor++)
            {
                headers.Add(repeatItems[0] + cursor);
                headers.Add(repeatItems[1] + cursor);
            }

            MakeHeadersBold(headersRow, headers);
        }

      


        private void BuildSGRQTab(List<Patient> patients)
        {
            ISheet currentSheet = _outputWorkbook.CreateSheet(_outputSheetNames[0]);
            int stgCount = patients.Select(p => p.STGQuestionnaires.Count).OrderByDescending(d => d).First();
            BuildSGRQHeaders(currentSheet, stgCount);

            for (int patientCursor = 0; patientCursor < patients.Count(); patientCursor++)
            {
                int cellIndex = 0;
                var nextToHeaderRow = patientCursor + 1;
                var currentPatient = patients.ToList()[patientCursor];
                var currentRow = currentSheet.CreateRow(nextToHeaderRow);
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.RM2Number);
                cellIndex++;
                var allSGRQ = currentPatient.STGQuestionnaires.OrderByDescending(q => q.DateTaken).ToList();
                if (allSGRQ.Count == 0) continue;
                for (int index = 0; index < allSGRQ.Count(); index++)
                {
                    var currentSGRQ = allSGRQ[index];
                    currentRow.CreateCell(cellIndex).SetCellValue(currentSGRQ.DateTaken.ToString("dd/MM/yyyy"));
                    cellIndex++;
                    currentRow.CreateCell(cellIndex).SetCellValue(Math.Round(currentSGRQ.SymptomScore, 2).ToString());
                    cellIndex++;
                    currentRow.CreateCell(cellIndex).SetCellValue(Math.Round(currentSGRQ.ActivityScore, 2).ToString());
                    cellIndex++;
                    currentRow.CreateCell(cellIndex).SetCellValue(Math.Round(currentSGRQ.ImpactScore, 2).ToString());
                    cellIndex++;
                    currentRow.CreateCell(cellIndex).SetCellValue(Math.Round(currentSGRQ.TotalScore, 2).ToString());
                    cellIndex++;
                }
            }
        }

        private void BuildSGRQHeaders(ISheet currentSheet, int highestSTGCount)
        {
            var headersRow = currentSheet.CreateRow(0);
            var headers = new List<string>()
            {
                "RM2",               
            };

            var repeatItems = new List<string>()
            {
                "DateTaken", "SymptomScore", "ActivityScore", "ÏmpactScore", "TotalScore"
            };
            for (int cursor = 1; cursor < highestSTGCount + 1; cursor++)
            {
                headers.Add(repeatItems[0] + cursor);
                headers.Add(repeatItems[1] + cursor);
                headers.Add(repeatItems[2] + cursor);
                headers.Add(repeatItems[3] + cursor);
                headers.Add(repeatItems[4] + cursor);
            }

            MakeHeadersBold(headersRow, headers);
        }

        private List<string> GetPatientIdentifiers()
        {
            if (_file != null) _file.CopyTo(_stream);
            _stream.Position = 0;
            _inputWorkbook = new XSSFWorkbook(_stream);
            var identifiers = new List<string>();
            var sheet = _inputWorkbook.GetSheetAt(0);
            for (var cursor = 0; cursor < sheet.PhysicalNumberOfRows; cursor++)
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

        private void MakeHeadersBold(IRow headersRow, List<string> headers)
        {
            for (int cursor = 0; cursor < headers.Count; cursor++)
            {
                var header = headers[cursor];
                var headerCell = headersRow.CreateCell(cursor);
                headerCell.SetCellType(NPOI.SS.UserModel.CellType.String);
                ApplyBoldCellStyle(headerCell);
                headerCell.SetCellValue(header);
            }
        }
    }
}
