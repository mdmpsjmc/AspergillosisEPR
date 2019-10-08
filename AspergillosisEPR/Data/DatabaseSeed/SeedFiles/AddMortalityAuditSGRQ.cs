using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed.SeedFiles
{
  public class AddMortalityAuditSGRQ
  {
    public static void Seed(AspergillosisContext context)
    {
      var importer = context.DBImportTypes.Where(dbit => dbit.ImporterClass == "MortalityAuditDatesSGRQImporter").FirstOrDefault();
      if (importer == null)
      {
        var dbImportType = new DbImportType { Name = "Mortality Audit SGRQ Importer", ImporterClass = "MortalityAuditDatesSGRQImporter" };
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
