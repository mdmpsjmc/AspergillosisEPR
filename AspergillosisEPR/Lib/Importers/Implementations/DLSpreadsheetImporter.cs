using NPOI.SS.UserModel;
using System.IO;
using Microsoft.AspNetCore.Http;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class DLSpreadsheetImporter : SpreadsheetImporter 
    {
        public DLSpreadsheetImporter(FileStream stream, IFormFile file,
                                string fileExtension, AspergillosisContext context) : base(stream, file, fileExtension, context)

        {
            _dictonary = DbImport.HeadersDictionary();
        }

        protected override void ProcessSheet(ISheet currentSheet) 
        {
            Patient patient;
            IRow headerRow = currentSheet.GetRow(0); //Get Header Row

            GetSpreadsheetHeaders(headerRow);
        }
    }
}
