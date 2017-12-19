using AspergillosisEPR.Models;
using System;
using System.Linq;

namespace AspergillosisEPR.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AspergillosisContext context)
        {
            context.Database.EnsureCreated();
            if (context.DiagnosisCategories.Any())
            {
                return;  
            }

            var Patients = new Patient[]
            {
            new Patient{FirstName="Carson",LastName="Alexander",DOB=DateTime.Parse("2005-09-01"),Gender="male", RM2Number="1234576RD"},
            new Patient{FirstName="Meredith",LastName="Alonso",DOB=DateTime.Parse("2002-09-01"), Gender="male", RM2Number="3454576RD"},
            new Patient { FirstName = "Arturo", LastName = "Anand", DOB = DateTime.Parse("2003-09-01"), Gender = "male" , RM2Number = "AD23445"},
            new Patient{FirstName="Gytis",LastName="Barzdukas",DOB=DateTime.Parse("2002-09-01"), Gender="female",RM2Number="AD5467676"},
            new Patient{FirstName="Yan",LastName="Li",DOB=DateTime.Parse("2002-09-01"), Gender="male",RM2Number="VVV333355"},
            new Patient{FirstName="Peggy",LastName="Justice",DOB=DateTime.Parse("2001-09-01"), Gender="female",RM2Number="ASH34578" },
            new Patient{FirstName="Laura",LastName="Norman",DOB=DateTime.Parse("2003-09-01"), Gender="female", RM2Number="POL123445"},
            new Patient{FirstName="Nino",LastName="Olivetto",DOB=DateTime.Parse("2005-09-01"), Gender="male", RM2Number="DAT123445"}
            };
            foreach (Patient p in Patients)
            {
                context.Patients.Add(p);
            }
            context.SaveChanges();

            var DiagnosisCategories = new DiagnosisCategory[]
            {
            new DiagnosisCategory{CategoryName="Primary"},
            new DiagnosisCategory{CategoryName="Secondary"},
            new DiagnosisCategory{CategoryName="Other"},
            };
            foreach (DiagnosisCategory c in DiagnosisCategories)
            {
                context.DiagnosisCategories.Add(c);
            }
            context.SaveChanges();

            var diagnoses = new DiagnosisType[]
            {
            new DiagnosisType{Name="Allergic bronchopulmonary aspergillosis (ABPA)"},
            new DiagnosisType{Name="Allergic Aspergillus sinusitis"},
            new DiagnosisType{Name="Aspergilloma"},
            new DiagnosisType{Name="Chronic pulmonary aspergillosis (CPA)"},
            new DiagnosisType{Name="Invasive aspergillosis"},
            new DiagnosisType{Name="Cutaneous (skin) aspergillosis"},
            };
            foreach (DiagnosisType dt in diagnoses)
            {
                context.DiagnosisTypes.Add(dt);
            }
            context.SaveChanges();


            var drugs = new Drug[]
            {
            new Drug{Name="Other drug 1"},
            new Drug{Name="Other drug"},
            new Drug{Name="Other drug 2"},
            new Drug{Name="Other drug 3"},
            new Drug{Name="Other drug 4"},
            new Drug{Name="Other drug 7"},
            };
            foreach (Drug d in drugs)
            {
                context.Drugs.Add(d);
            }

            var sideEffects = new SideEffect[]
           {
            new SideEffect{Name="Clumsiness"},
            new SideEffect{Name="Discouragement"},
            new SideEffect{Name="Drowsiness"},
            new SideEffect{Name="Feeling sad or empty"},
            new SideEffect{Name="Irritability"},
            new SideEffect{Name="Vomiting"},
            new SideEffect{Name="Fever"},
            new SideEffect{Name="Irregular heartbeats"}
           };
            foreach (SideEffect se in sideEffects)
            {
                context.SideEffects.Add(se);
            }
            context.SaveChanges();
        }

        public static void AddDefaultPatientStatuses(AspergillosisContext context)
        {
            if (context.PatientStatuses.Any())
            {
                return;
            }
            var statuses = new PatientStatus[]
            {
                new PatientStatus{Name="Active"},
                new PatientStatus{Name="Discharged"},
                new PatientStatus{Name="Inactive"},
                new PatientStatus{Name="Deceased"},
            };
            foreach(var status in statuses)
            {
                context.Add(status);
            }
            context.SaveChanges();
       }

        public static async void CreateDbImportTypes(AspergillosisContext context)
        {
            if (context.DBImportTypes.Any())
            {
                return;
            }

            var dbImportTypes = new DbImportType[]
            {
                new DbImportType {Name = "Chris Harris - Excel Spreadsheet", ImporterClass = "CHSpreadsheetImporter" },
                new DbImportType { Name = "Graham Atherton - QoL CSV file", ImporterClass = "GAQoLCSVImporter" },
                new DbImportType { Name = "David Lowes - Excel Spreadsheet", ImporterClass = "DLSpreadsheetImporter" }
            };

            foreach (var dbImportType in dbImportTypes)
            {
                context.Add(dbImportType);
            }
            await context.SaveChangesAsync();
        }
    }
}