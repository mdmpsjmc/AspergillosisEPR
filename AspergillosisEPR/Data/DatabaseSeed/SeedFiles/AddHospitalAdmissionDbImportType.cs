using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed.SeedFiles
{
  public static class AddHospitalAdmissionDbImportType
  {
    public static void Seed(AspergillosisContext context)
    {
      var importer = context.DBImportTypes.Where(dbit => dbit.ImporterClass == "HospitalAdmissionsExcelImporter").FirstOrDefault();
      if (importer == null)
      {
        var dbImportType = new DbImportType { Name = "Import Mortality Audit Hospital Admissions", ImporterClass = "HospitalAdmissionsExcelImporter" };
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
