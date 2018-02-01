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
            foreach (var status in statuses)
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

        public static async void AddIgTypes(AspergillosisContext context)
        {
            if (context.ImmunoglobulinTypes.Any())
            {
                return;
            }

            var iGType = new ImmunoglobulinType[]
            {
                new ImmunoglobulinType { Name = "IgA"},
                new ImmunoglobulinType { Name = "IgD" },
                new ImmunoglobulinType { Name = "IgE"},
                new ImmunoglobulinType { Name = "IgG" },
                new ImmunoglobulinType { Name = "IgM"}
            };

            foreach (var ig in iGType)
            {
                context.Add(ig);
            }
            await context.SaveChangesAsync();
        }

        public static async void AddRadiologyModels(AspergillosisContext context)
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
                    await context.SaveChangesAsync();
                }
            }
        }
