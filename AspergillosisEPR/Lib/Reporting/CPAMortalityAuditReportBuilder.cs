using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Reporting
{
    public class CPAMortalityAuditReportBuilder
    {
        private AspergillosisContext _context;
        private XSSFWorkbook _outputWorkbook;
        private XSSFWorkbook _inputWorkbook;
        private FileStream _stream;
        private IFormFile _file;
        private ILogger _logger;
        
        private List<string> _outputSheetNames = new List<string>()
        {
            "Demographics and dates",
            "Diagnoses",
            "SGRQ",
            "MRC",
            "Weight",
            "PFT",
            "Aspergillus F IgG",
            "Total IgE",
        };

        public CPAMortalityAuditReportBuilder(AspergillosisContext context, 
                                              FileStream inputFileStream,
                                              ILogger logger, 
                                              IFormFile formFile)
        {
            _context = context;            
            _outputWorkbook = new XSSFWorkbook();
            _stream = inputFileStream;
            _file = formFile;
            _logger = logger;
        }     
       
        public CPAMortalityAuditReportBuilder(AspergillosisContext context,
                                             ILogger logger,
                                             FileStream inputFileStream)
        {
            _context = context;
            _outputWorkbook = new XSSFWorkbook();
            _logger = logger;
            _stream = inputFileStream;
        }

        public byte[] Build()
        {
            var ids = GetPatientIdentifiers();
            var patients = _context.Patients
                                   .Where(p => ids.Contains(p.RM2Number))
                                   .Include(p => p.PatientDiagnoses)
                                   .ThenInclude(p => p.DiagnosisType)
                                   .Include(p => p.STGQuestionnaires)
                                   .Include(p => p.PatientMRCScores)
                                   .Include(p => p.PatientMeasurements)
                                   .Include(p => p.PatientPulmonaryFunctionTests)
                                   .ThenInclude(p => p.PulmonaryFunctionTest)
                                   .Include(p => p.PatientImmunoglobulines)
                                    .ThenInclude(p => p.ImmunoglobulinType)
                                   .Include(p => p.PatientTestResults)
                                    .ThenInclude(p => p.TestType)
                                   .OrderBy(p => p.LastName)
                                   .ToList();
            BuildDemographicsTab(patients);
            BuildDiagnosesTab(patients);
            BuildSGRQTab(patients);
            BuildMRCTab(patients);
            BuildWeightTab(patients);
            BuildPFTTab(patients);
            BuildIgGTab(patients);
            BuildTotalIgETab(patients);
            BuildTestResultTab(patients, "C-Reactive Protein (CRP)");
            BuildTestResultTab(patients, "Haemoglobin");
            BuildTestResultTab(patients, "WBC");
            BuildTestResultTab(patients, "Lymphocytes");
            return SerializeWorkbook();
        }

        private void BuildTestResultHeaders(ISheet currentSheet, int itemCount)
        {
            var headersRow = currentSheet.CreateRow(0);
            var headers = new List<string>()
            {
                "RM2",
                "Forename",
                "Surname",
            };

            var repeatItems = new List<string>()
            {
                "DateTaken",  "Result"
            };
            for (int cursor = 1; cursor < itemCount + 1; cursor++)
            {
                headers.Add(repeatItems[0] + cursor);
                headers.Add(repeatItems[1] + cursor);
            }

            MakeHeadersBold(headersRow, headers);
        }

      
        private void BuildTestResultTab(List<Patient> patients, string testName)
        {
            ISheet currentSheet = _outputWorkbook.CreateSheet(testName);
            int itemCount = patients.Select(p => p.PatientTestResults
                                                   .Where(ig => ig.TestType.Name.Equals(testName)).Count())
                                                   .OrderByDescending(d => d)
                                                   .First();
            BuildTestResultHeaders(currentSheet, itemCount);
            for (int patientCursor = 0; patientCursor < patients.Count(); patientCursor++)
            {
                int cellIndex = 0;
                var nextToHeaderRow = patientCursor + 1;
                var currentPatient = patients.ToList()[patientCursor];
                var currentRow = currentSheet.CreateRow(nextToHeaderRow);
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.RM2Number);
                cellIndex++;
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.FirstName);
                cellIndex++;
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.LastName);
                cellIndex++;
                var items = currentPatient.PatientTestResults
                                          .Where(ig => ig.TestType.Name.Equals(testName))
                                          .OrderByDescending(q => q.DateTaken)
                                          .ToList();
                if (items.Count == 0) continue;
                for (int index = 0; index < items.Count(); index++)
                {
                    var currentItem = items[index];
                    if (currentItem.DateTaken != null)
                    {
                        var dateTaken = currentItem.DateTaken.ToString("dd/MM/yyyy");
                        currentRow.CreateCell(cellIndex).SetCellValue(dateTaken);
                        cellIndex++;
                    }
                    else
                    {
                        currentRow.CreateCell(cellIndex).SetCellValue("");
                        cellIndex++;
                    }

                    currentRow.CreateCell(cellIndex).SetCellValue(currentItem.Value.ToString());
                    cellIndex++;
                }
            }
        }       

        private void BuildTotalIgETab(List<Patient> patients)
        {
            ISheet currentSheet = _outputWorkbook.CreateSheet(_outputSheetNames[7]);
            int itemCount = patients.Select(p => p.PatientImmunoglobulines
                                                   .Where(ig => ig.ImmunoglobulinType.Name.Equals("Total IgE")).Count())
                                                   .OrderByDescending(d => d)
                                                   .First();
            BuildTotalIgEHeaders(currentSheet, itemCount);
            for (int patientCursor = 0; patientCursor < patients.Count(); patientCursor++)
            {
                int cellIndex = 0;
                var nextToHeaderRow = patientCursor + 1;
                var currentPatient = patients.ToList()[patientCursor];
                var currentRow = currentSheet.CreateRow(nextToHeaderRow);
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.RM2Number);
                cellIndex++;
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.FirstName);
                cellIndex++;
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.LastName);
                cellIndex++;
                var items = currentPatient.PatientImmunoglobulines
                                          .Where(ig => ig.ImmunoglobulinType.Name.Equals("Total IgE"))
                                          .OrderByDescending(q => q.DateTaken)
                                          .ToList();
                if (items.Count == 0) continue;
                for (int index = 0; index < items.Count(); index++)
                {
                    var currentItem = items[index];
                    if (currentItem.DateTaken != null)
                    {
                        var dateTaken = currentItem.DateTaken.ToString("dd/MM/yyyy");
                        currentRow.CreateCell(cellIndex).SetCellValue(dateTaken);
                        cellIndex++;
                    }
                    else
                    {
                        currentRow.CreateCell(cellIndex).SetCellValue("");
                        cellIndex++;
                    }

                    currentRow.CreateCell(cellIndex).SetCellValue(currentItem.Value.ToString());
                    cellIndex++;
                    currentRow.CreateCell(cellIndex).SetCellValue(currentItem.Range.ToString());
                    cellIndex++;
                }
            }
        }

        private void BuildTotalIgEHeaders(ISheet currentSheet, int itemCount)
        {
            var headersRow = currentSheet.CreateRow(0);
            var headers = new List<string>()
            {
                "RM2",
                "Forename",
                "Surname",
            };

            var repeatItems = new List<string>()
            {
                "DateTaken",  "Result", "Range"
            };
            for (int cursor = 1; cursor < itemCount + 1; cursor++)
            {
                headers.Add(repeatItems[0] + cursor);
                headers.Add(repeatItems[1] + cursor);
                headers.Add(repeatItems[2] + cursor);
            }

            MakeHeadersBold(headersRow, headers);
        }

        private void BuildIgGTab(List<Patient> patients)
        {
            ISheet currentSheet = _outputWorkbook.CreateSheet(_outputSheetNames[6]);
            int itemCount = patients.Select(p => p.PatientImmunoglobulines
                                                   .Where(ig => ig.ImmunoglobulinType.Name.Equals("Aspergillus F IgG")).Count())
                                                   .OrderByDescending(d => d)
                                                   .First();
            BuildIgGHeaders(currentSheet, itemCount);
            for (int patientCursor = 0; patientCursor < patients.Count(); patientCursor++)
            {
                int cellIndex = 0;
                var nextToHeaderRow = patientCursor + 1;
                var currentPatient = patients.ToList()[patientCursor];
                var currentRow = currentSheet.CreateRow(nextToHeaderRow);
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.RM2Number);
                cellIndex++;
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.FirstName);
                cellIndex++;
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.LastName);
                cellIndex++;
                var items = currentPatient.PatientImmunoglobulines
                                          .Where(ig => ig.ImmunoglobulinType.Name.Equals("Aspergillus F IgG"))
                                          .OrderByDescending(q => q.DateTaken)
                                          .ToList();
                if (items.Count == 0) continue;
                for (int index = 0; index < items.Count(); index++)
                {
                    var currentItem = items[index];
                    if (currentItem.DateTaken != null)
                    {
                        var dateTaken = currentItem.DateTaken.ToString("dd/MM/yyyy");
                        currentRow.CreateCell(cellIndex).SetCellValue(dateTaken);
                        cellIndex++;
                    }
                    else
                    {
                        currentRow.CreateCell(cellIndex).SetCellValue("");
                        cellIndex++;
                    }

                    currentRow.CreateCell(cellIndex).SetCellValue(currentItem.Value.ToString());
                    cellIndex++;                  
                }
            }
        }

        private void BuildIgGHeaders(ISheet currentSheet, int itemCount)
        {
            var headersRow = currentSheet.CreateRow(0);
            var headers = new List<string>()
            {
                "RM2",
                "Forename",
                "Surname",
            };

            var repeatItems = new List<string>()
            {
                "DateTaken",  "Result"
            };
            for (int cursor = 1; cursor < itemCount + 1; cursor++)
            {
                headers.Add(repeatItems[0] + cursor);
                headers.Add(repeatItems[1] + cursor);
            }

            MakeHeadersBold(headersRow, headers);
        }

        private void BuildPFTTab(List<Patient> patients)
        {
            ISheet currentSheet = _outputWorkbook.CreateSheet(_outputSheetNames[5]);
            int itemCount = patients.Select(p => p.PatientPulmonaryFunctionTests.Count).OrderByDescending(d => d).First();
            BuildPFTHeaders(currentSheet, itemCount);
            for (int patientCursor = 0; patientCursor < patients.Count(); patientCursor++)
            {
                int cellIndex = 0;
                var nextToHeaderRow = patientCursor + 1;
                var currentPatient = patients.ToList()[patientCursor];
                var currentRow = currentSheet.CreateRow(nextToHeaderRow);
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.RM2Number);
                cellIndex++;
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.FirstName);
                cellIndex++;
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.LastName);
                cellIndex++;
                var items = currentPatient.PatientPulmonaryFunctionTests.OrderByDescending(q => q.DateTaken).ToList();
                if (items.Count == 0) continue;
                for (int index = 0; index < items.Count(); index++)
                {
                    var currentItem = items[index];
                    if (currentItem.DateTaken != null)
                    {
                        var dateTaken = currentItem.DateTaken.Value.ToString("dd/MM/yyyy");
                        currentRow.CreateCell(cellIndex).SetCellValue(dateTaken);
                        cellIndex++;
                    } else
                    {
                        currentRow.CreateCell(cellIndex).SetCellValue("");
                        cellIndex++;
                    }
                    
                    currentRow.CreateCell(cellIndex).SetCellValue(currentItem.PulmonaryFunctionTest.ShortName);
                    cellIndex++;
                    currentRow.CreateCell(cellIndex).SetCellValue(Math.Round(currentItem.ResultValue, 2).ToString());
                    cellIndex++;
                    currentRow.CreateCell(cellIndex).SetCellValue(Math.Round(currentItem.PredictedValue, 0).ToString());
                    cellIndex++;
                }
            }
        }

        private void BuildPFTHeaders(ISheet currentSheet, int itemCount)
        {
            var headersRow = currentSheet.CreateRow(0);
            var headers = new List<string>()
            {
                "RM2",
                "Forename",
                "Surname",
            };

            var repeatItems = new List<string>()
            {
                "DateTaken", "Test", "Result", "Predicted"
            };
            for (int cursor = 1; cursor < itemCount + 1; cursor++)
            {
                headers.Add(repeatItems[0] + cursor);
                headers.Add(repeatItems[1] + cursor);
                headers.Add(repeatItems[2] + cursor);
                headers.Add(repeatItems[3] + cursor);
            }

            MakeHeadersBold(headersRow, headers);
        }

        private void BuildWeightTab(List<Patient> patients)
        {
            ISheet currentSheet = _outputWorkbook.CreateSheet(_outputSheetNames[4]);
            int itemCount = patients.Select(p => p.PatientMeasurements.Count).OrderByDescending(d => d).First();
            BuildWeightHeaders(currentSheet, itemCount);
            for (int patientCursor = 0; patientCursor < patients.Count(); patientCursor++)
            {
                int cellIndex = 0;
                var nextToHeaderRow = patientCursor + 1;
                var currentPatient = patients.ToList()[patientCursor];
                var currentRow = currentSheet.CreateRow(nextToHeaderRow);
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.RM2Number);
                cellIndex++;
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.FirstName);
                cellIndex++;
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.LastName);
                cellIndex++;
                var items = currentPatient.PatientMeasurements.OrderByDescending(q => q.DateTaken).ToList();
                if (items.Count == 0) continue;
                for (int index = 0; index < items.Count(); index++)
                {
                    var currentItem = items[index];
                    currentRow.CreateCell(cellIndex).SetCellValue(currentItem.DateTaken.ToString("dd/MM/yyyy"));
                    cellIndex++;
                    currentRow.CreateCell(cellIndex).SetCellValue(Math.Round(currentItem.Weight.Value, 2).ToString());
                    cellIndex++;
                }
            }
        }

        private void BuildWeightHeaders(ISheet currentSheet, int itemCount)
        {
            var headersRow = currentSheet.CreateRow(0);
            var headers = new List<string>()
            {
                "RM2",
                "Forename",
                "Surname",
            };

            var repeatItems = new List<string>()
            {
                "DateTaken", "Weight"
            };
            for (int cursor = 1; cursor < itemCount + 1; cursor++)
            {
                headers.Add(repeatItems[0] + cursor);
                headers.Add(repeatItems[1] + cursor);
            }

            MakeHeadersBold(headersRow, headers);
        }

        private void BuildMRCTab(List<Patient> patients)
        {
            ISheet currentSheet = _outputWorkbook.CreateSheet(_outputSheetNames[3]);
            int itemCount = patients.Select(p => p.PatientMRCScores.Count).OrderByDescending(d => d).First();
            BuildMRCHeaders(currentSheet, itemCount);
            for (int patientCursor = 0; patientCursor < patients.Count(); patientCursor++)
            {
                int cellIndex = 0;
                var nextToHeaderRow = patientCursor + 1;
                var currentPatient = patients.ToList()[patientCursor];
                var currentRow = currentSheet.CreateRow(nextToHeaderRow);
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.RM2Number);
                cellIndex++;
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.FirstName);
                cellIndex++;
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.LastName);
                cellIndex++;
                var items = currentPatient.PatientMRCScores.OrderByDescending(q => q.DateTaken).ToList();
                if (items.Count == 0) continue;
                for (int index = 0; index < items.Count(); index++)
                {
                    var currentItem = items[index];
                    currentRow.CreateCell(cellIndex).SetCellValue(currentItem.DateTaken.ToString("dd/MM/yyyy"));
                    cellIndex++;
                    currentRow.CreateCell(cellIndex).SetCellValue(currentItem.Score);
                    cellIndex++;
                }
            }
        }

        private void BuildMRCHeaders(ISheet currentSheet, int itemCount)
        {
            var headersRow = currentSheet.CreateRow(0);
            var headers = new List<string>()
            {
                "RM2",
                "Forename",
                "Surname",
            };

            var repeatItems = new List<string>()
            {
                "DateTaken", "Score", 
            };
            for (int cursor = 1; cursor < itemCount + 1; cursor++)
            {
                headers.Add(repeatItems[0] + cursor);
                headers.Add(repeatItems[1] + cursor);
            }

            for (int cursor = 0; cursor < headers.Count; cursor++)
            {
                var header = headers[cursor];
                var headerCell = headersRow.CreateCell(cursor);
                headerCell.SetCellType(CellType.String);
                ApplyBoldCellStyle(headerCell);
                headerCell.SetCellValue(header);
            }
     
        }

        private void BuildSGRQTab(List<Patient> patients)
        {
            ISheet currentSheet = _outputWorkbook.CreateSheet(_outputSheetNames[2]);
            int stgCount = patients.Select(p => p.STGQuestionnaires.Count).OrderByDescending(d => d).First();
            BuildSGRQHeaders(currentSheet, stgCount);
           
            for (int patientCursor = 0; patientCursor < patients.Count(); patientCursor++)
            {
                int cellIndex = 0;
                var nextToHeaderRow = patientCursor + 1;
                var currentPatient = patients.ToList()[patientCursor];
                var currentRow = currentSheet.CreateRow(nextToHeaderRow);
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.RM2Number);
                cellIndex++;
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.FirstName);
                cellIndex++;
                currentRow.CreateCell(cellIndex).SetCellValue(currentPatient.LastName);
                cellIndex++;
                var allSGRQ = currentPatient.STGQuestionnaires.OrderByDescending(q => q.DateTaken).ToList();
                if (allSGRQ.Count == 0) continue;
                for (int index = 0; index < allSGRQ.Count(); index++)
                {
                    var currentSGRQ = allSGRQ[index];
                    currentRow.CreateCell(cellIndex).SetCellValue(currentSGRQ.DateTaken.ToString("dd/MM/yyyy"));
                    cellIndex++;
                    currentRow.CreateCell(cellIndex).SetCellValue(Math.Round(currentSGRQ.SymptomScore, 2).ToString());
                    cellIndex++;
                    currentRow.CreateCell(cellIndex).SetCellValue(Math.Round(currentSGRQ.ActivityScore, 2).ToString());
                    cellIndex++;
                    currentRow.CreateCell(cellIndex).SetCellValue(Math.Round(currentSGRQ.ImpactScore, 2).ToString());
                    cellIndex++;
                    currentRow.CreateCell(cellIndex).SetCellValue(Math.Round(currentSGRQ.TotalScore, 2).ToString());
                    cellIndex++;
                }
            }
        }

        private void BuildSGRQHeaders(ISheet currentSheet, int highestSTGCount)
        {
            var headersRow = currentSheet.CreateRow(0);
            var headers = new List<string>()
            {
                "RM2",
                "Forename",
                "Surname",
            };

            var repeatItems = new List<string>()
            {
                "DateTaken", "SymptomScore", "ActivityScore", "ÏmpactScore", "TotalScore"
            };
            for(int cursor=1; cursor < highestSTGCount+1; cursor++)
            {
                headers.Add(repeatItems[0] + cursor);
                headers.Add(repeatItems[1] + cursor);
                headers.Add(repeatItems[2] + cursor);
                headers.Add(repeatItems[3] + cursor);
                headers.Add(repeatItems[4] + cursor);
            }

            MakeHeadersBold(headersRow, headers);
        }

        private void BuildDiagnosesTab(List<Patient> patients)
        {
            ISheet currentSheet = _outputWorkbook.CreateSheet(_outputSheetNames[1]);
            BuildDiagnosisHeaders(currentSheet);
            for (int patientCursor = 0; patientCursor < patients.Count(); patientCursor++)
            {
                var nextToHeaderRow = patientCursor + 1;
                var currentPatient = patients.ToList()[patientCursor];
                var currentRow = currentSheet.CreateRow(nextToHeaderRow);
                currentRow.CreateCell(0).SetCellValue(currentPatient.RM2Number);
                currentRow.CreateCell(1).SetCellValue(currentPatient.FirstName);
                currentRow.CreateCell(2).SetCellValue(currentPatient.LastName);
                var firstDiagnosis = currentPatient.PatientDiagnoses.FirstOrDefault();
                var diagnosesDescriptions = currentPatient.PatientDiagnoses.Select(pd => pd.Description).ToList();
                var diagnosesShortNames = currentPatient.PatientDiagnoses.Select(pd => pd.DiagnosisType.ShortName).ToList();
                var diagnosesNames = currentPatient.PatientDiagnoses.Select(pd => pd.DiagnosisType.Name).ToList();

                var isCCPA = diagnosesShortNames.Contains("CCPA") ? "CCPA" : "";
                var isCFPA = diagnosesShortNames.Contains("CFPA") ? "CFPA" : "";
                var isAspergilloma = diagnosesNames.Contains("Aspergilloma") ? "Aspergilloma" : "";
                var isLungCancer = diagnosesNames.Contains("Lung Cancer") ? "Lung Cancer" : "";
                var isCancer = diagnosesNames.Contains("Cancer") ? "Other cancer" : "";
                var isBillateral = diagnosesDescriptions.Contains("bilateral") || diagnosesDescriptions.Contains("Bilateral")  ? "bilateral" : "";
                var isRA = diagnosesShortNames.Contains("RA") ? "RA" : "";
                var isHIV = diagnosesNames.Contains("HIV") || diagnosesShortNames.Contains("HIV") ? "HIV" : "";
                var renalFailure = diagnosesShortNames.Contains("CKD") 
                    || diagnosesShortNames.Contains("PKD") 
                    || diagnosesNames.Contains("Renal cyst")
                    || diagnosesNames.Contains("Renal impairment")
                    || diagnosesNames.Contains("Renal tumour")
                    ? "Renal failure" : "";
                var isDiab = diagnosesNames.Contains("Diabetes") ? "Diabetes" : "";
                var isGPA = diagnosesNames.Contains("Wegener’s granulomatosis") ? "GPA" : "";
                var isCHURG = diagnosesNames.Contains("Churg-Strauss Syndrome") ? "Churg-Strauss Syndrome" : "";
                var isSLE = diagnosesNames.Contains("Systemic Lupus Erythematosus") ? "Systemic Lupus Erythematosus" : "";
                var isPM = diagnosesShortNames.Contains("PM") ? "PM" : "";
                var isMCTD = diagnosesShortNames.Contains("MCTD") ? "MCTD" : "";
                var myco = diagnosesNames.Contains("Mycobacterium") ? "Mycobacterium infection" : "";
                var copd = diagnosesShortNames.Contains("COPD") 
                                || diagnosesNames.Contains("Emphysema") ? "COPD" : "";


                currentRow.CreateCell(3).SetCellValue(isCCPA);
                currentRow.CreateCell(4).SetCellValue(isCFPA);
                currentRow.CreateCell(5).SetCellValue(isAspergilloma);
                currentRow.CreateCell(6).SetCellValue(copd);
                currentRow.CreateCell(7).SetCellValue(isLungCancer);
                currentRow.CreateCell(8).SetCellValue(isCancer);
                currentRow.CreateCell(9).SetCellValue(isBillateral);
                currentRow.CreateCell(10).SetCellValue(isRA);
                currentRow.CreateCell(11).SetCellValue(isHIV);
                currentRow.CreateCell(12).SetCellValue(renalFailure);
                currentRow.CreateCell(13).SetCellValue(isDiab);
                currentRow.CreateCell(14).SetCellValue(isGPA);
                currentRow.CreateCell(15).SetCellValue(isCHURG);
                currentRow.CreateCell(16).SetCellValue(isSLE);
                currentRow.CreateCell(17).SetCellValue(isPM);
                currentRow.CreateCell(18).SetCellValue(isMCTD);
                currentRow.CreateCell(19).SetCellValue(myco);

            }
        }

        private void BuildDiagnosisHeaders(ISheet currentSheet)
        {
            var headersRow = currentSheet.CreateRow(0);
            var headers = new List<string>()
            {
                "RM2",
                "Forename",
                "Surname",
                "HasCCPA",
                "HasCFPA",
                "HasAspergilloma",
                "HasCOPD",
                "HasLungCancer",
                "HasCancer",
                "HasBillateralDisease",
                "HasRheumathoidArthritis",
                "HasHIV",
                "HasRenalFailure",
                "HasDiabetes",
                "HasGPA",
                "HasChurgStrauss",
                "HasSLE",
                "HasPolymyositis",
                "HasMixedConnectiveTissueDisease",
                "HasMycobacteriumInfection",
            };
            MakeHeadersBold(headersRow, headers);
        }

        private void BuildDemographicsTab(List<Patient> patients)
        {
            ISheet currentSheet = _outputWorkbook.CreateSheet(_outputSheetNames[0]);
            BuildDemographicsHeaders(currentSheet);
            for(int patientCursor = 0; patientCursor < patients.Count(); patientCursor++)
            {
                var nextToHeaderRow = patientCursor + 1;
                var currentPatient = patients.ToList()[patientCursor];
                _context.Entry(currentPatient).Collection(p => p.PatientNACDates).Load();
                var date = currentPatient.PatientNACDates.FirstOrDefault();
                var currentRow = currentSheet.CreateRow(nextToHeaderRow);
                currentRow.CreateCell(0).SetCellValue(currentPatient.RM2Number);
                currentRow.CreateCell(1).SetCellValue(currentPatient.FirstName);
                currentRow.CreateCell(2).SetCellValue(currentPatient.LastName);
                currentRow.CreateCell(3).SetCellValue(currentPatient.Age());
                if (date != null) currentRow.CreateCell(4).SetCellValue(date.AgeWhenFirstSeen());
                currentRow.CreateCell(5).SetCellValue(currentPatient.Gender);
                currentRow.CreateCell(6).SetCellValue(currentPatient.PostCode);
                currentRow.CreateCell(7).SetCellValue(GetDistanceFromWythenshawe(currentPatient));
                currentRow.CreateCell(8).SetCellValue(currentPatient.BucketDistance());
                if (date != null)
                {
                    currentRow.CreateCell(9).SetCellValue(date.FirstSeenAtNAC.ToString("dd/MM/yyyy"));
                    if (date.LastObservationPoint != null) currentRow.CreateCell(10).SetCellValue(date.LastObservationPoint.Value.ToString("dd/MM/yyyy"));
                    if (currentPatient.DateOfDeath != null)
                    {
                        currentRow.CreateCell(11).SetCellValue(currentPatient.DateOfDeath.Value.ToString("dd/MM/yyyy"));
                    }
                    if (date.DateOfDiagnosis != null) currentRow.CreateCell(12).SetCellValue(date.DateOfDiagnosis.Value.ToString("dd/MM/yyyy"));
                    if (date.ProbableStartOfDisease != null) currentRow.CreateCell(13).SetCellValue(date.ProbableStartOfDisease.Value.ToString("dd/MM/yyyy"));
                    if (date.DefiniteStartOfDisease != null) currentRow.CreateCell(14).SetCellValue(date.DefiniteStartOfDisease.Value.ToString("dd/MM/yyyy"));

                }
           
            }
        }

        private string GetDistanceFromWythenshawe(Patient currentPatient)
        {
            return Math.Round(currentPatient.DistanceFromWythenshawe, 2).ToString() + "m";
        }

        private void BuildDemographicsHeaders(ISheet currentSheet)
        {            
            var headersRow = currentSheet.CreateRow(0);
            var headers = new List<string>()
            {
                "RM2",
                "Forename",
                "Surname",
                "AgeAtDeath",
                "AgeWhenFirstSeen",
                "Sex",
                "Postcode",
                "DistanceFromWythenshawe",
                "DistanceBucket",
                "FirstSeenAtNAC", 
                "LastObservationPoint",
                "DateOfDeath",
                "DateOfDiagnosis",
                "ProbableStartOfDisease",
                "DefiniteStartOfDisease",
            };
            MakeHeadersBold(headersRow, headers);
        }

        private List<string> GetPatientIdentifiers()
        {
            if (_file != null) _file.CopyTo(_stream);
            _stream.Position = 0;
            _inputWorkbook = new XSSFWorkbook(_stream);
            var identifiers = new List<string>();
            var sheet = _inputWorkbook.GetSheetAt(0);
            for(var cursor = 0; cursor < sheet.PhysicalNumberOfRows; cursor++)
            {
                var currentRow = sheet.GetRow(cursor);
                var rm2Number = currentRow.Cells[0].ToString().Trim();
                identifiers.Add(rm2Number);
            }
            return identifiers;
        }

        private void ApplyBoldCellStyle(ICell cell)
        {
            ICellStyle boldFontCellStyle = _outputWorkbook.CreateCellStyle();
            IFont boldFont = _outputWorkbook.CreateFont();
            boldFont.IsBold = true;
            boldFontCellStyle.SetFont(boldFont);
            cell.CellStyle = boldFontCellStyle;
        }

        private byte[] SerializeWorkbook()
        {
            NpoiMemoryStream ms = new NpoiMemoryStream();
            using (NpoiMemoryStream tempStream = new NpoiMemoryStream())
            {
                tempStream.AllowClose = false;
                _outputWorkbook.Write(tempStream);
                tempStream.Flush();
                tempStream.Seek(0, SeekOrigin.Begin);
                tempStream.AllowClose = true;
                var byteArray = tempStream.ToArray();
                ms.Write(byteArray, 0, byteArray.Length);
                return ms.ToArray();
            }
        }

        private void MakeHeadersBold(IRow headersRow, List<string> headers)
        {
            for (int cursor = 0; cursor < headers.Count; cursor++)
            {
                var header = headers[cursor];
                var headerCell = headersRow.CreateCell(cursor);
                headerCell.SetCellType(CellType.String);
                ApplyBoldCellStyle(headerCell);
                headerCell.SetCellValue(header);
            }
        }

    }
}
