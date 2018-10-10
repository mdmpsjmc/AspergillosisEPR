using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed.SeedFiles
{
    public class PatientTestsResultsImportTypeSeed
    {
        public static void CRPSeed(AspergillosisContext context)
        {
            var importer = context.DBImportTypes.Where(dbit => dbit.ImporterClass == "EPRCReactiveProteinImporter").FirstOrDefault();
            if (importer == null)
            {
                var dbImportType = new DbImportType { Name = "EPR CRP Report", ImporterClass = "EPRCReactiveProteinImporter" };
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
