using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AspergillosisEPR.Data;
using System.Collections;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class CHSpreadsheetImporter : SpreadsheetImporter
    {
        public static string UNDERLYING_DISEASE_HEADER = "Underlying disease";
        public static string[] IdentifierHeaders = { "HOSPITAL No", "HOSPITAL NUMBER" };
        

        public CHSpreadsheetImporter(FileStream stream, IFormFile file, 
                                 string fileExtension,  AspergillosisContext context) : base(stream, file, fileExtension, context)

        {
        }

        public static Hashtable HeadersDictionary()
        {
            return new Hashtable()
            {
                  { "SURNAME", "Patient.LastName" },
                  { "FORENAME", "Patient.FirstName" },
                  { "FIRST NAME", "Patient.FirstName"},
                  { "HOSPITAL No", "Patient.RM2Number" },
                  { "SEX", "Patient.Gender"},
                  { "Sex", "Patient.Gender"},
                  { "DOB", "Patient.DOB"},
                  { "Date of death", "Patient.DateOfDeath"},
                  { "HOSPITAL NUMBER", "Patient.RM2Number"},
                  { "CCPA", "PatientDiagnosis"},
                  { "ABPA", "PatientDiagnosis"},
                  { "SAFS", "PatientDiagnosis"},
                  { "OTHER","PatientDiagnosis"},
                  { "Underlying disease", "PatientDiagnosis" }
             };
        }

        protected override void ProcessSheet(ISheet currentSheet)
        {
            Action<Patient, IRow, int> sheetProcessingAction = (patient, row, cellCount) =>
            {
                patient = ReadRowCellsIntoPatientObject(patient, row, cellCount);
                var existingPatient = ExistingPatient(patient.RM2Number);
                if (existingPatient == null)
                {
                    Imported.Add(patient);
                }
                else
                {
                    CopyPropertiesFrom(existingPatient, patient);
                }
            };
            InitializeSheetProcessingForRows(HeadersDictionary(), currentSheet, sheetProcessingAction);
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

        protected override List<string> IdentiferHeaders()
        {
            return IdentifierHeaders.ToList();
        }
    }
}
