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

namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class DLSpreadsheetImporter : SpreadsheetImporter
    {
        public DLSpreadsheetImporter(FileStream stream, IFormFile file,
                                string fileExtension, AspergillosisContext context) : base(stream, file, fileExtension, context)

        {
           
        }

        public static Hashtable HeadersDictionary()
        {
            return new Hashtable()
            {
                  { "Surname", "Patient.LastName" },
                  { "Forname", "Patient.FirstName" },
                  { "RM2", "Patient.RM2Number" },
                  { "Sex", "Patient.Gender"},
                  { "DoB", "Patient.DOB"},
                  { "STG Symptoms", "PatientSTGQuestionnaire.SymptomScore"},
                  { "STG Impact", "PatientSTGQuestionnaire.ImpactScore"},
                  { "STG Activity", "PatientSTGQuestionnaire.ActivityScore"},
                  { "STG Total", "PatientSTGQuestionnaire.TotalScore"},
                  { "STG Complettion Date" , "PatientSTGQuestionnaire.DateTaken"},
                  { "Bronchiectasis" , "PatientDiagnosis" }
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
            var stgResolver = new PatientSTGQuestionnaireResolver(_context);
            for (int cellCursor = 0; cellCursor < cellCount; cellCursor++)
            {
                if (row.GetCell(cellCursor, MissingCellPolicy.CREATE_NULL_AS_BLANK) != null)
                {
                    ReadCell(patient, row, stgResolver, cellCursor);
                }
            }
            var questionnaires = stgResolver.Resolve();
            patient.STGQuestionnaires = questionnaires;
            return patient;
        }

        private void ReadCell(Patient patient, IRow row, PatientSTGQuestionnaireResolver resolver, int cellIndex)
        {
            string header = _headers.ElementAt(cellIndex);
            string newObjectFields = (string)_dictonary[header];       
            string propertyValue = row.GetCell(cellIndex, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();

            if (!string.IsNullOrEmpty(propertyValue) && newObjectFields != null && FirstTwoCellsNotEmpty(row))
            {
                var klassAndField = newObjectFields.Split(".");
                switch (klassAndField[0])
                {
                    case "Patient":
                        string propertyName = klassAndField[1];
                        if (propertyName == "Gender")
                        {
                            if (propertyValue == "2")
                            {
                                propertyValue = "Male";
                            } else if (propertyValue == "1")
                            {
                                propertyValue = "Female";
                            }
                        }
                        SetPatientProperty(patient, propertyName, row, cellIndex, propertyValue);
                        break;
                    case "PatientDiagnosis":                       
                        if (propertyValue == "1")
                        {
                            var diagnosesResolver = new PatientDiagnosisResolver(patient, _context);
                            var diagnoses = diagnosesResolver.ResolveForName(header, "Underlying Diagnosis");
                        }                                                
                        break;
                    case "PatientSTGQuestionnaire":
                        string columnName = klassAndField[1];
                        resolver.SetQuestionnaireProperty(columnName, propertyValue);
                        break;
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