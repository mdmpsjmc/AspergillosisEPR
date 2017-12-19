using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace AspergillosisEPR.Lib.Importers
{
    public class GAQoLCSVImporter : Importer
    {
        private Dictionary<string, PatientSTGQuestionnaire> _csvImports;
        public GAQoLCSVImporter(FileStream stream, IFormFile file,
                                 string fileExtension, AspergillosisContext context)
        {
            _stream = stream;
            _file = file;
            _fileExtension = fileExtension;
            Imported = new List<dynamic>();
            _context = context;
            _csvImports = new Dictionary<string, PatientSTGQuestionnaire>();
            ReadCSVFile();
        }

        private void ReadCSVFile()
        {
            _file.CopyTo(_stream);
            _stream.Position = 0;
            TextReader textReader = new StreamReader(_stream);
            //var lines = ReadAllLines(textReader);            
            var csv = new CsvReader(textReader);
            csv.Configuration.BadDataFound = null;
            csv.Configuration.BadDataFound = context =>
            {
                Console.WriteLine($"Bad data found on row '{context.RawRow}'");
            };

            try
            {
                var records = csv.GetRecords<dynamic>();
                Console.WriteLine(records.Count());
            }
            catch (Exception ex)
            {

                // Log bad data.
               

                Console.WriteLine(ex.Message);
            } 

        }

        private IEnumerable<string> ReadAllLines(TextReader textReader)
        {
            string line;
            var lines = new List<string>();
            while((line = textReader.ReadLine()) != null)
            {
                lines.Add(line);
            }
            return lines;
        }
    }
}
