using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using System.Collections;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class IntraDrugLevelExcelImporter : SpreadsheetImporter
    {
        public IntraDrugLevelExcelImporter(FileStream stream, 
                                           IFormFile file, 
                                           string fileExtension, AspergillosisContext context) : base(stream, file, fileExtension, context)
        {
            _context = context;
        }

        public static Hashtable HeadersDictionary()
        {
            return new Hashtable()
            {
                  { "SURNAME", "Patient.LastName" },
                  { "FORENAME", "Patient.FirstName" },
                  { "RM2", "Patient.RM2Number" },
                  { "NHSNO", "Patient.NhsNumber"},
                  { "DOB", "Patient.DOB"},
                  { "DATE TAKEN", "PatientDrugLevel.DateTaken"},
                  { "DATE RECEIVED", "PatientDrugLevel.DateReceived"},
                  { "RESULT -  mg/L", "PatientDrugLevel.ResultValue"}
             };
        }


        protected override List<string> IdentiferHeaders()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessSheet(ISheet currentSheet)
        {
            throw new NotImplementedException();
        }
    }
}
