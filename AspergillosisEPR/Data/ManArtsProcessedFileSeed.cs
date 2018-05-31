using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data
{
    public class ManArtsProcessedFileSeed
    {
        public static void Seed(AspergillosisContext context)
        {
            var importer = context.DBImportTypes.Where(dbit => dbit.ImporterClass == "ManArtsProcessedFileImporter").FirstOrDefault();
            if (importer == null)
            {
                var dbImportType = new DbImportType { Name = "ManArts Processed and Merged File (Mike Porter)", ImporterClass = "ManArtsProcessedFileImporter" };
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
