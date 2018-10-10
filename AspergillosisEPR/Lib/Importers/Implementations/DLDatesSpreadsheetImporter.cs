using NPOI.SS.UserModel;
using System.IO;
using Microsoft.AspNetCore.Http;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using System.Collections;
using System.Linq;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using AspergillosisEPR.Models.Patients;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class DLDatesSpreadsheetImporter : SpreadsheetImporter
    {
        public DLDatesSpreadsheetImporter(FileStream stream, IFormFile file,
                                string fileExtension, AspergillosisContext context) : base(stream, file, fileExtension, context)

        {
           
        }

        public static Hashtable HeadersDictionary()
        {
            return new Hashtable()
            {
                  { "Probable start of disease", "PatientNACDates.ProbableStartOfDisease"},
                  { "Definite start of disease", "PatientNACDates.DefiniteStartOfDisease"},
                  { "Date of diagnosis", "PatientNACDates.DateOfDiagnosis"},
                  { "Date of 1st seen at NAC", "PatientNACDates.FirstSeenAtNAC"},
                  { "Band of CPA on Referal" , "PatientNACDates.CPABand"},
             };
        }

        protected override void ProcessSheet(ISheet currentSheet)
        {
            Action<Patient, IRow, int> sheetProcessingAction = (patient, row, cellCount) =>
            {
                var modifiedPatient = ReadCellsForPatient(patient, row, cellCount);
                if (modifiedPatient.IsValid()) Imported.Add(modifiedPatient); 
            };
            InitializeSheetProcessingForRows(HeadersDictionary(), currentSheet, sheetProcessingAction);
        }

        private Patient ReadCellsForPatient(Patient patient, IRow row, int cellCount)
        {
            var nacDates = new PatientNACDates();
            nacDates.PatientId = patient.ID;
            for (int cellCursor = 0; cellCursor < cellCount; cellCursor++)
            {
                if (row.GetCell(cellCursor, MissingCellPolicy.CREATE_NULL_AS_BLANK) != null)
                {
                    ReadCell(patient, row, nacDates, cellCursor);
                }
            }
            patient.PatientNACDates.Add(nacDates);
            return patient;
        }

        private void ReadCell(Patient patient, IRow row, PatientNACDates nacDates, int cellIndex)
        {
            string header = _headers.ElementAt(cellIndex);
            string newObjectFields = (string)_dictonary[header];       
            string propertyValue = row.GetCell(cellIndex, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();

            if (!string.IsNullOrEmpty(propertyValue) && newObjectFields != null && FirstTwoCellsNotEmpty(row))
            {
                var klassAndField = newObjectFields.Split(".");
                string propertyName = klassAndField[1];
                if (klassAndField[0] == "PatientNACDates")
                {

                    if (propertyName.Contains("CPA"))
                    {
                        var cpaBand = Int32.Parse(propertyValue);
                        nacDates.CPABand = cpaBand;
                    } else
                    {
                        Type type = nacDates.GetType();
                        PropertyInfo propertyInfo = type.GetProperty(propertyName);
                        if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?)) {
                            propertyInfo.
                                 SetValue(nacDates, Convert.ChangeType(propertyValue, typeof(DateTime)), null);
                        }                        
                    }                 
                }
            }
        }

        private void SetPatientProperty(Patient patient, string property, IRow row, 
                                        int cellIndex, string propertyValue)
        {
            Type type = patient.GetType();
            PropertyInfo propertyInfo = type.GetProperty(property);
            DateTime dateRowValue;
            if (propertyInfo != null && propertyInfo.PropertyType == typeof(DateTime))
            {
                dateRowValue = row.GetCell(cellIndex).DateCellValue;
                propertyInfo.SetValue(patient, dateRowValue);
            }
            else
            {
                propertyInfo.
                    SetValue(patient, Convert.ChangeType(RemoveUnwantedChars(property, propertyValue), 
                                                         propertyInfo.PropertyType), null);
            }
        }

        private string RemoveUnwantedChars(string propertyName, string propertyValue)
        {
            if (propertyName == "RM2Number") {
                Regex digitsOnly = new Regex(@"[^\d]");
                return digitsOnly.Replace(propertyValue, "");
            }
            return propertyValue.Replace("rpt", "").Replace("two", "").Trim();
        }

        private bool FirstTwoCellsNotEmpty(IRow row)
        {
            return row.GetCell(0).CellType != CellType.Blank && row.GetCell(1).CellType != CellType.Blank;
        }

        protected override List<string> IdentiferHeaders()
        {
            return new List<string> { "RM2" };
        }
    }
}