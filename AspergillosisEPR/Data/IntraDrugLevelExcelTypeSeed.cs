using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data
{
    public class IntraDrugLevelExcelTypeSeed
    {
        public static void Seed(AspergillosisContext context)
        {
            var importer = context.DBImportTypes.Where(dbit => dbit.ImporterClass == "IntraDrugLevelExcelImporter").FirstOrDefault();
            if (importer == null)
            {
                var dbImportType = new DbImportType { Name = "Import Intraconazole Drug Levels", ImporterClass = "IntraDrugLevelExcelImporter" };
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
