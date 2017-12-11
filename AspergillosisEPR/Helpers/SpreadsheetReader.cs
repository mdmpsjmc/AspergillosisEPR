using AspergillosisEPR.Models;
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

namespace AspergillosisEPR.Helpers
{
    public class SpreadsheetReader
    {
        private readonly FileStream _stream;
        public IFormFile _file { get; }
        public string _fileExtension { get; }
        public List<Patient> ImportedPatients { get; set; }
        public Hashtable _dictonary { get; private set; }
        private List<string> _headers;


        public SpreadsheetReader(FileStream stream, IFormFile file, string fileExtension)
        {
            _stream = stream;
            _file = file;
            _fileExtension = fileExtension;
            ImportedPatients = new List<Patient>();
            _dictonary = DbImport.HeadersDictionary();
            _headers = new List<string>();
            ReadSpreadsheetFile();
        }

        private void ReadSpreadsheetFile()
        {
            Patient patient;
            ISheet currentSheet;
            _file.CopyTo(_stream);
            _stream.Position = 0;
            if (_fileExtension == ".xls")
            {
                HSSFWorkbook workbook = new HSSFWorkbook(_stream);  //old excel
                currentSheet = workbook.GetSheetAt(0);
            }
            else
            {
                XSSFWorkbook workbook = new XSSFWorkbook(_stream); //new excel
                currentSheet = workbook.GetSheetAt(0);
            }

            IRow headerRow = currentSheet.GetRow(0); //Get Header Row
            int cellCount = headerRow.LastCellNum;

            GetSpreadsheetHeaders(headerRow);

            for (int rowsCursor = (currentSheet.FirstRowNum + 1); rowsCursor <= currentSheet.LastRowNum; rowsCursor++)
            {
                patient = new Patient();
                IRow row = currentSheet.GetRow(rowsCursor);

                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

                patient = ReadRowCellsIntoPatientObject(patient, row, cellCount);
                ImportedPatients.Add(patient);
            }
        }


        private Patient ReadRowCellsIntoPatientObject(Patient patient, IRow row, int cellCount)
        {
            for (int cellCursor = row.FirstCellNum; cellCursor < cellCount; cellCursor++)
            {
                if (row.GetCell(cellCursor) != null)
                {
                    readCell(cellCursor, row, patient);
                }
            }
            return patient;
        }

        private void readCell(int cellIndex, IRow row, Patient patient)
        {
            string rowValue = row.GetCell(cellIndex).ToString();
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
                            Type type = patient.GetType();
                            PropertyInfo propertyInfo = type.GetProperty(klassAndField[1]);
                            if (propertyInfo.PropertyType == typeof(DateTime)) // convert to date if its a date
                            {
                                DateTime dateRowValue = row.GetCell(cellIndex).DateCellValue;
                                propertyInfo.SetValue(patient, Convert.ChangeType(dateRowValue, propertyInfo.PropertyType), null);
                            }
                            else
                            {
                                propertyInfo.SetValue(patient, Convert.ChangeType(rowValue, propertyInfo.PropertyType), null);
                            }
                            break;
                    }
                }
            }
        }

        private void GetSpreadsheetHeaders(IRow headerRow)
        {
            for (int headerCellCursor = 0; headerCellCursor < headerRow.LastCellNum; headerCellCursor++)
            {
                ICell headerCell = headerRow.GetCell(headerCellCursor);
                if (headerCell == null || string.IsNullOrWhiteSpace(headerCell.ToString())) continue;
                _headers.Add(headerCell.ToString());
            }
        }
    }
}
