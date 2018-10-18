using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed.SeedFiles
{
    public static class QoLExcelImportType
    {
        public static void Seed(AspergillosisContext context)
        {
            var importer = context.DBImportTypes.Where(dbit => dbit.ImporterClass == "QoLSpreadsheetImporter").FirstOrDefault();
            if (importer == null)
            {
                var dbImportType = new DbImportType { Name = "Demographics Excel Created From QoL", ImporterClass = "QoLSpreadsheetImporter" };
                context.DBImportTypes.Add(dbImportType);
                context.SaveChangesAsync();
            } else
            {
                return;
            }      
        }

        public static void SeedMRC(AspergillosisContext context)
        {
            var importer = context.DBImportTypes.Where(dbit => dbit.ImporterClass == "GAQoLMRCImporter").FirstOrDefault();
            if (importer == null)
            {
                var dbImportType = new DbImportType { Name = "MRC Scores from QOL (and new weight)", ImporterClass = "GAQoLMRCImporter" };
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
