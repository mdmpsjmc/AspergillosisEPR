using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Radiology;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed.SeedFiles
{
    public static class RadiologyDataInitializer
    {
        public static  void AddRadiologyModels(AspergillosisContext context)
        {
            if (context.Findings.Any())
            {
                return;
            }

            var findings = new Finding[]
            {
                new Finding { Name = "Aspergilloma"},
                new Finding { Name = "Bronchiectasis"},
                new Finding { Name = "Consolidation" },
                new Finding { Name = "Cavitation"},
                new Finding { Name = "Emphysema" },
                new Finding { Name = "Lobar collapse"},
                new Finding { Name = "Lung fibrosis"},
                new Finding { Name = "Ground glass opacity" },
                new Finding { Name = "Lobar collapse"},
                new Finding { Name = "Lung nodules"},
                new Finding { Name = "Normal" }
            };

            foreach (var finding in findings)
            {
                context.Add(finding);
            }

            var locations = new ChestLocation[]
            {
                new ChestLocation { Name = "Apical"},
                new ChestLocation { Name = "Bilateral"},
                new ChestLocation { Name = "Lingular" },
                new ChestLocation { Name = "Multilobar"},
                new ChestLocation { Name = "Perihilar" },
                new ChestLocation { Name = "LLL"},
                new ChestLocation { Name = "LUL"},
                new ChestLocation { Name = "RUL" },
                new ChestLocation { Name = "RRL"},
                new ChestLocation { Name = "RML"},
                new ChestLocation { Name = "N/A"}

            };

            foreach (var location in locations)
            {
                context.Add(location);
            }

            var distributions = new ChestDistribution[]
            {
                new ChestDistribution { Name = "Apical"},
                new ChestDistribution { Name = "Unilateral"}

            };

            foreach (var dist in distributions)
            {
                context.Add(dist);
            }

            var grades = new Grade[]
             {
                new Grade { Name = "Mild"},
                new Grade { Name = "Moderate"},
                new Grade { Name = "Severe"}
             };

            foreach (var grade in grades)
            {
                context.Add(grade);

            }
            var treatmetnResponses = new TreatmentResponse[]
             {
                new TreatmentResponse { Name = "N/A"},
                new TreatmentResponse { Name = "Improving"},
                new TreatmentResponse { Name = "Worse"},
                new TreatmentResponse { Name = "New"},
                new TreatmentResponse { Name = "No Change"}
             };

            foreach (var tr in treatmetnResponses)
            {
                context.Add(tr);
            }


            var radiologyTypes = new RadiologyType[]
            {
                new RadiologyType { Name = "CXR"},
                new RadiologyType { Name = "CT"},
                new RadiologyType { Name = "HRCT"}
             };

            foreach (var rt in radiologyTypes)
            {
                context.Add(rt);
            }

             context.SaveChanges();
        }

        public static void OtherRadiologyTypes(AspergillosisContext context, IHostingEnvironment hostingEnvironment)
        {
            if (context.RadiologyTypes.Where(rt => rt.Name.Equals("CCABDC")).Any()) return;
            var rootDirectory = hostingEnvironment.ContentRootPath;
            var dataFile = File.OpenRead(Path.Join(rootDirectory, "Data/DatabaseSeed/radiology.csv"));
            var files = new List<FileStream>() { dataFile };
            for (int i = 0; i < files.Count; i++)
            {
                var csvFile = files[i];

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
                        RadiologyType radiology;
                        radiology = new RadiologyType();
                        radiology.Name = (string)record["Field1"];
                        radiology.Description = (string)record["Field2"];

                        context.RadiologyTypes.Add(radiology);
                    }
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
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
