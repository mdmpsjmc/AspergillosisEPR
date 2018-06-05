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
            GetDbDiagnosesTypesFromSpreadsheet();
            NewDiagnosesFromSpreadsheet();
            Action<Patient, IRow, int> sheetProcessingAction = (patient, row, cellCount) =>
            {
                if (patient.ID > 0)
                {
                    _context.Entry(patient).Collection(p => p.PatientDiagnoses).Load();
                    patient.PatientDiagnoses.ToList().ForEach(pd =>
                    {
                        _context.Entry(pd).Reference(d => d.DiagnosisType).Load();
                    });
                }             
                _patientAliveStatus = _context.PatientStatuses.Where(s => s.Name == "Active").FirstOrDefault().ID;
                _patientDeceasedStatus = _context.PatientStatuses.Where(s => s.Name == "Deceased").FirstOrDefault().ID;
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
            if (!string.IsNullOrEmpty(propertyValue) && newObjectFields != null)
            {
                string diagnosisName = header.Replace("Has", String.Empty);
                if (GetSpreadsheetUnderlyingDiagnoses().Contains(diagnosisName))
                {
                    var diagnosisFromExcel = BuildManARTSDiagnosis(diagnosisName, row);
                    var manartsDiagnosisResolver = new ManARTSPatientDiagnosisResolver(patient, diagnosisName, _context, diagnosisFromExcel);
                    manartsDiagnosisResolver.Resolve();
                }
            }
        }

        private ManARTSDiagnosis BuildManARTSDiagnosis(string diagnosisName, IRow row)
        {
            var notesHeader = _headers.Where(h => h == diagnosisName + "Notes")
                                      .FirstOrDefault();

            var yearHeader = _headers.Where(h => h == diagnosisName + "Year")
                                     .FirstOrDefault();

            var primarySecondaryHeader = _headers.Where(h => h == diagnosisName + "PrimarySecondary")
                                                 .FirstOrDefault();

            int notesHeaderIndex = _headers.IndexOf(notesHeader);
            int yearsHeaderIndex = _headers.IndexOf(yearHeader);
            int primarySecondaryHeaderIndex = _headers.IndexOf(primarySecondaryHeader);
            int cpaTypeHeaderIndex = _headers.IndexOf("CPAType");

            var manartsDiagnosis = new ManARTSDiagnosis();
            manartsDiagnosis.Name = diagnosisName;
            manartsDiagnosis.Notes = (notesHeaderIndex != -1) ? row.GetCell(notesHeaderIndex, MissingCellPolicy.RETURN_BLANK_AS_NULL)?.StringCellValue : null;
            manartsDiagnosis.Year = (notesHeaderIndex != -1) ? row.GetCell(yearsHeaderIndex, MissingCellPolicy.RETURN_BLANK_AS_NULL)?.NumericCellValue.ToString() : null;
            manartsDiagnosis.PrimarySecondary = (primarySecondaryHeaderIndex != -1) ? row.GetCell(primarySecondaryHeaderIndex, MissingCellPolicy.RETURN_BLANK_AS_NULL)?.StringCellValue : null;            
            if (diagnosisName == "CPA")
            {
                string cpaType = row.GetCell(cpaTypeHeaderIndex)?.StringCellValue;
                if (!string.IsNullOrEmpty(manartsDiagnosis.Notes))
                {
                    manartsDiagnosis.Notes = manartsDiagnosis.Notes + " " + cpaType;
                } else
                {
                    manartsDiagnosis.Notes = cpaType;
                }               
            }
            return manartsDiagnosis;
        }

        private List<string> NewDiagnosesFromSpreadsheet()
        {
            var dbDiagnosesNames = _dbDiagnoses
                                            .Select(dx => {
                                                if (string.IsNullOrEmpty(dx.ShortName)) return dx.Name;
                                                else return dx.ShortName;
                                            })
                                            .ToList();

            return _newDiagnoses = GetSpreadsheetUnderlyingDiagnoses()
                                                            .Except(dbDiagnosesNames)
                                                            .ToList();
        }

        private List<string> GetSpreadsheetUnderlyingDiagnoses()
        {
            return _headers.Where(h => h.Contains("Has"))
                           .Select(h => h.Replace("Has", String.Empty)
                                         .SplitCamelCase())
                           .ToList();
        }

        private void GetDbDiagnosesTypesFromSpreadsheet()
        {
            _dbDiagnoses = _context.DiagnosisTypes
                                    .Where(dt => GetSpreadsheetUnderlyingDiagnoses().Contains(dt.Name)
                                                || GetSpreadsheetUnderlyingDiagnoses().Contains(dt.ShortName))
                                    .ToList();
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
