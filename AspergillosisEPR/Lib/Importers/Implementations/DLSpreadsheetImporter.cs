using NPOI.SS.UserModel;
using System.IO;
using Microsoft.AspNetCore.Http;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using System.Collections;
using System.Linq;
using System;
using System.Reflection;

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
                  { "STG Symptoms", "PatientSTGQuestionnaire"},
                  { "STG Impact", "PatientSTGQuestionnaire"},
                  { "STG Activity", "PatientSTGQuestionnaire"},
                  { "STG Total", "PatientSTGQuestionnaire"},
                  { "STG Complettion Date" , "PatientSTGQuestionnaire"},
                  { "Bronchiectasis" , "PatientDiagnosis" }
             };
        }

        protected override void ProcessSheet(ISheet currentSheet)
        {
            Action<Patient, IRow, int> sheetProcessingAction = (patient, row, cellCount) =>
            {
                var modifiedPatient = ReadCellsForPatient(patient, row, cellCount);
                //Imported.Add(modifiedPatient);
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

        private void ReadCell(Patient patient, IRow row, int cellIndex)
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
                        SetPatientProperty(patient, propertyName, row, cellIndex, propertyValue);
                        break;
                    case "PatientDiagnosis":
                        var resolver = new PatientDiagnosisResolver(patient, _context);
                        if (propertyValue == "2")
                        {
                            var diagnoses = resolver.ResolveForName(header, "Underlying Diagnosis");
                            Console.WriteLine(diagnoses);
                        }                                                
                        break;
                    case "PatientSTGQuestionnaire":
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
                propertyInfo.SetValue(patient, Convert.ChangeType(propertyValue, propertyInfo.PropertyType), null);
            }
        }

        private bool FirstTwoCellsNotEmpty(IRow row)
        {
            return row.GetCell(0).CellType != CellType.Blank && row.GetCell(1).CellType != CellType.Blank;
        }
    }
}
