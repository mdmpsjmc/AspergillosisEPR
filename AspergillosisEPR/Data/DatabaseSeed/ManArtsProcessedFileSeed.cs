using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed
{
    public class ManArtsProcessedFileSeed
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
    }
}
