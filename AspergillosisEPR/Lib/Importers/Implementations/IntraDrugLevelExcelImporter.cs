using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using System.Collections;
using AspergillosisEPR.Models;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Extensions;
namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class IntraDrugLevelExcelImporter : SpreadsheetImporter
    {
        private static string IMPORTED_DRUG_LEVEL_NAME = "Itraconazole";
        private PatientDrugLevelResolver _resolver;
        private Drug _drug;
        private UnitOfMeasurement _uom;
        private int _patientAliveStatus;
        private int _patientDeceasedStatus;


        public IntraDrugLevelExcelImporter(FileStream stream, 
                                           IFormFile file, 
                                           string fileExtension, AspergillosisContext context) : base(stream, file, fileExtension, context)
        {
            
            _context = context;       
        }

        public static Hashtable HeadersDictionary()
        {
            return new Hashtable()
            {
                  { "SURNAME", "Patient.LastName" },
                  { "RM2Number", "Patient.RM2Number"},
                  { "FORENAME", "Patient.FirstName" },
                  { "RM2", "Patient.RM2Number" },
                  { "NHSNO", "Patient.NhsNumber"},
                  { "DOB", "Patient.DOB"},
                  { "DOD", "Patient.DateOfDeath"}, 
                  { "SEX", "Patient.Gender" },
                  { "DATE TAKEN", "PatientDrugLevel.DateTaken"},
                  { "DATE RECEIVED", "PatientDrugLevel.DateReceived"},
                  { "LAB NO", "PatientDrugLevel.LabNumber" },
                  { "DEATH_INDICATOR", "Patient.PatientStatusId"},
                  { "RESULT -  mg/L", "PatientDrugLevel.ResultValue"}
             };
        }

        protected override void ProcessSheet(ISheet currentSheet)
        {
            Action<Patient, IRow, int> sheetProcessingAction = (patient, row, cellCount) =>
            {
                _drug = _context.Drugs.Where(d => d.Name.Contains(IMPORTED_DRUG_LEVEL_NAME)).FirstOrDefault();
                _uom = _context.UnitOfMeasurements.Where(uom => uom.Name.Contains("mg/L")).FirstOrDefault();
                _patientAliveStatus = _context.PatientStatuses.Where(s => s.Name == "Active").FirstOrDefault().ID;
                _patientDeceasedStatus = _context.PatientStatuses.Where(s => s.Name == "Deceased").FirstOrDefault().ID;

                var patientFromExcel = ReadCellsForPatient(patient, row, cellCount);
                var existingImportedPatient = FindPatientInImported(patientFromExcel);

                if (existingImportedPatient != null)
                {
                    var concatenated = patientFromExcel
                                           .DrugLevels
                                           .ToList()
                                           .Concat(existingImportedPatient.DrugLevels.ToList());

                    existingImportedPatient.DrugLevels = concatenated.ToList();
                    patientFromExcel = existingImportedPatient;
                }

                CheckIfAlreadyImportedInDatabase(patientFromExcel, existingImportedPatient);
            };
            InitializeSheetProcessingForRows(HeadersDictionary(), currentSheet, sheetProcessingAction);
        }

        private void CheckIfAlreadyImportedInDatabase(Patient patientFromExcel, Patient existingImportedPatient)
        {
            var dbPatient = FindPatientInDatabase(patientFromExcel);
            if (dbPatient != null && !string.IsNullOrEmpty(patientFromExcel.NhsNumber)) dbPatient.NhsNumber = patientFromExcel.NhsNumber;

            if (dbPatient != null)
            {
                var excelPatientDrugLevelList = patientFromExcel.DrugLevels.Select(dl => dl.ResultValue).ToList();
                var dbPatientDrugLevelList = dbPatient.DrugLevels.Select(dl => dl.ResultValue).ToList();
                var newDrugLevels = excelPatientDrugLevelList.Except(dbPatientDrugLevelList);

                var toInsertDrugLevels = patientFromExcel.DrugLevels.Where(dl => newDrugLevels.Contains(dl.ResultValue));

                foreach (var drugLevel in toInsertDrugLevels)
                {
                    dbPatient.DrugLevels.Add(drugLevel);
                }
                Imported.Add(dbPatient);
            }
            else
            {
                if (patientFromExcel.IsValid() && existingImportedPatient == null)
                {
                    Imported.Add(patientFromExcel);
                }
            }
        }

        private Patient FindPatientInImported(Patient importedPatient)
        {
            if (importedPatient.FirstName == null || importedPatient.LastName == null || importedPatient.DOB == null)
            {
                return null;
            }
            var exisingImportedPatient = Imported.Where(p => p.RM2Number == importedPatient.RM2Number)                                           
                                        .FirstOrDefault();
            if (exisingImportedPatient != null)
            {
                return exisingImportedPatient;
            }
            else
            {
                return null;
            }
        }

        private Patient FindPatientInDatabase(Patient importedPatient)
        {
            if (importedPatient.FirstName == null || importedPatient.LastName == null || importedPatient.DOB == null)
            {
                return null;
            }
            var dbPatient = _context.Patients.Where(p => p.RM2Number == importedPatient.RM2Number)
                                             .Include(p => p.DrugLevels)
                                             .FirstOrDefault();

            if (dbPatient != null)
            {
                return dbPatient;
            } else
            {
                return null;
            }
        }

        private Patient ReadCellsForPatient(Patient patient, IRow row, int cellCount)
        {
            _resolver = new PatientDrugLevelResolver(_context, _drug, _uom);
            for (int cellCursor = 0; cellCursor < cellCount; cellCursor++)
            {
                
                if (row.GetCell(cellCursor, MissingCellPolicy.CREATE_NULL_AS_BLANK) != null)
                {
                    ReadCell(patient, row, _resolver, cellCursor);
                }
            }
            return _resolver.Resolve(patient);
        }

        private void ReadCell(Patient patient, IRow row, PatientDrugLevelResolver resolver, int cellCursor)
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
                        if (propertyName == "Gender")
                        {
                            if (propertyValue == "F")
                            {
                                propertyValue = "Female";
                            } else if (propertyValue == "M")
                            {
                                propertyValue = "Male";
                            }
                        }
                        string valueToSet = propertyValue.Replace("RM2", String.Empty);
                        if (propertyName == "FirstName" || propertyName == "LastName")
                        {
                            valueToSet = valueToSet.ToUpper();
                        }
                        if (propertyName == "PatientStatusId")
                        {
                            valueToSet = GetPatientStatusId(propertyValue);
                        }
                        SetPatientProperty(patient, propertyName, row, cellCursor, valueToSet);
                        break;
                    case "PatientDrugLevel":
                        resolver.SetProperty(klassAndField[1], propertyValue);
                        break;
                }
            }
        }

        private string GetPatientStatusId(string propertyValue)
        {
            if (propertyValue == "Y")
            {
                propertyValue = _patientDeceasedStatus.ToString();
            }
            else if ((propertyValue == "N") || string.IsNullOrEmpty(propertyValue))
            {
                propertyValue = _patientAliveStatus.ToString();
            }
            return propertyValue;
        }    

        private void SetPatientProperty(Patient patient, string propertyName, 
                                       IRow row, int cellCursor, string propertyValue)
        {
            Type type = patient.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            DateTime dateRowValue;
            try
            {
                if (propertyInfo != null && propertyInfo.PropertyType == typeof(DateTime))
                {
                    dateRowValue = row.GetCell(cellCursor).DateCellValue;
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
            } catch(InvalidOperationException ex)
            {
                propertyInfo.SetValue(patient, null);
            }
        }

        protected override List<string> IdentiferHeaders()
        {
            return new List<string> { "SURNAME", "FORENAME", "DOB" };
        }
    }
}
