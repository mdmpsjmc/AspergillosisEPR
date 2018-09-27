using AspergillosisEPR.Data;
using AspergillosisEPR.Lib.Importers.ManARTS;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class ManARTSPFTAndHeamatologyImporter : SpreadsheetImporter
    {
        public ManARTSPFTAndHeamatologyImporter(FileStream stream, IFormFile file,
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
            return new Hashtable(){
                  { "Height", "PatientMeasurement.Height"},
                  { "FEV1", "PatientPulmonaryFunctionTest.PulmonaryFunctionTestId"},
                  { "FVC", "PatientPulmonaryFunctionTest.PulmonaryFunctionTestId"},
                  { "VA", "PatientPulmonaryFunctionTest.PulmonaryFunctionTestId"},
                  { "KCO", "PatientPulmonaryFunctionTest.PulmonaryFunctionTestId"},
                  { "DCLO", "PatientPulmonaryFunctionTest.PulmonaryFunctionTestId"},
                  { "SVC", "PatientPulmonaryFunctionTest.PulmonaryFunctionTestId"},
                  { "TLC", "PatientPulmonaryFunctionTest.PulmonaryFunctionTestId"},
                  { "HaematologyDate", "PatientHaematology.DateTaken"},
                  { "Hb", "PatientHaematology.Hb"},
                  { "WBC", "PatientHaematology.WBC"},
                  { "Albumin", "PatientHaematology.Albumin"},
            };
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
                if (patient.ID <= 0) return;
                var patientFromExcel = ReadCellsForPatient(patient, row, cellCount);
            };
            InitializeSheetProcessingForRows(HeadersDictionary(), currentSheet, sheetProcessingAction);
        }

        private object ReadCellsForPatient(Patient patient, IRow row, int cellCount)
        {
            var pft = new PatientPulmonaryFunctionTest();
            var hmt = new PatientHaematology();
            for (int cellCursor = 0; cellCursor < cellCount; cellCursor++)
            {
                if (row.GetCell(cellCursor, MissingCellPolicy.CREATE_NULL_AS_BLANK) != null)
                {
                    ReadCell(patient, pft, hmt, row, cellCursor);
                }
            }
            Imported.Add(patient);
            return patient;
        }

        private void ReadCell(Patient patient, PatientPulmonaryFunctionTest pft, 
                              PatientHaematology hmt, IRow row, int cellCursor)
        {
            string header = _headers.ElementAt(cellCursor);
            string newObjectFields = (string)_dictonary[header];
            if (string.IsNullOrEmpty(newObjectFields)) return;
            string propertyValue = row.GetCell(cellCursor, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
            if (!string.IsNullOrEmpty(propertyValue) && newObjectFields != null)
            {
                var klassAndField = newObjectFields.Split(".");
                string propertyName = klassAndField[1];
                switch (klassAndField[0])
                {
                    case "PatientPulmonaryFunctionTest":
                        var test = _context.PulmonaryFunctionTests.Where(p => p.ShortName.Equals(header))
                                                             .FirstOrDefault();
                        pft = new PatientPulmonaryFunctionTest();
                        string property = "ResultValue";
                        var value = row.GetCell(cellCursor).NumericCellValue;
                        pft.GetType().GetProperty(property).SetValue(pft, (decimal)value);
                        pft.PulmonaryFunctionTestId = test.ID;
                        pft.PatientId = patient.ID;

                        property = "PredictedValue";
                        var predHeaderIndex = _headers.FindIndex(h => h.Contains(header+"Predicted"));
                        var predHeaderValue = row.GetCell(predHeaderIndex)?.NumericCellValue;
                        if (predHeaderValue != null)
                        {
                            pft.GetType().GetProperty(property).SetValue(pft, (decimal)predHeaderValue);
                        }
                      
         
                            var dateHeaderIndex = _headers.FindIndex(h => h.Contains("DateOfTest"));
                            var dateCellValue = row.GetCell(dateHeaderIndex).DateCellValue;
                            pft.GetType().GetProperty("DateTaken").SetValue(pft, dateCellValue);
                            if (patient.PatientPulmonaryFunctionTests == null) patient.PatientPulmonaryFunctionTests = new List<PatientPulmonaryFunctionTest>();
                            patient.PatientPulmonaryFunctionTests.Add(pft);
                        break;
                    case "PatientMeasurement":
                        var meas = new PatientMeasurement();
                        var dateIndex = _headers.FindIndex(h => h.Contains("DateOfTest"));
                        var dateValue = row.GetCell(dateIndex).DateCellValue;
                        meas.DateTaken = dateValue;

                        var heightIdx = _headers.FindIndex(h => h.Contains("Height"));
                        var heightValue = row.GetCell(heightIdx)?.NumericCellValue;

                        var weightIdx = _headers.FindIndex(h => h.Contains("Weight"));
                        var weightValue = row.GetCell(weightIdx)?.NumericCellValue;

                        if (heightValue != null) meas.Height = Convert.ToDecimal(heightValue);
                        if (weightValue != null) meas.Weight = Convert.ToDecimal(weightValue);

                        if (patient.PatientMeasurements == null) patient.PatientMeasurements = new List<PatientMeasurement>();
                        patient.PatientMeasurements.Add(meas);
                        break;
                    case "PatientHaematology":
                        if (header.Equals("HaematologyDate"))
                        {
                            propertyName = "DateTaken";
                            var hemValue = row.GetCell(cellCursor).DateCellValue;
                            hmt.GetType().GetProperty(propertyName).SetValue(hmt, hemValue);
                            hmt.PatientId = patient.ID;
                            if (patient.PatientHaematologies == null) patient.PatientHaematologies = new List<PatientHaematology>();
                            patient.PatientHaematologies.Add(hmt);

                            var hbInd = _headers.FindIndex(h => h.Contains("Hb"));

                            var hbValue = row.GetCell(hbInd)?.NumericCellValue;
                            if (hbValue != null)
                            {
                                hmt.Hb = hbValue.Value;                                
                            }

                            var wbcInd = _headers.FindIndex(h => h.Contains("WBC"));
                            if (wbcInd != -1)
                            {
                                var wbcValue = row.GetCell(wbcInd)?.NumericCellValue;
                                if (wbcValue != null) hmt.WBC = wbcValue.Value;
                            }
                       

                            var albInd = _headers.FindIndex(h => h.Contains("Albumin"));
                            var albVaue = row.GetCell(albInd)?.NumericCellValue;


                            if (albVaue != null) hmt.Albumin = albVaue.Value;
                            
                        }
                        break;
                }

            }
        } 
    }
}
