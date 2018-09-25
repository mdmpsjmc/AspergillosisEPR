using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed.SeedFiles
{
    public static class ManArtsProcessedFileSeed
    {
        public static void Seed(AspergillosisContext context)
        {
            var importer = context.DBImportTypes.Where(dbit => dbit.ImporterClass == "ManArtsSurgeriesAndNotesImporter").FirstOrDefault();
            if (importer == null)
            {
                var dbImportType = new DbImportType { Name = "ManArts Import from processed and merged file (Surgeries and notes - Mike Porter)", ImporterClass = "ManArtsSurgeriesAndNotesImporter" };
                context.DBImportTypes.Add(dbImportType);
                context.SaveChanges();
            }
            else
            {
                return;
            }
        }

        public static void SeedSmokingStatuses(AspergillosisContext context)
        {
            var importer = context.DBImportTypes.Where(dbit => dbit.ImporterClass == "ManArtsSmokingStatusImporter").FirstOrDefault();
            if (importer == null)
            {
                var dbImportType = new DbImportType { Name = "ManArts Import from processed and merged file (Smoking Status - Mike Porter)", ImporterClass = "ManArtsSmokingStatusImporter" };
                context.DBImportTypes.Add(dbImportType);
                context.SaveChanges();
            }
            else
            {
                return;
            }
        }

        public static void SeedDrugLevel(AspergillosisContext context)
        {
            var importer = context.DBImportTypes.Where(dbit => dbit.ImporterClass == "ManARTSDrugLevelsImporter").FirstOrDefault();
            if (importer == null)
            {
                var dbImportType = new DbImportType { Name = "ManArts Import from processed and merged file (Drug Levels - Mike Porter)", ImporterClass = "ManARTSDrugLevelsImporter" };
                context.DBImportTypes.Add(dbImportType);
                context.SaveChanges();
            }
            else
            {
                return;
            }
        }

        public static void SeedPFTandHaematology(AspergillosisContext context)
        {
            var importer = context.DBImportTypes.Where(dbit => dbit.ImporterClass == "ManARTSPFTAndHeamatologyImporter").FirstOrDefault();
            if (importer == null)
            {
                var dbImportType = new DbImportType { Name = "ManArts Import from processed and merged file (PFTs and Haemo Levels - Mike Porter)", ImporterClass = "ManARTSPFTAndHeamatologyImporter" };
                context.DBImportTypes.Add(dbImportType);
                context.SaveChanges();
            }
            else
            {
                return;
            }
        }
    }
}
