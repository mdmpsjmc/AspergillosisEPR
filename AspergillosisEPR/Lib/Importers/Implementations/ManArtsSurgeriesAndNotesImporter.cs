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
using AspergillosisEPR.Models.Patients;
using System.Text.RegularExpressions;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class ManArtsSurgeriesAndNotesImporter : SpreadsheetImporter
    {
        public ManArtsSurgeriesAndNotesImporter(FileStream stream, IFormFile file,
                                            string fileExtension, AspergillosisContext context) : base(stream, file, fileExtension, context)
        {
            _context = context;
        }

        public static Hashtable HeadersDictionary()
        {
            return SurgeryHeaders.Dictionary();
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
                Console.Write("DS");
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
            switch (header)
            {
                case "OtherDiagnosisAndNotes":
                    SetPatientGenericNote(patient, propertyValue);
                    break;
                case "HasLungSurgery":
                    var surgeryResolver = new ManARTSPatientSurgeryResolver(_context, propertyValue, row, _headers);
                    PatientSurgery surgery = surgeryResolver.Resolve();
                    if (surgery != null)
                    {
                        surgery.PatientId = patient.ID;
                        _context.PatientSurgeries.Add(surgery);
                        Imported.Add(surgery);
                        _context.SaveChanges();
                    }
                break;
            }
        }

        private void SetPatientGenericNote(Patient patient, string note)
        {
            if (!string.IsNullOrEmpty(patient.GenericNote))
            {
                patient.GenericNote = patient.GenericNote + ", " + note;
            } else
            {
                patient.GenericNote = note;
            }
        }
    }
}
