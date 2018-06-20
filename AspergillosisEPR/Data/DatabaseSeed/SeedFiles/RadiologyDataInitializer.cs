using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Radiology;
using System;
using System.Collections.Generic;
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
    }
}
