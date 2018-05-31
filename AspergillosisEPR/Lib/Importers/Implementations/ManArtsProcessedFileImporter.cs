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
            return new Hashtable()
            {
                  { "Forename", "Patient.FirstName" },
                  { "Surename", "Patient.LastName"},
                  { "DOB", "Patient.DOB" },
                  { "RM2", "Patient.RM2Number" },
                  { "DEATH_TIME", "Patient.DateOfDeath" },
                  { "DeathIndicator", "Patient.PatientStatusId"},
                  { "Gender", "Patient.Gender" },
                  { "NhsNumber", "Patient.NhsNumber"},
                  { "HasCPA", "PatientDiagnosis.DiagnosisTypeId" },
                  { "CPAType", "PatientDiagnosis.DiagnosisTypeId"},
                  { "CPANotes", "Patient.DiagnosisTypeId"},
                  { "CPAYear", "PatientDiagnosis.DiagnosisTypeId"},
                  { "CCPAPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasAspergilloma", "PatientDiagnosis.DiagnosisTypeId"},
                  { "AspergillomaYear", "PatientDiagnosis.DiagnosisTypeId"},
                  { "AspergillomaPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasABPA", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasAsthma", "PatientDiagnosis.DiagnosisTypeId"},
                  { "AsthmaNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "AsthmaYear", "PatientDiagnosis.DiagnosisTypeId"},
                  { "AsthmaPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasEmphysema", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasCOPD", "PatientDiagnosis.DiagnosisTypeId"},
                  { "COPDNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "COPDYear",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "COPDPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasPneumonia", "PatientDiagnosis.DiagnosisTypeId" },
                  { "HasFungalLungDisease", "PatientDiagnosis.DiagnosisTypeId" },
                  { "FungalLungDiseaseNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "FungalLungDiseaseYear",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "FungalLungDiseaseYearPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasChronicCough", "PatientDiagnosis.DiagnosisTypeId" },
                  { "ChronicCoughNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "ChronicCoughYear",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "ChronicCoughPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasTB", "PatientDiagnosis.DiagnosisTypeId" },
                  { "IsTBPrevious", "PatientDiagnosis.DiagnosisTypeId" },
                  { "PreviousTBNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "PreviousTBYear",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "PreviousTBYearPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasBronchiectasis", "PatientDiagnosis.DiagnosisTypeId" },
                  { "BronchiectasisNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "BronchiectasisYear",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "BronchiectasisPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasSarcoidosis", "PatientDiagnosis.DiagnosisTypeId" },
                  { "SarcoidosisNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "SarcoidosisYear",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "SarcoidosisPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasHypertension", "PatientDiagnosis.DiagnosisTypeId" },
                  { "HypertensionPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasCancer", "PatientDiagnosis.DiagnosisTypeId" },
                  { "CancerNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "CancerYear",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "CancerPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},               
                  { "HasLungSurgery", "PatientDiagnosis.DiagnosisTypeId" },
                  { "LungSurgeryNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "LungSurgeryCancerYear",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "LungSurgeryPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasDiabetes", "PatientDiagnosis.DiagnosisTypeId" },
                  { "DiabetesNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "DiabetesYear",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "DiabetesPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasHeartDisease", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasOsteoporosis",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "OtherDiagnosisAndNotes", "PatientDiagnosis.DiagnosisTypeId"},
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
            GetDbDiagnosesTypesFromSpreadsheet();
            NewDiagnosesFromSpreadsheet();
            Action<Patient, IRow, int> sheetProcessingAction = (patient, row, cellCount) =>
            {
                _context.Entry(patient).Collection(p => p.PatientDiagnoses).Load();
                patient.PatientDiagnoses.ToList().ForEach(pd =>
                {
                    _context.Entry(pd).Reference(d => d.DiagnosisType).Load();
                });
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

            var manartsDiagnosis = new ManARTSDiagnosis()
            {
                Name = diagnosisName,
                Notes = row.GetCell(notesHeaderIndex-1, MissingCellPolicy.RETURN_BLANK_AS_NULL)?.StringCellValue,
                Year = row.GetCell(yearsHeaderIndex-1, MissingCellPolicy.RETURN_BLANK_AS_NULL)?.StringCellValue,
                PrimarySecondary = row.GetCell(primarySecondaryHeaderIndex-1, MissingCellPolicy.RETURN_BLANK_AS_NULL)?.StringCellValue
            };
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
