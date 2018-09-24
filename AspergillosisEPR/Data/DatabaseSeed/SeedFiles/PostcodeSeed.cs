using AspergillosisEPR.Models;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed.SeedFiles
{
    public static class PostcodeSeed
    {
        public static void ReadCsvIntoDatabase(IHostingEnvironment appEnvironment,
                                               AspergillosisContext context)
        {
            if (context.UKOutwardCodes.Any()) return;
            var rootDirectory = appEnvironment.ContentRootPath;
            var csvFile = File.OpenRead(Path.Join(rootDirectory, "Data/DatabaseSeed/postcode-outcodes.csv"));
            TextReader textReader = new StreamReader(csvFile);
            var csv = new CsvReader(textReader);
            ConfigureCSVReader(csv);
            try
            {
                var records = csv.GetRecords<dynamic>().ToList();

                foreach (var r in records)
                {
                    if ((records.IndexOf(r)) == 0) continue;
                    var record = new Dictionary<string, object>(r);
                    
                    var postCode = new UKOutwardCode();
                    postCode.Code = (string)record["Field2"];
                    postCode.Latitude = Decimal.Parse((string)record["Field3"]);
                    postCode.Longitude = Decimal.Parse((string)record["Field4"]);
              
                    context.UKOutwardCodes.Add(postCode);
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ConfigureCSVReader(CsvReader csv)
        {
            csv.Configuration.BadDataFound = null;
            csv.Configuration.HasHeaderRecord = false;
            csv.Configuration.BadDataFound = context =>
            {
                Console.WriteLine($"Bad data found on row '{context.RawRow}'");
            };
        }
    }
}
