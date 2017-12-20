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

namespace AspergillosisEPR.Lib.Importers
{
    public class CHSpreadsheetImporter : Importer
    {
        private Hashtable _dictonary { get; set; }
        public static string UNDERLYING_DISEASE_HEADER = "Underlying disease";
        public static string[] IdentifierHeaders = { "HOSPITAL No", "HOSPITAL NUMBER" };
        private List<string> _headers;

        public CHSpreadsheetImporter(FileStream stream, IFormFile file, 
                                 string fileExtension,  AspergillosisContext context)
        {
            _stream = stream;
            _file = file;
            _fileExtension = fileExtension;
            Imported = new List<dynamic>();
            _dictonary = DbImport.HeadersDictionary();
            _headers = new List<string>();
            _context = context;
            ReadSpreadsheetFile();
        }

        private void ReadSpreadsheetFile()
        {
            _file.CopyTo(_stream);
            _stream.Position = 0;
            if (_fileExtension == ".xls")
            {
                HSSFWorkbook workbook = new HSSFWorkbook(_stream);  //old excel
                ProcessSheets(workbook);
            }
            else
            {
                XSSFWorkbook workbook = new XSSFWorkbook(_stream); //new excel
                ProcessSheets(workbook);
            }            
        }

        private void ProcessSheets(XSSFWorkbook workbook)
        {
            for(int tabIndex=0; tabIndex < workbook.NumberOfSheets; tabIndex++)
            {
                ISheet currentSheet = workbook.GetSheetAt(tabIndex);
                ProcessSheet(currentSheet);
            }
        }

        private void ProcessSheets(HSSFWorkbook workbook)
        {
            for (int tabIndex = 0; tabIndex < workbook.NumberOfSheets; tabIndex++)
            {
                ISheet currentSheet = workbook.GetSheetAt(tabIndex);
                ProcessSheet(currentSheet);
            }
        }

        private void ProcessSheet(ISheet currentSheet)
        {
            Patient patient;
            IRow headerRow = currentSheet.GetRow(0); //Get Header Row
            

            GetSpreadsheetHeaders(headerRow);
            int cellCount2= headerRow.Cells.GetRange(0, _headers.Count()).Count;
            int cellCount = headerRow.LastCellNum;

            for (int rowsCursor = (currentSheet.FirstRowNum + 1); rowsCursor <= currentSheet.LastRowNum; rowsCursor++)
            {
                patient = new Patient();
                IRow row = currentSheet.GetRow(rowsCursor);

                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                patient = ReadRowCellsIntoPatientObject(patient, row, cellCount2);
                var existingPatient = ExistingPatient(patient.RM2Number);
                if (existingPatient == null)
                {
                    Imported.Add(patient);
                } else
                {
                    CopyPropertiesFrom(existingPatient, patient);
                }
               
            }
        }

        private void CopyPropertiesFrom(Patient existingPatient, Patient sourcePatient)
        {
            existingPatient.FirstName = sourcePatient.FirstName;
            existingPatient.LastName = sourcePatient.LastName;
            existingPatient.RM2Number = sourcePatient.RM2Number;
            existingPatient.DOB = sourcePatient.DOB;
            existingPatient.Gender = sourcePatient.Gender;
            existingPatient.DateOfDeath = sourcePatient.DateOfDeath;
            existingPatient.PatientStatusId = sourcePatient.PatientStatusId;
            var combinedDiagnoses = sourcePatient.PatientDiagnoses.Concat(existingPatient.PatientDiagnoses).Distinct().ToList();
            existingPatient.PatientDiagnoses = combinedDiagnoses.GroupBy(p => p.DiagnosisTypeId).Select(g => g.First()).ToList();
        }

        public static explicit operator CHSpreadsheetImporter(Type v)
        {
            throw new NotImplementedException();
        }

        private Patient ReadRowCellsIntoPatientObject(Patient patient, IRow row, int cellCount)
        {
            List<string> diagnosesNames = new List<string>();
            for (int cellCursor = row.FirstCellNum; cellCursor < cellCount; cellCursor++)
            {
                if (row.GetCell(cellCursor) != null)
                {
                    ReadCell(patient, row, cellCursor, diagnosesNames);
                }
            }
            var patientDiagnosisResolver = new PatientDiagnosisResolver(patient, diagnosesNames, _context);
            patient.PatientDiagnoses = patientDiagnosisResolver.Resolve();
            return patient;
        }

        private void ReadCell(Patient patient, IRow row, int cellIndex, List<string> diagnosesNames)
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
                            if (_headers[cellIndex] == UNDERLYING_DISEASE_HEADER)
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
            DateTime dateRowValue;
            if (propertyInfo != null && propertyInfo.PropertyType == typeof(DateTime)) // convert to date if its a date
            {
                try
                {
                    dateRowValue = row.GetCell(cellIndex).DateCellValue;
                }
                catch (InvalidDataException ex)
                {
                    dateRowValue = DateTime.Parse(row.GetCell(cellIndex).ToString());
                    Console.WriteLine(ex.Message);
                }                
                propertyInfo.SetValue(patient, Convert.ChangeType(dateRowValue, propertyInfo.PropertyType), null);
            }
            else 
            {
                string stringRowValue = row.GetCell(cellIndex).ToString();
                if (stringRowValue != "")
                {
                    try
                    {
                        propertyInfo.SetValue(patient, Convert.ChangeType(stringRowValue, propertyInfo.PropertyType), null);
                    }
                    catch (InvalidCastException)
                    {// date in a text field
                        dateRowValue = row.GetCell(cellIndex).DateCellValue;
                        propertyInfo.SetValue(patient, dateRowValue);                        
                    }
                }                
            }
        }

        private Patient ExistingPatient(string rRM2Number)
        {
            return Imported.Where(p => p.RM2Number == rRM2Number).FirstOrDefault();
        }

        private void GetSpreadsheetHeaders(IRow headerRow)
        {
            _headers = new List<string>();
            for (int headerCellCursor = 0; headerCellCursor < headerRow.LastCellNum; headerCellCursor++)
            {
                ICell headerCell = headerRow.GetCell(headerCellCursor);
                if (headerCell == null || string.IsNullOrWhiteSpace(headerCell.ToString())) continue;
                _headers.Add(headerCell.ToString());
            }
        }
    }
}
