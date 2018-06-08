using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed
{
    public class UnitOfMeasureMgLSeed
    {
        private static string UNIT_NAME = "mg/L";

        public static void Seed(AspergillosisContext context)
        {
            var importer = context.UnitOfMeasurements.Where(uom => uom.Name == UNIT_NAME).FirstOrDefault();
            if (importer == null)
            {
                var unit = new UnitOfMeasurement { Name = UNIT_NAME };
                context.UnitOfMeasurements.Add(unit);
                context.SaveChanges();
            }
            else
            {
                return;
            }
        }
    }
}
