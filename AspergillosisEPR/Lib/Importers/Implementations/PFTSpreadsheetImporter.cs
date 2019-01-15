using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class PFTSpreadsheetImporter : SpreadsheetImporter
    {
        public PFTSpreadsheetImporter(FileStream stream, IFormFile file,
                                     string fileExtension, AspergillosisContext context) : base(stream, file, fileExtension, context)
        {
            _context = context;
        }

        public static Hashtable HeadersDictionary()
        {
            return new Hashtable()
            {
                  { "Visit Date", "Calculator.DateTaken" },
                  { "Patient ID", "Patient.NhsNumber" },
                  { "Name", "Patient.LastName"},
                  { "Firstname", "Patient.FirstName" },
                  { "Date of Birth", "Patient.DOB"},
                  { "Gender", "Patient.Gender"},
                  { "Age", "Calculator.Age"},
                  { "Height", "PatientMeasurement.Height"},
                  { "Weight", "PatientMeasurement.Weight"},
                  { "DLCO", "Calculator.DLCOValue"},
                  { "FEV1", "Calculator.FEV1Value"},
                  { "FVC", "Calculator.FVCValue"},
                  { "VA", "Calculator.VAValue"},
                  { "KCO", "Calculator.KCOValue"},
             };
        }

        protected override List<string> IdentiferHeaders()
        {
            return new List<string> { "Patient ID", "Date of Birth", "Firstname", "Name" };
        }

        protected override void ProcessSheet(ISheet currentSheet)
        {
            Action<Patient, IRow, int> sheetProcessingAction = (patient, row, cellCount) =>
            {
                string identifierValue = row.Cells[1].StringCellValue;
                var dbPatient = FindByDOBAndNames(row, identifierValue);
                if (dbPatient == null)
                {
                    return;
                } else
                {
                    patient = dbPatient;
                }
                var modifiedPatient = ReadCellsForPatient(patient, row, cellCount);
                if (modifiedPatient.IsValid() && dbPatient != null) Imported.Add(modifiedPatient);
            };
            InitializeSheetProcessingForRows(HeadersDictionary(), currentSheet, sheetProcessingAction);
        }

        private Patient FindByDOBAndNames(IRow row, string identifierValue)
        {
            if (identifierValue.Length < 7)
            {
                var patient = FindPatient(row, identifierValue);
                return patient;

            }
            else
            {
                var nhsNumber = identifierValue.Split(" ").Last();
                var patient = _context.Patients.Where(p => p.NhsNumber == nhsNumber.ToLowerInvariant())
                                               .FirstOrDefault();
                if (patient == null) patient = FindPatient(row, identifierValue.Substring(0, 6));
                return patient;
            }
        }

        private Patient FindPatient(IRow row, string identifierValue)
        {
            DateTime dob;
            try
            {
                dob = DateTime.ParseExact(identifierValue, "ddMMyy", CultureInfo.InvariantCulture);
            }
            catch (System.FormatException ex)
            {
                return null;
            }

            var lastName = row.GetCell(2).ToString();
            var firstName = row.GetCell(4).ToString();
            var patient = _context.Patients.Where(p => p.FirstName.ToLowerInvariant().Equals(firstName.ToLowerInvariant())
                                                  && p.LastName.ToLowerInvariant().Equals(lastName.ToLowerInvariant())
                                                  && p.DOB.Date.Equals(dob.Date))
                                                  .FirstOrDefault();
            return patient;
        }


        private Patient ReadCellsForPatient(Patient patient, IRow row, int cellCount)
        {
            var measurement = new PatientMeasurement();
            measurement.PatientId = patient.ID;
            var pftResolver = new PulmonaryFunctionTestResovler();
            for (int cellCursor = 0; cellCursor < cellCount; cellCursor++)
            {
                if (row.GetCell(cellCursor, MissingCellPolicy.CREATE_NULL_AS_BLANK) != null)
                {
                    ReadCell(patient, measurement, row, pftResolver, cellCursor);
                }
            }
            var pfts = pftResolver.ResolvePFTs(_context, patient);
            _context.PatientPulmonaryFunctionTests.AddRange(pfts);
            return patient;
        }

        private void ReadCell(Patient patient, 
                              PatientMeasurement measurement, 
                              IRow row,
                              PulmonaryFunctionTestResovler pftResolver,
                              int cellCursor)
        {

            string header = _headers.ElementAt(cellCursor);
            string newObjectFields = (string)_dictonary[header];
            string propertyValue = row.GetCell(cellCursor, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();

            if (!string.IsNullOrEmpty(propertyValue) && newObjectFields != null)
            {
                var klassAndField = newObjectFields.Split(".");
                switch (klassAndField[0])
                {
                    case "Patient":
                        string propertyName = klassAndField[1];
                        if (propertyValue.Length > 7 && propertyName == "NhsNumber")
                        {                            
                            propertyValue = propertyValue.Remove(0, 7);
                        }

                        SetPatientProperty(patient, propertyName, row, cellCursor, propertyValue);
                        break;
                    case "PatientMeasurement":                        
                        propertyName = klassAndField[1];                                      
                        SetPatientMeasurementProperty(measurement, pftResolver, propertyName, propertyValue, row, cellCursor);
                        break;
                    case "Calculator":
                        propertyName = klassAndField[1];
                        if (propertyName == "Age")
                        {
                            pftResolver.Age = propertyValue;
                        }
                        else if (propertyName == "DLCOValue")
                        {
                            pftResolver.DLCOValue = propertyValue;
                        }
                        else if (propertyName == "FEV1Value")
                        {
                            pftResolver.FEV1Value = propertyValue;
                        }
                        else if (propertyName == "KCOValue")
                        {
                            pftResolver.KCOValue = propertyValue;
                        }
                        else if (propertyName == "FVCValue")
                        {
                            pftResolver.FVCValue = propertyValue;
                        }
                        else if (propertyName == "VAValue")
                        {
                            pftResolver.VAValue = propertyValue;
                        }
                        else if (propertyName == "DateTaken")
                        {
                            pftResolver.DateTaken = propertyValue;
                            measurement.DateTaken = (DateTime)Convert.ChangeType(propertyValue, typeof(DateTime));
                        }                            
                        break;
                }
            }
        }

        private void SetPatientMeasurementProperty(PatientMeasurement measurement,
                                                   PulmonaryFunctionTestResovler resolver,
                                                   string propertyName, 
                                                   string propertyValue, 
                                                   IRow row, 
                                                   int cellCursor)
        {
            Type type = measurement.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            DateTime dateRowValue;
            try
            {
                if (propertyInfo != null && propertyInfo.PropertyType == typeof(DateTime))
                {
                    dateRowValue = row.GetCell(cellCursor).DateCellValue;
                    propertyInfo.SetValue(measurement, dateRowValue);
                }
                else if (propertyInfo.PropertyType == typeof(int?) || propertyInfo.PropertyType == typeof(int))
                {
                    int propertyIntValue = Int32.Parse(propertyValue);
                    propertyInfo.SetValue(measurement, propertyIntValue);
                }
                else if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
                {
                    propertyInfo.
                         SetValue(measurement, Convert.ChangeType(propertyValue, typeof(DateTime)), null);
                }
                else if (propertyInfo.PropertyType == typeof(Decimal) || propertyInfo.PropertyType == typeof(Decimal?))
                {
                    decimal propertyDecValue = (decimal)Convert.ChangeType(propertyValue, typeof(Decimal));
                    var propertyDecMultValue = propertyDecValue * 100;

                    if (propertyName == "Height")
                    {
                        resolver.Height = propertyDecValue;
                    }
                    propertyInfo.SetValue(measurement, propertyDecMultValue, null);
                }
                else
                {
                    propertyInfo.
                        SetValue(measurement, Convert.ChangeType(propertyValue, propertyInfo.PropertyType), null);
                }
            }
            catch (InvalidOperationException ex)
            {
                propertyInfo.SetValue(measurement, null);
            }
        }


        private void SetPatientProperty(Patient patient, string propertyName, IRow row, int cellIndex, string propertyValue)
        {
            Type type = patient.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            DateTime dateRowValue;
            try
            {
                if (propertyInfo != null && propertyInfo.PropertyType == typeof(DateTime))
                {
                    dateRowValue = row.GetCell(cellIndex).DateCellValue;
                    propertyInfo.SetValue(patient, dateRowValue);
                }
                else if (propertyInfo.PropertyType == typeof(int?) || propertyInfo.PropertyType == typeof(int))
                {
                    int propertyIntValue = Int32.Parse(propertyValue);
                    propertyInfo.SetValue(patient, propertyIntValue);
                }
                else if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
                {
                    propertyInfo.
                         SetValue(patient, Convert.ChangeType(propertyValue, typeof(DateTime)), null);
                }
                else
                {
                    propertyInfo.
                        SetValue(patient, Convert.ChangeType(propertyValue, propertyInfo.PropertyType), null);
                }
            }
            catch (InvalidOperationException ex)
            {
                propertyInfo.SetValue(patient, null);
            }
        }

    }
}
