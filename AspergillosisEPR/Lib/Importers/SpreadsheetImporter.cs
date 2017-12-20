using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AspergillosisEPR.Lib.Importers
{
    public abstract class SpreadsheetImporter : Importer
    {
        protected List<string> _headers;
        protected Hashtable _dictonary { get; set; }

        public SpreadsheetImporter(FileStream stream, IFormFile file,
                                string fileExtension, AspergillosisContext context)
        {
            _stream = stream;
            _file = file;
            _fileExtension = fileExtension;
            Imported = new List<dynamic>();
            _headers = new List<string>();
            _context = context;
            ReadSpreadsheetFile();
        }

        protected void ReadSpreadsheetFile()
        {
            _file.CopyTo(_stream);
            _stream.Position = 0;
            if (_fileExtension == ".xls")
            {
                HSSFWorkbook workbook = new HSSFWorkbook(_stream);  //old excel
                ProcessSheets(workbook);
            }
            else
            {
                XSSFWorkbook workbook = new XSSFWorkbook(_stream); //new excel
                ProcessSheets(workbook);
            }
        }

        protected void ProcessSheets(XSSFWorkbook workbook)
        {
            for (int tabIndex = 0; tabIndex < workbook.NumberOfSheets; tabIndex++)
            {
                ISheet currentSheet = workbook.GetSheetAt(tabIndex);
                ProcessSheet(currentSheet);
            }
        }

        protected void ProcessSheets(HSSFWorkbook workbook)
        {
            for (int tabIndex = 0; tabIndex < workbook.NumberOfSheets; tabIndex++)
            {
                ISheet currentSheet = workbook.GetSheetAt(tabIndex);
                ProcessSheet(currentSheet);
            }
        }

        protected void GetSpreadsheetHeaders(IRow headerRow)
        {
            _headers = new List<string>();
            for (int headerCellCursor = 0; headerCellCursor < headerRow.LastCellNum; headerCellCursor++)
            {
                ICell headerCell = headerRow.GetCell(headerCellCursor);
                if (headerCell == null || string.IsNullOrWhiteSpace(headerCell.ToString())) continue;
                _headers.Add(headerCell.ToString());
            }
        }

        protected IRow InitializeHeaders(Hashtable headers, ISheet currentSheet)
        {
            _dictonary = headers;
            IRow headerRow = currentSheet.GetRow(0); //Get Header Row

            GetSpreadsheetHeaders(headerRow);
            return headerRow;
        }

        protected void InitializeSheetProcessingForRows(Hashtable headers, ISheet currentSheet, Action<Patient, IRow, int> sheetProcessingAction)
        {
            IRow headerRow = InitializeHeaders(headers, currentSheet);
            int cellCount = headerRow.Cells.GetRange(0, _headers.Count()).Count;

            Patient patient;

            for (int rowsCursor = (currentSheet.FirstRowNum + 1); rowsCursor <= currentSheet.LastRowNum; rowsCursor++)
            {
                patient = new Patient();
                IRow row = currentSheet.GetRow(rowsCursor);

                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                sheetProcessingAction(patient, row, cellCount);
            }
        }

        protected Patient ExistingPatient(string rRM2Number)
        {
            return Imported.Where(p => p.RM2Number == rRM2Number).FirstOrDefault();
        }

        protected abstract void ProcessSheet(ISheet currentSheet);
    }
}
