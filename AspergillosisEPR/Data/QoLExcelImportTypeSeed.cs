using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data
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

    }
}
