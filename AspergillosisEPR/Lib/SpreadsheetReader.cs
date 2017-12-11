using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Reflection;
using AspergillosisEPR.Lib;
using AspergillosisEPR.Data;

namespace AspergillosisEPR.Helpers
{
    public class SpreadsheetReader
    {
        private readonly FileStream _stream;
        public IFormFile _file { get; }
        public string _fileExtension { get; }
        public List<Patient> ImportedPatients { get; set; }
        public Hashtable _dictonary { get; private set; }
        public static string OTHER_HEADER_NAME = "OTHER";    
        private List<string> _headers;
        private readonly AspergillosisContext _context;


        public SpreadsheetReader(FileStream stream, IFormFile file, 
                                 string fileExtension,  AspergillosisContext context)
        {
            _stream = stream;
            _file = file;
            _fileExtension = fileExtension;
            ImportedPatients = new List<Patient>();
            _dictonary = DbImport.HeadersDictionary();
            _headers = new List<string>();
            _context = context;
            ReadSpreadsheetFile();
        }

        private void ReadSpreadsheetFile()
        {
            Patient patient;
            ISheet currentSheet;
            _file.CopyTo(_stream);
            _stream.Position = 0;
            if (_fileExtension == ".xls")
            {
                HSSFWorkbook workbook = new HSSFWorkbook(_stream);  //old excel
                currentSheet = workbook.GetSheetAt(0);
            }
            else
            {
                XSSFWorkbook workbook = new XSSFWorkbook(_stream); //new excel
                currentSheet = workbook.GetSheetAt(0);
            }

            IRow headerRow = currentSheet.GetRow(0); //Get Header Row
            int cellCount = headerRow.LastCellNum;

            GetSpreadsheetHeaders(headerRow);

            for (int rowsCursor = (currentSheet.FirstRowNum + 1); rowsCursor <= currentSheet.LastRowNum; rowsCursor++)
            {
                patient = new Patient();
                IRow row = currentSheet.GetRow(rowsCursor);

                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                patient = ReadRowCellsIntoPatientObject(patient, row, cellCount);
                ImportedPatients.Add(patient);
            }
        }


        private Patient ReadRowCellsIntoPatientObject(Patient patient, IRow row, int cellCount)
        {
            List<string> diagnosesNames = new List<string>();
            for (int cellCursor = row.FirstCellNum; cellCursor < cellCount; cellCursor++)
            {
                if (row.GetCell(cellCursor) != null)
                {
                    readCell(patient, row, cellCursor, diagnosesNames);
                }
            }
            var patientDiagnosisResolver = new PatientDiagnosisResolver(patient, diagnosesNames, _context);
            patient.PatientDiagnoses = patientDiagnosisResolver.Resolve();
            return patient;
        }

        private void readCell(Patient patient, IRow row, int cellIndex, List<string> diagnosesNames)
        {
            string header = _headers.ElementAt(cellIndex);
            string newObjectFields = (string)_dictonary[header];
            if (newObjectFields != null)
            {
                string[] fields = newObjectFields.Split("|");
                foreach (string field in fields)
                {
                    var klassAndField = field.Split(".");
                    switch (klassAndField[0])
                    {
                        case "Patient":
                            SetPatientProperty(patient, row, cellIndex, klassAndField[1]);
                            break;
                        case "PatientDiagnosis":
                            var hasDiagnosis = row.GetCell(cellIndex).ToString().Trim();
                            if (hasDiagnosis == "X")
                            {
                                diagnosesNames.Add(_headers[cellIndex]);
                            }
                            if (_headers[cellIndex] == OTHER_HEADER_NAME)
                            {
                                diagnosesNames.Add(hasDiagnosis);
                            }
                            break;
                    }
                }
            }
        }

        private void SetPatientProperty(Patient patient, IRow row, int cellIndex, string property)
        {
            Type type = patient.GetType();
            PropertyInfo propertyInfo = type.GetProperty(property);
            if (propertyInfo.PropertyType == typeof(DateTime)) // convert to date if its a date
            {
                DateTime dateRowValue = row.GetCell(cellIndex).DateCellValue;
                propertyInfo.SetValue(patient, Convert.ChangeType(dateRowValue, propertyInfo.PropertyType), null);
            }
            else
            {
                propertyInfo.SetValue(patient, Convert.ChangeType(row.GetCell(cellIndex).ToString(), propertyInfo.PropertyType), null);
            }
        }

        private void GetSpreadsheetHeaders(IRow headerRow)
        {
            for (int headerCellCursor = 0; headerCellCursor < headerRow.LastCellNum; headerCellCursor++)
            {
                ICell headerCell = headerRow.GetCell(headerCellCursor);
                if (headerCell == null || string.IsNullOrWhiteSpace(headerCell.ToString())) continue;
                _headers.Add(headerCell.ToString());
            }
        }
    }
}
