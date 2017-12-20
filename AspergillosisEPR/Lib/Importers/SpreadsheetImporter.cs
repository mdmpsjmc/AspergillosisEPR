using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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
            _dictonary = DbImport.HeadersDictionary();
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

        protected abstract void ProcessSheet(ISheet currentSheet);
    }
}
