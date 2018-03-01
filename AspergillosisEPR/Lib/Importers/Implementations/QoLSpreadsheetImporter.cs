using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using System.Collections;
using AspergillosisEPR.Models;
using System.Reflection;
using AspergillosisEPR.Helpers;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class QoLSpreadsheetImporter : SpreadsheetImporter
    {
        private int _patientAliveStatus;
        private int _patientDeceasedStatus;
        private DiagnosisType _cpaDiagnosis;
        private DiagnosisCategory _primaryDiagnosisCat;

        public QoLSpreadsheetImporter(FileStream stream, IFormFile file,
                                string fileExtension, AspergillosisContext context) : base(stream, file, fileExtension, context)

        {
            _context = context;   
        }

        public static Hashtable HeadersDictionary()
        {
            return new Hashtable()
            {
                  { "SURNAME", "Patient.LastName" },
                  { "GIVEN_NAME", "Patient.FirstName" },
                  { "RM22", "Patient.RM2Number" },
                  { "SEX", "Patient.Gender"},
                  { "DOB", "Patient.DOB"},
                  { "DEATH_TIME", "Patient.DateOfDeath"},
                  { "DEATH_INDICATOR", "Patient.PatientStatusId"}
             };
        }

        protected override List<string> IdentiferHeaders()
        {
            return new List<string>() { "RM22" };
        }

        protected override void ProcessSheet(ISheet currentSheet)
        {
            Action<Patient, IRow, int> sheetProcessingAction = (patient, row, cellCount) =>
            {
                _patientAliveStatus = _context.PatientStatuses.Where(s => s.Name == "Active").FirstOrDefault().ID;
                _patientDeceasedStatus = _context.PatientStatuses.Where(s => s.Name == "Deceased").FirstOrDefault().ID;
                _cpaDiagnosis = _context.DiagnosisTypes.Where(dt => dt.Name == "Chronic pulmonary aspergillosis").SingleOrDefault();
                _primaryDiagnosisCat = _context.DiagnosisCategories.Where(dc => dc.CategoryName == "Primary").SingleOrDefault();

                var importedPatient = ReadCellsToPatient(patient, row, cellCount);
                AddPatientDiagnosis(patient);
                if (importedPatient.IsValid()) Imported.Add(importedPatient);

            };
            InitializeSheetProcessingForRows(HeadersDictionary(), currentSheet, sheetProcessingAction);
        }

        private void AddPatientDiagnosis(Patient patient)
        {
            var patientDiagnosis = new PatientDiagnosis();
            patientDiagnosis.DiagnosisCategoryId = _primaryDiagnosisCat.ID;
            patientDiagnosis.DiagnosisTypeId = _cpaDiagnosis.ID;
            patientDiagnosis.PatientId = patient.ID;
            _context.PatientDiagnoses.Add(patientDiagnosis);
        }

        private Patient ReadCellsToPatient(Patient patient, IRow row, int cellCount)
        {
            for (int cellCursor = 0; cellCursor < cellCount; cellCursor++)
            {
                if (row.GetCell(cellCursor, MissingCellPolicy.CREATE_NULL_AS_BLANK) != null)
                {
                    ReadCell(patient, row, cellCursor);
                }
            }
            return patient;
        }

        private void ReadCell(Patient patient, IRow row, int cellIndex)
        {
            string header = _headers.ElementAt(cellIndex);
            string newObjectFields = (string)_dictonary[header];
            string propertyValue = row.GetCell(cellIndex, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();

            if (!string.IsNullOrEmpty(propertyValue) && newObjectFields != null)
            {
                var klassAndField = newObjectFields.Split(".");
                if (klassAndField[0] == "Patient")
                {
                    switch (klassAndField[1])
                    {
                        case "RM2Number":
                        case "FirstName":
                        case "LastName":
                            SetPatientProperty(patient, klassAndField[1], row, cellIndex, propertyValue.Replace("RM2", String.Empty));
                            break;
                        case "Gender":
                            propertyValue = GetValueForGender(propertyValue);
                            SetPatientProperty(patient, klassAndField[1], row, cellIndex, propertyValue);
                            break;
                        case "DateOfDeath":
                            propertyValue = propertyValue.TruncateLongString(8);
                            DateTime parsedDate;
                            DateTime.TryParseExact(propertyValue, 
                                "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out parsedDate);
                            propertyValue = parsedDate.ToString();
                            SetPatientProperty(patient, klassAndField[1], row, cellIndex, propertyValue);
                            break;
                        case "PatientStatusId":
                            propertyValue = GetPatientStatusId(propertyValue);
                            SetPatientProperty(patient, klassAndField[1], row, cellIndex, propertyValue);
                            break;
                        case "DOB":
                            DateTime date;
                            DateTime.TryParseExact(propertyValue,
                                "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out date);
                            propertyValue = date.ToString();
                            SetPatientProperty(patient, klassAndField[1], row, cellIndex, propertyValue);
                            break;
                    }
                }
            }
        }

        private void SetPatientProperty(Patient patient, string property, IRow row,
                                       int cellIndex, string propertyValue)
        {
            Type type = patient.GetType();
            PropertyInfo propertyInfo = type.GetProperty(property);
            if (propertyInfo.PropertyType == typeof(int?) || propertyInfo.PropertyType == typeof(int))
            {
                int propertyIntValue = Int32.Parse(propertyValue);
                propertyInfo.SetValue(patient, propertyIntValue);
            }
            else if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?)) {
                propertyInfo.
                     SetValue(patient, Convert.ChangeType(propertyValue, typeof(DateTime)), null);
            }
            else
            {
                propertyInfo.
                    SetValue(patient, Convert.ChangeType(propertyValue.Replace("RM2",String.Empty),propertyInfo.PropertyType),  null);
            }
        }


        private string GetPatientStatusId(string propertyValue)
        {
            if (propertyValue == "Y")
            {
                propertyValue = _patientDeceasedStatus.ToString();
            }
            else if (propertyValue == "N")
            {
                propertyValue = _patientAliveStatus.ToString();
            }
            return propertyValue;
        }

        private static string GetValueForGender(string propertyValue)
        {
            if (propertyValue == "F")
            {
                propertyValue = "Female";
            }
            else if (propertyValue == "M")
            {
                propertyValue = "Male";
            }

            return propertyValue;
        }

    }
}
