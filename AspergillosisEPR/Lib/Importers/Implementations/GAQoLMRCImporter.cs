using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Patients;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class GAQoLMRCImporter : Importer
    {
        private static string RM2NUMBER = "Field4";
        private static string DATE_TAKEN = "Field6";
        private static string WEIGHT = "Field8";
        private static string HEIGHT = "Field10";
        private static string SCORE = "Field9";

        public GAQoLMRCImporter(FileStream stream, IFormFile file,
                               string fileExtension, AspergillosisContext context)
        {
            _stream = stream;
            _file = file;
            _fileExtension = fileExtension;
            Imported = new List<dynamic>();
            _context = context;
            ReadCSVFile();
        }

        private void ReadCSVFile()
        {
            _file.CopyTo(_stream);
            _stream.Position = 0;
            ReadCSV();
        }

        private void ReadCSV()
        {
            TextReader textReader = new StreamReader(_stream);
            var csv = new CsvReader(textReader);
            ConfigureCSVReader(csv);
            try
            {
                var records = csv.GetRecords<dynamic>().ToList();
                foreach (var r in records)
                {
                    var record = new Dictionary<string, object>(r);
                    var rm2Number = GetRM2Number(record);
                    var patient = GetPatientByRM2Number(rm2Number);
                    if (patient == null) continue;
                    var score = BuildMRCScore(patient, record);
                    DateTime? dateTaken = null;
                    if (score == null)
                    {
                        string stringDateTaken = (string)record[DATE_TAKEN];
                        dateTaken = ParseDate(stringDateTaken);
                    } else
                    {
                        dateTaken = score.DateTaken;
                    }
                    BuildPatientMeasurement(patient, dateTaken, record);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ConfigureCSVReader(CsvReader csv)
        {
            csv.Configuration.BadDataFound = null;
            csv.Configuration.HasHeaderRecord = false;
            csv.Configuration.BadDataFound = context =>
            {
                Console.WriteLine($"Bad data found on row '{context.RawRow}'");
            };
        }

        private string GetRM2Number(Dictionary<string, object> record)
        {
            string rm2Number = (string)record[RM2NUMBER];
            return rm2Number.Replace("CPA", "").Trim();
        }

        private PatientMRCScore BuildMRCScore(Patient patient, IDictionary record)
        {
            var score = new PatientMRCScore();
            score.PatientId = patient.ID;
            var stringScore = (string)record[SCORE];
            if (stringScore == "NULL" || string.IsNullOrEmpty(stringScore)) return null;
            score.Score = stringScore;
            string stringDateTaken = (string)record[DATE_TAKEN];
            var dateTaken = ParseDate(stringDateTaken);
            if (dateTaken == null) return null;
            score.DateTaken = dateTaken.Value;
            _context.Entry(patient).Collection(p => p.PatientMRCScores).Load();
            var dates = patient.PatientMRCScores.Select(s => s.DateTaken.Date).ToList();
            var existingDbDates = dates.FindAll(d => d.Date == score.DateTaken.Date);
            if (existingDbDates.Count > 0) return null;
            Imported.Add(score);
            return score;
        }

        private DateTime? ParseDate(string dateTaken)
        {
            try
            {
                return DateTime.ParseExact(dateTaken, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                try
                {
                    return Convert.ToDateTime(dateTaken);
                }
                catch (FormatException)
                {
                    Console.WriteLine(dateTaken);
                    return null;
                }
            }
        }

        private void BuildPatientMeasurement(Patient patient, DateTime? dateTaken, IDictionary record)
        {
            string csvHeight = (string)record[HEIGHT];
            string csvWeight = (string)record[WEIGHT];
            if (dateTaken == null) return;
            if (!string.IsNullOrEmpty(csvHeight) || !string.IsNullOrEmpty(csvHeight))
            {
                decimal height, weight;
                decimal.TryParse(csvHeight, out height);
                decimal.TryParse(csvWeight, out weight);
                if (height != 0 || weight != 0)
                {
                    var measurement = new PatientMeasurement();
                    measurement.Weight = weight;
                    measurement.Height = height;
                    measurement.PatientId = patient.ID;
                    measurement.DateTaken = dateTaken.Value;
                    var existing = _context.PatientMeasurements
                                           .Where(m => m.PatientId == patient.ID && measurement.DateTaken.Date == dateTaken.Value.Date)
                                           .FirstOrDefault();
                    if (existing != null) return;
                    _context.PatientMeasurements.Add(measurement);
                }
            }
        }

        private Patient GetPatientByRM2Number(string rm2Nmber)
        {
            return _context.Patients.Where(p => p.RM2Number == rm2Nmber).FirstOrDefault();
        }
    }
}
