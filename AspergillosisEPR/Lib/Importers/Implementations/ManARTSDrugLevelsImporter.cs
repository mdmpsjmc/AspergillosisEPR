using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class ManARTSDrugLevelsImporter : SpreadsheetImporter
    {
        public ManARTSDrugLevelsImporter(FileStream stream, IFormFile file,
                                       string fileExtension, AspergillosisContext context) : base(stream, file, fileExtension, context)
        {
            _context = context;
        }

        public static List<string> HeaderNames()
        {
            return new List<string>
            {
                "DrugAssayDate", "DrugAssayDrug", "DrugAssayResult"
            };
        }

        public static Hashtable HeadersDictionary()
        {
            var hashtable = new Hashtable();
            for(int cursor = 1; cursor < 33; cursor++)
            {
                var namesWithCursor = HeaderNames().Select(h => h + cursor.ToString());
                foreach(var name in namesWithCursor)
                {
                    
                    if (name.Contains("DrugAssayDate"))
                    {
                        hashtable.Add(name, "PatientDrugLevel.DateReceived");
                    } else if (name.Contains("DrugAssayDrug"))
                    {
                        hashtable.Add(name, "PatientDrugLevel.DrugId");
                    } else if (name.Contains("DrugAssayResult"))
                    {
                        hashtable.Add(name, "PatientDrugLevel.ResultValue");
                    }
                }
            }
            hashtable.Add("HospNumber", "Patient.RM2Number");
            return hashtable;
        }

        protected override List<string> IdentiferHeaders()
        {
            return new List<string> { "HospNumber" };
        }

        protected override void ProcessSheet(ISheet currentSheet)
        {
            var headerRow = currentSheet.GetRow(0);
            GetSpreadsheetHeaders(headerRow);
            Action<Patient, IRow, int> sheetProcessingAction = (patient, row, cellCount) =>
            {
                if (patient.ID <= 0) return;
                var patientFromExcel = ReadCellsForPatient(patient, row, cellCount);
            };
            InitializeSheetProcessingForRows(HeadersDictionary(), currentSheet, sheetProcessingAction);
        }

        private object ReadCellsForPatient(Patient patient, IRow row, int cellCount)
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
            if (string.IsNullOrEmpty(newObjectFields)) return;
            string headerWithoutNumbers = Regex.Replace(header, @"[\d-]", string.Empty);
            if (header.Equals("DrugAssayDate1"))
            {
                for (int cursor = 1; cursor < 33; cursor++)
                {
                    var namesWithCursor = HeaderNames().Select(h => h + cursor.ToString());
                    var patientDrugLevel = new PatientDrugLevel();
                    patientDrugLevel.UnitOfMeasurementId = 1;
                    foreach (var name in namesWithCursor)
                    {
                        int headerIndex = _headers.IndexOf(name);
                        string klassAndProperty = (string) HeadersDictionary()[name];
                        var klassAndPropertyAry = klassAndProperty.Split(".");
                        ICell cell = row.GetCell(headerIndex, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                        if (name.Contains("DrugAssayDate"))
                        {
                            String stringDate = cell.StringCellValue;
                            if (stringDate == "") continue;
                            var date = DateTime.Parse(stringDate);
                            var existingDrugLevel = _context.PatientDrugLevels
                                                            .Where(dl => dl.PatientId == patient.ID &&
                                                                    dl.DateTaken.Day == date.Day &&
                                                                    dl.DateTaken.Month == date.Month &&
                                                                    dl.DateTaken.Year == date.Year)
                                                             .FirstOrDefault();
                            if (existingDrugLevel != null) patientDrugLevel = existingDrugLevel;                                                            
                            patientDrugLevel.DateReceived = date;
                            patientDrugLevel.DateTaken = date;
                        }
                        else if (name.Contains("DrugAssayDrug"))
                        {
                            var drug = _context.Drugs.FirstOrDefault(d => d.Name.Equals(cell.StringCellValue.Trim()));
                            if (drug == null) continue;
                            patientDrugLevel.DrugId = drug.ID;
                        }
                        else if (name.Contains("DrugAssayResult"))
                        {
                            string result = cell.StringCellValue.Replace("<",String.Empty).Replace(">", String.Empty);
                            if (string.IsNullOrEmpty(result)) continue;
                            patientDrugLevel.ResultValue = Decimal.Parse(result);
                        }
                    }
                    if (patientDrugLevel.DateReceived == null) continue;
                    if (patientDrugLevel.DrugId == 0) continue;
                    patientDrugLevel.Patient = patient;
                    if (patient.DrugLevels == null)
                    {
                        patient.DrugLevels = new List<PatientDrugLevel>();
                    }
                    if (patientDrugLevel.ID.Equals(0))
                    {
                        patient.DrugLevels.Add(patientDrugLevel);
                        _context.PatientDrugLevels.Add(patientDrugLevel);
                    } else
                    {
                        _context.PatientDrugLevels.Update(patientDrugLevel);
                    }
                    Imported.Add(patientDrugLevel);
                }
            }
        }
    }
}
