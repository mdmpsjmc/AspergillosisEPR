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
using AspergillosisEPR.Extensions;
using Microsoft.EntityFrameworkCore;
using AspergillosisEPR.Lib.Importers.ManARTS;
using AspergillosisEPR.Helpers;
using System.Reflection;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class ManArtsProcessedFileImporter : SpreadsheetImporter
    {
        private int _patientAliveStatus;
        private int _patientDeceasedStatus;
        private List<DiagnosisType> _dbDiagnoses;
        private List<string> _newDiagnoses;

        public ManArtsProcessedFileImporter(FileStream stream, IFormFile file,
                                            string fileExtension, AspergillosisContext context) : base(stream, file, fileExtension, context)
        {
            _context = context;
        }

        public static Hashtable HeadersDictionary()
        {
            return DiagnosisHeaders.Dictionary();
        }

        protected override List<string> IdentiferHeaders()
        {
            return new List<string> { "RM2" };
        }

        protected override void ProcessSheet(ISheet currentSheet)
        {
            var headerRow = currentSheet.GetRow(0);
            GetSpreadsheetHeaders(headerRow);
            Action<Patient, IRow, int> sheetProcessingAction = (patient, row, cellCount) =>
            {               
                var patientFromExcel = ReadCellsForPatient(patient, row, cellCount);
            };
            InitializeSheetProcessingForRows(HeadersDictionary(), currentSheet, sheetProcessingAction);
        }

        private Patient ReadCellsForPatient(Patient patient, IRow row, int cellCount)
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

        private void ReadCell(Patient patient, IRow row, int cellCursor)
        {
            string header = _headers.ElementAt(cellCursor);
            string newObjectFields = (string)_dictonary[header];
            string propertyValue = row.GetCell(cellCursor, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();                        
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
            else if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
            {
                propertyInfo.
                     SetValue(patient, Convert.ChangeType(propertyValue, typeof(DateTime)), null);
            }
            else
            {
                propertyInfo.
                    SetValue(patient, Convert.ChangeType(propertyValue.Replace("RM2", String.Empty), propertyInfo.PropertyType), null);
            }
        }      

        private Patient FindPatientInDatabase(Patient importedPatient)
        {
            if (string.IsNullOrEmpty(importedPatient.RM2Number))
            {
                return null;
            }
            var dbPatient = _context.Patients.Where(p => p.RM2Number == importedPatient.RM2Number)
                                             .Include(p => p.PatientDiagnoses)
                                                .ThenInclude(pd => pd.DiagnosisType)
                                             .FirstOrDefault();

            if (dbPatient != null)
            {
                return dbPatient;
            }
            else
            {
                return null;
            }
        }
    }
}
