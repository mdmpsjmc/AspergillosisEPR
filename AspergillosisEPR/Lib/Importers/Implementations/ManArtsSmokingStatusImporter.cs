using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using System.Collections;
using AspergillosisEPR.Models;
using AspergillosisEPR.Lib.Importers.ManARTS;
using AspergillosisEPR.Models.Patients;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class ManArtsSmokingStatusImporter : SpreadsheetImporter
    {    

        public ManArtsSmokingStatusImporter(FileStream stream, IFormFile file,
                                            string fileExtension, AspergillosisContext context) : base(stream, file, fileExtension, context)
        {
            _context = context;
        }

        public static Hashtable HeadersDictionary()
        {
            return SmokingStatusHeaders.Dictionary();
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
            if (string.IsNullOrEmpty(propertyValue)) return;
            var capturedKeys = new List<string>();
            foreach(var key in HeadersDictionary().Keys)
            {
                capturedKeys.Add(key as String);
            }
            if (header == "Smoker")
            {
                var smokingStatusResolver = new SmokingStatusResolver(_context, propertyValue, row, _headers);
                var smokingStatus = smokingStatusResolver.Resolve();
                if (smokingStatus != null)
                {
                    smokingStatus.PatientId = patient.ID;
                    // TODO ADD TO _context
                    Imported.Add(smokingStatus);
                    _context.SaveChanges();
                }
            }                
        }
    }
}
