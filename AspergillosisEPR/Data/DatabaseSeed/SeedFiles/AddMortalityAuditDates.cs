using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed.SeedFiles
{
  public class AddMortalityAuditDates
  {
    public static void Seed(AspergillosisContext context)
    {
      var importer = context.DBImportTypes.Where(dbit => dbit.ImporterClass == "MortalityAuditDatesSpreadsheetImporter").FirstOrDefault();
      if (importer == null)
      {
        var dbImportType = new DbImportType { Name = "Mortality Audit Dates", ImporterClass = "MortalityAuditDatesSpreadsheetImporter" };
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
