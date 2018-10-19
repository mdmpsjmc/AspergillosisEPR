using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using Microsoft.AspNetCore.Http;
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
        private List<string> _outputSheetNames = new List<string>()
        {
            "Demographics",
            "Diagnoses",
            "SGRQ",
            "MRC",
            "Weight",
            "PFT",
            "Aspergillus F IgG",
            "Total IgE",
            "C-Reactive Protein",
            "Heamoglobin", 
            "WBC",
            "Lymphocytes"
        };

        public CPAMortalityAuditReportBuilder(AspergillosisContext context, 
                                              FileStream inputFileStream,
                                              IFormFile formFile)
        {
            _context = context;            
            _outputWorkbook = new XSSFWorkbook();
            _stream = inputFileStream;
            _file = formFile;
        }

        public void Build()
        {
            var ids = GetPatientIdentifiers();
            var patients = _context.Patients
                                   .Where(p => ids.Contains(p.RM2Number))
                                   .OrderByDescending(p => p.LastName)
                                   .ToList();
            BuildDemographicsTab(patients);
        }

        private void BuildDemographicsTab(List<Patient> patients)
        {
            ISheet currentSheet = _outputWorkbook.CreateSheet(_outputSheetNames[0]);
            for(int patientCursor = 0; patientCursor < patients.Count(); patientCursor++)
            {
                var currentPatient = patients.ToList()[patientCursor];
                var currentRow = currentSheet.CreateRow(patientCursor);
                _context.Entry(currentPatient).Collection(p => p.PatientNACDates).Load();                
            }
        }

        private List<string> GetPatientIdentifiers()
        {
            _file.CopyTo(_stream);
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

    }


}
