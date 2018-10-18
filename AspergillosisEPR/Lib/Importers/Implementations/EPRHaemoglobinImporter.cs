using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class EPRHaemoglobinImporter : Importer
    {

        private List<string> _lines;
        private string _id;
        private UnitOfMeasurement _uom;

        public EPRHaemoglobinImporter(FileStream stream, IFormFile file,
                                      string fileExtension, 
                                       AspergillosisContext context)
        {
            _stream = stream;
            _file = file;
            _fileExtension = fileExtension;
            Imported = new List<dynamic>();
            _context = context;
            ReadCSVFile();
            _lines = new List<string>();
            _uom = context.UnitOfMeasurements.Where(uom => uom.Name == "g/l").FirstOrDefault();
        }

        private void ReadCSVFile()
        {
            _file.CopyTo(_stream);
            _stream.Position = 0;
            _lines = ReadPDFText().Split("\n").ToList();
            GetIdentifier();
            var matched = ReadValues();
            ParseAndSaveLines(matched);
        }

        private void ParseAndSaveLines(List<string> matched)
        {
            _uom = _context.UnitOfMeasurements.Where(uom => uom.Name == "g/l").FirstOrDefault();

            var patient = _context.Patients.Where(p => p.RM2Number.Contains(_id))
                                           .Include(p => p.PatientTestResults)
                                             .ThenInclude(pt => pt.TestType)
                                           .FirstOrDefault();

            var testType = _context.TestTypes
                                   .Where(tt => tt.Name == "Haemoglobin")
                                   .FirstOrDefault();

            var existingDates = patient.PatientTestResults
                                       .Where(ig => ig.TestTypeId == testType.ID)
                                       .Select(pi => pi.DateTaken.Date)
                                       .ToList();

            foreach (string line in matched)
            {
                var dataArray = line.Split(" ");
                string range = "";
                string dateString = dataArray.Take(3).ToList().Join(" ");
                string testValue = dataArray[5].Replace("*", String.Empty)
                                               .Replace("<", String.Empty)
                                               .Replace(">", String.Empty);
                string sampleId = dataArray[4];
                if (dataArray.Length > 6)
                {
                    range = dataArray[6] + dataArray[7] + dataArray[8];
                }
                else
                {
                    range = "(" + Regex.Match(_lines[12], @"\(([^)]*)\)").Groups[1].Value + ")";
                    Console.WriteLine(dataArray);
                }
                DateTime parsedDate;
                DateTime.TryParseExact(dateString, "dd MMM yyyy", null, System.Globalization.DateTimeStyles.None, out parsedDate);
                if (existingDates.FindAll(d => d.Date == parsedDate.Date).ToList().Count == 0)
                {
                    var crp = new PatientTestResult();
                    crp.TestTypeId = testType.ID;
                    crp.SampleId = sampleId;
                    crp.Range = range;
                    crp.PatientId = patient.ID;
                    crp.Value = decimal.Parse(testValue);
                    crp.DateTaken = parsedDate;
                    crp.UnitOfMeasurement = _uom;
                    _context.PatientTestResult.Add(crp);
                    Imported.Add(crp);
                }
            }
        }

        private string GetIdentifier()
        {
            _id = _lines.Where(l => l.Contains("Hospital number"))
                                     .FirstOrDefault()
                                     .Split(":")
                                     .Last()
                                     .Trim();
            return _id;
        }

        private List<string> ReadValues()
        {
            var firstCharacterIsNumber = new Regex("^[0-9]");
            var matched = new List<string>();
            foreach (string line in _lines)
            {
                if (firstCharacterIsNumber.IsMatch(line))
                {
                    matched.Add(line);
                }
            }
            return matched;
        }

        private string ReadPDFText()
        {
            using (PdfReader reader = new PdfReader(_stream))
            {
                StringBuilder text = new StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append("\n" + PdfTextExtractor.GetTextFromPage(reader, i));
                }

                return text.ToString();
            }
        }

     
    }
}
