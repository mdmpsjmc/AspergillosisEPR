using AspergillosisEPR.Data.DatabaseSeed.SeedFiles;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed
{
    public class AspergillosisDatabaseSeeder
    {
        public static void SeedDatabase(IHostingEnvironment hostingEnvironment, 
                                        AspergillosisContext context)
        {
            SampleDataInitializer.Initialize(context);
            SampleDataInitializer.AddDefaultPatientStatuses(context);
            SampleDataInitializer.CreateDbImportTypes(context);
            SampleDataInitializer.AddIgTypes(context);
            RadiologyDataInitializer.AddRadiologyModels(context);
            CaseReportFormsDataInitializer.AddCaseReportFormsModels(context);
            QoLExcelImportType.Seed(context);
            IGgEPRImportTypeSeed.Seed(context);
            CaseReportFormsDataInitializer.AddSelectFieldTypes(context);
            MedicalTiralsDataInitializer.AddMedicalTrialsModels(context);
            IntraDrugLevelExcelTypeSeed.Seed(context);
            UnitOfMeasureMgLSeed.Seed(context);
            ManArtsImportSeed.Seed(context);
            ManArtsImportSeed.AddOtherPFTs(context);
            ManArtsProcessedFileSeed.Seed(context);
            ManArtsProcessedFileSeed.SeedSmokingStatuses(context);
            ManArtsProcessedFileSeed.SeedDrugLevel(context);
            EPRClinicLetterDbImportTypeSeed.Seed(context);
            FoodDatabaseSeed.SeedDefaultFoods(context);
            OtherAllergicItemDatabaseSeed.SeedDefaultItems(context);
            FungiAllergicItemDatabaseSeed.SeedDefaultItems(context);
            ReportTypeSeed.Initialize(context);
            PostcodeSeed.ReadCsvIntoDatabase(hostingEnvironment, context);
        }
    }
}
