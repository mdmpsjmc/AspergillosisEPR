using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed.SeedFiles
{
  public static class DateOfDiagnosisDbImportType
  {
    public static void Seed(AspergillosisContext context)
    {
      var importer = context.DBImportTypes.Where(dbit => dbit.ImporterClass == "DateOfCPADiagnosisExcelImporter").FirstOrDefault();
      if (importer == null)
      {
        var dbImportType = new DbImportType { Name = "Import Date of CPA Diagnosis for Mortality Audit", ImporterClass = "DateOfCPADiagnosisExcelImporter" };
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
