using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AspergillosisEPR.Lib.Importers.Implementations
{
    public class GAQoLCSVImporter : Importer
    {
        private static string RM2NUMBER = "Field4";
        private static string DATE_TAKEN = "Field6";
        private static string WEIGHT = "Field8";
        private static string HEIGHT = "Field10";
        private static string SYMPTOM_SCORE = "Field119";
        private static string IMPACT_SCORE = "Field120";
        private static string ACTIVITY_SCORE = "Field121";
        private static string TOTAL_SCORE = "Field122";

        public GAQoLCSVImporter(FileStream stream, IFormFile file,
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
                    var sgrquestionnaire = BuildPatientSTGQuestionnaire(patient, record);
                    BuildPatientMeasurement(patient, sgrquestionnaire.DateTaken, record);
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
            return  rm2Number.Replace("CPA", "").Trim();
        }

        private PatientSTGQuestionnaire BuildPatientSTGQuestionnaire(Patient patient, IDictionary record)
        {
            var sgrquestionare = new PatientSTGQuestionnaire();
            sgrquestionare.PatientId = patient.ID;
            sgrquestionare.SymptomScore = decimal.Parse((string) record[SYMPTOM_SCORE]);
            sgrquestionare.ImpactScore = decimal.Parse((string) record[IMPACT_SCORE]);
            sgrquestionare.ActivityScore = decimal.Parse((string) record[ACTIVITY_SCORE]);
            sgrquestionare.TotalScore = decimal.Parse((string) record[TOTAL_SCORE]);
            string dateTaken = (string)record[DATE_TAKEN];
            try
            {
                sgrquestionare.DateTaken = DateTime.ParseExact(dateTaken, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            } catch (FormatException)
            {
                try
                {
                    sgrquestionare.DateTaken = Convert.ToDateTime(dateTaken);
                } catch (FormatException)
                {
                    Console.WriteLine(dateTaken);
                }
                
            }
            _context.Entry(patient).Collection(p => p.STGQuestionnaires).Load();
            var dates = patient.STGQuestionnaires.Select(sgrq => sgrq.DateTaken.Date).ToList();
            var existingDbDates = dates.FindAll(d => d.Date == sgrquestionare.DateTaken.Date);
            if (existingDbDates.Count > 0) return sgrquestionare;
            if (sgrquestionare.IsValid())
            {
                Imported.Add(sgrquestionare);
            }
            return sgrquestionare;
        }

        private void BuildPatientMeasurement(Patient patient, DateTime dateTaken, IDictionary record)
        {
            string csvHeight = (string)record[HEIGHT];
            string csvWeight = (string)record[WEIGHT];
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
                    measurement.DateTaken = dateTaken;
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
