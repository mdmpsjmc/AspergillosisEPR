using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed.SeedFiles
{
    public static class IGgEPRImportTypeSeed
    {
        public static void Seed(AspergillosisContext context)
        {
            var importer = context.DBImportTypes.Where(dbit => dbit.ImporterClass == "IGgLevelEPRImporter").FirstOrDefault();
            if (importer == null)
            {
                var dbImportType = new DbImportType { Name = "Import IGg Pdf Report from EPR", ImporterClass = "IGgLevelEPRImporter" };
                context.DBImportTypes.Add(dbImportType);
                context.SaveChangesAsync();
            }
            else
            {
                return;
            }
        }
    }
}
