using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace AspergillosisEPR.Lib.Importers
{
    public class GAQoLCSVImporter : Importer
    {
        private static string RM2NUMBER = "Field4";
        private static string DATE_TAKEN = "Field5";
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
                    BuildPatientSTGQuestionnaire(patient, record);
                }
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
            var stgQuestionnaire = new PatientSTGQuestionnaire();
            stgQuestionnaire.PatientId = patient.ID;
            stgQuestionnaire.SymptomScore = decimal.Parse((string) record[SYMPTOM_SCORE]);
            stgQuestionnaire.ImpactScore = decimal.Parse((string) record[IMPACT_SCORE]);
            stgQuestionnaire.ActivityScore = decimal.Parse((string) record[ACTIVITY_SCORE]);
            stgQuestionnaire.TotalScore = decimal.Parse((string) record[TOTAL_SCORE]);
            stgQuestionnaire.DateTaken = DateTime.Parse((string) record[DATE_TAKEN]);
            Imported.Add(stgQuestionnaire);
            return stgQuestionnaire;
        }

        private Patient GetPatientByRM2Number(string rm2Nmber)
        {
            return _context.Patients.Where(p => p.RM2Number == rm2Nmber).FirstOrDefault();
        }
    }
}
