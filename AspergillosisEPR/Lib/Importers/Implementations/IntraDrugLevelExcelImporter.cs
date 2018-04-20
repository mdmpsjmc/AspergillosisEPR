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

namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class IntraDrugLevelExcelImporter : SpreadsheetImporter
    {
        private static string IMPORTED_DRUG_LEVEL_NAME = "Itraconazole";
        private PatientDrugLevelResolver _resolver;
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
                  { "FORENAME", "Patient.FirstName" },
                  { "RM2", "Patient.RM2Number" },
                  { "NHSNO", "Patient.NhsNumber"},
                  { "DOB", "Patient.DOB"},
                  { "DATE TAKEN", "PatientDrugLevel.DateTaken"},
                  { "DATE RECEIVED", "PatientDrugLevel.DateReceived"},
                  { "RESULT -  mg/L", "PatientDrugLevel.ResultValue"}
             };
        }

        protected override void ProcessSheet(ISheet currentSheet)
        {
            Action<Patient, IRow, int> sheetProcessingAction = (patient, row, cellCount) =>
            {
                var patientFromExcel = ReadCellsForPatient(patient, row, cellCount);
                var existingImportedPatient = FindPatientInImported(patientFromExcel);

                if (existingImportedPatient != null)
                {
                    var concatenated = patientFromExcel.DrugLevels.ToList().Concat(existingImportedPatient.DrugLevels.ToList());
                    existingImportedPatient.DrugLevels = concatenated.ToList();
                    patientFromExcel = existingImportedPatient;
                }

                CompareIfAlreadyImportedInDatabase(patientFromExcel, existingImportedPatient);
            };
            InitializeSheetProcessingForRows(HeadersDictionary(), currentSheet, sheetProcessingAction);
        }

        private void CompareIfAlreadyImportedInDatabase(Patient patientFromExcel, Patient existingImportedPatient)
        {
            var dbPatient = FindPatientInDatabase(patientFromExcel);
            if (dbPatient != null)
            {
                var drugLevels = patientFromExcel.DrugLevels.Select(dl => dl.ResultValue).ToList()
                                                                   .Except(dbPatient.DrugLevels.Select(dl => dl.ResultValue).ToList());
                var toInsertDrugLevels = patientFromExcel.DrugLevels.Where(dl => drugLevels.Contains(dl.ResultValue));
                foreach (var drugLevel in toInsertDrugLevels)
                {
                    dbPatient.DrugLevels.Add(drugLevel);
                }
                Imported.Add(dbPatient);
            }
            else
            {
                patientFromExcel.RM2Number = "0";
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
            var exisingImportedPatient = Imported.Where(p => p.FirstName.ToLower() == importedPatient.FirstName.ToLower()
                                                            && p.LastName.ToLower() == importedPatient.LastName.ToLower()
                                                              && p.DOB.Date == importedPatient.DOB.Date)                                           
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
            var dbPatient = _context.Patients.Where(p => p.FirstName.ToLower() == importedPatient.FirstName.ToLower()
                                                      && p.LastName.ToLower() == importedPatient.LastName.ToLower()
                                                      && p.DOB.Date == importedPatient.DOB.Date)
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
            _resolver = new PatientDrugLevelResolver(_context, IMPORTED_DRUG_LEVEL_NAME);
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
                        SetPatientProperty(patient, propertyName, row, cellCursor, propertyValue);
                        break;
                    case "PatientDrugLevel":
                        resolver.SetProperty(klassAndField[1], propertyValue);
                        break;
                }
            }
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
