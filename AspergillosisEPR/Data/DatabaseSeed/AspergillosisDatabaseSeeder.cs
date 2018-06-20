using AspergillosisEPR.Data.DatabaseSeed.SeedFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed
{
    public class AspergillosisDatabaseSeeder
    {
        public static void SeedDatabase(AspergillosisContext context)
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
            ManArtsProcessedFileSeed.Seed(context);
            ManArtsProcessedFileSeed.SeedSmokingStatuses(context);
            EPRClinicLetterDbImportTypeSeed.Seed(context);
        }
    }
}
