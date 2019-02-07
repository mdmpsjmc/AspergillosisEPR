using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class ReferralDatesSpreadsheetImporter : SpreadsheetImporter
    {

        public ReferralDatesSpreadsheetImporter(FileStream stream, IFormFile file,
                              string fileExtension, AspergillosisContext context) : base(stream, file, fileExtension, context)

        {

        }

        public static Hashtable HeadersDictionary()
        {
            return new Hashtable()
            {
                  { "RM2", "Patient.RM2Number" },
                  { "REF", "PatientNACDates.ReferralDate" },
                  { "BAND", "PatientNACDates.CPABand" },
                  { "DRUG1", "PatientNACDates.InitialDrug"},
                  { "DRUG3", "PatientNACDates.FollowUp3MonthsDrug"}             
             };
        }

        protected override List<string> IdentiferHeaders()
        {
            return new List<string> { "RM2" };
        }

        protected override void ProcessSheet(ISheet currentSheet)
        {

            Action<Patient, IRow, int> sheetProcessingAction = (patient, row, cellCount) =>
            {                
                InitializeNACDatesNavigationProperty(patient);
                ReadCellsForPatient(patient, row, cellCount);
            };             
            InitializeSheetProcessingForRows(HeadersDictionary(), currentSheet, sheetProcessingAction);
        }

        private void ReadCellsForPatient(Patient patient, IRow row, int cellCount)
        {
            for (int cellCursor = 0; cellCursor < cellCount; cellCursor++)
            {
                if (patient.ID <= 0) continue;
                if (!patient.PatientNACDates.Any()) patient.PatientNACDates = new List<PatientNACDates>() { new PatientNACDates() };
                if (row.GetCell(cellCursor, MissingCellPolicy.CREATE_NULL_AS_BLANK) != null)
                {
                    ReadCell(patient, row, cellCursor);
                }
            }
            if (patient.ID > 0)
            {
                Imported.Add(patient);                
            }
        }

        private void InitializeNACDatesNavigationProperty(Patient patient)
        {
            var nacInfo = _context.PatientNACDates.FirstOrDefault();
            if (nacInfo == null)
            {
                nacInfo = new PatientNACDates();
                nacInfo.PatientId = patient.ID;
                _context.PatientNACDates.Add(nacInfo);
            }
            _context.Update(nacInfo);
        }

        private void ReadCell(Patient patient, IRow row, int cellCursor)
        {
            string header = _headers.ElementAt(cellCursor);
            string newObjectFields = (string)_dictonary[header];
            string propertyValue = row.GetCell(cellCursor, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
            if (!string.IsNullOrEmpty(propertyValue) && newObjectFields != null)
            {
                var klassAndField = newObjectFields.Split(".");
                switch (klassAndField[0])
                {                    
                    case "PatientNACDates":
                        var nacInfo = patient.PatientNACDates.First();
                        string propertyName = klassAndField[1];
                        switch (propertyName)
                        {
                            case "ReferralDate":
                                DateTime referralDate = Convert.ToDateTime(propertyValue);
                                nacInfo.ReferralDate = referralDate;
                            break;
                            case "InitialDrug":
                                nacInfo.InitialDrug = propertyValue;
                            break;
                            case "FollowUp3MonthsDrug":
                                nacInfo.FollowUp3MonthsDrug = propertyValue;
                            break;
                            case "CPABand":
                                string band = Regex.Match(propertyValue, @"\d+").Value;
                                if (!band.Equals(String.Empty)) nacInfo.CPABand = Int32.Parse(band);
                            break;
                        }
                        break;
                }
            }
        }
    }
}
