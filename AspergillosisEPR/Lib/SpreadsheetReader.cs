﻿using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Reflection;
using AspergillosisEPR.Lib;
using AspergillosisEPR.Data;

namespace AspergillosisEPR.Helpers
{
    public class SpreadsheetReader
    {
        private readonly FileStream _stream;
        private IFormFile _file { get; }
        private string _fileExtension { get; }     
        private Hashtable _dictonary { get; set; }
        public static string UNDERLYING_DISEASE_HEADER = "Underlying disease";
        public List<Patient> ImportedPatients { get; set; }
        private List<string> _headers;
        private readonly AspergillosisContext _context;


        public SpreadsheetReader(FileStream stream, IFormFile file, 
                                 string fileExtension,  AspergillosisContext context)
        {
            _stream = stream;
            _file = file;
            _fileExtension = fileExtension;
            ImportedPatients = new List<Patient>();
            _dictonary = DbImport.HeadersDictionary();
            _headers = new List<string>();
            _context = context;
            ReadSpreadsheetFile();
        }

        private void ReadSpreadsheetFile()
        {
            _file.CopyTo(_stream);
            _stream.Position = 0;
            if (_fileExtension == ".xls")
            {
                HSSFWorkbook workbook = new HSSFWorkbook(_stream);  //old excel
                processSheets(workbook);
            }
            else
            {
                XSSFWorkbook workbook = new XSSFWorkbook(_stream); //new excel
                processSheets(workbook);
            }            
        }

        private void processSheets(XSSFWorkbook workbook)
        {
            for(int tabIndex=0; tabIndex < workbook.NumberOfSheets; tabIndex++)
            {
                ISheet currentSheet = workbook.GetSheetAt(tabIndex);
                processSheet(currentSheet);
            }
        }

        private void processSheets(HSSFWorkbook workbook)
        {
            for (int tabIndex = 0; tabIndex < workbook.NumberOfSheets; tabIndex++)
            {
                ISheet currentSheet = workbook.GetSheetAt(tabIndex);
                processSheet(currentSheet);
            }
        }

        private void processSheet(ISheet currentSheet)
        {
            Patient patient;
            IRow headerRow = currentSheet.GetRow(0); //Get Header Row
            

            GetSpreadsheetHeaders(headerRow);
            int cellCount2= headerRow.Cells.GetRange(0, _headers.Count()).Count;
            int cellCount = headerRow.LastCellNum;

            for (int rowsCursor = (currentSheet.FirstRowNum + 1); rowsCursor <= currentSheet.LastRowNum; rowsCursor++)
            {
                patient = new Patient();
                IRow row = currentSheet.GetRow(rowsCursor);

                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                patient = ReadRowCellsIntoPatientObject(patient, row, cellCount2);
                ImportedPatients.Add(patient);
            }
        }

        private Patient ReadRowCellsIntoPatientObject(Patient patient, IRow row, int cellCount)
        {
            List<string> diagnosesNames = new List<string>();
            for (int cellCursor = row.FirstCellNum; cellCursor < cellCount; cellCursor++)
            {
                if (row.GetCell(cellCursor) != null)
                {
                    readCell(patient, row, cellCursor, diagnosesNames);
                }
            }
            var patientDiagnosisResolver = new PatientDiagnosisResolver(patient, diagnosesNames, _context);
            patient.PatientDiagnoses = patientDiagnosisResolver.Resolve();
            return patient;
        }

        private void readCell(Patient patient, IRow row, int cellIndex, List<string> diagnosesNames)
        {
            string header = _headers.ElementAt(cellIndex);
            string newObjectFields = (string)_dictonary[header];
            if (newObjectFields != null)
            {
                string[] fields = newObjectFields.Split("|");
                foreach (string field in fields)
                {
                    var klassAndField = field.Split(".");
                    switch (klassAndField[0])
                    {
                        case "Patient":
                            SetPatientProperty(patient, row, cellIndex, klassAndField[1]);
                            break;
                        case "PatientDiagnosis":
                            var hasDiagnosis = row.GetCell(cellIndex).ToString().Trim();
                            if (hasDiagnosis == "X")
                            {
                                diagnosesNames.Add(_headers[cellIndex]);
                            }
                            if (_headers[cellIndex] == UNDERLYING_DISEASE_HEADER)
                            {
                                diagnosesNames.Add(hasDiagnosis);
                            }
                            break;
                    }
                }
            }
        }

        private void SetPatientProperty(Patient patient, IRow row, int cellIndex, string property)
        {
            Type type = patient.GetType();
            PropertyInfo propertyInfo = type.GetProperty(property);
            DateTime dateRowValue;
            if (propertyInfo != null && propertyInfo.PropertyType == typeof(DateTime)) // convert to date if its a date
            {
                try
                {
                    dateRowValue = row.GetCell(cellIndex).DateCellValue;
                }
                catch (InvalidDataException ex)
                {
                    dateRowValue = DateTime.Parse(row.GetCell(cellIndex).ToString());
                    Console.WriteLine(ex.Message);
                }                
                propertyInfo.SetValue(patient, Convert.ChangeType(dateRowValue, propertyInfo.PropertyType), null);
            }
            else 
            {
                string stringRowValue = row.GetCell(cellIndex).ToString();
                if (stringRowValue != "")
                {
                    try
                    {
                        propertyInfo.SetValue(patient, Convert.ChangeType(stringRowValue, propertyInfo.PropertyType), null);
                    }
                    catch (InvalidCastException)
                    {// date in a text field
                        dateRowValue = row.GetCell(cellIndex).DateCellValue;
                        propertyInfo.SetValue(patient, dateRowValue);                        
                    }
                }                
            }
        }

        private void GetSpreadsheetHeaders(IRow headerRow)
        {
            _headers = new List<string>();
            for (int headerCellCursor = 0; headerCellCursor < headerRow.LastCellNum; headerCellCursor++)
            {
                ICell headerCell = headerRow.GetCell(headerCellCursor);
                if (headerCell == null || string.IsNullOrWhiteSpace(headerCell.ToString())) continue;
                _headers.Add(headerCell.ToString());
            }
        }
    }
}
