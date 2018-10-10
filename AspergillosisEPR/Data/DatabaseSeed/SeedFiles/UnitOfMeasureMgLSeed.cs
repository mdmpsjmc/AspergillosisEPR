using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed.SeedFiles
{
    public static class UnitOfMeasureMgLSeed
    {
        private static string UNIT_NAME = "mg/L";
        private static string GL_L = "g/l";
        private static string BLOOD = "x10^9/l";
        private static string IGE = "kU/L";

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

        public static void SeedOtherUnits(AspergillosisContext context)
        {
            var importer = context.UnitOfMeasurements.Where(uom => uom.Name == GL_L).FirstOrDefault();
            if (importer == null)
            {
                var unit = new UnitOfMeasurement { Name = GL_L };
                context.UnitOfMeasurements.Add(unit);
                context.SaveChanges();

                var unit2 = new UnitOfMeasurement { Name = BLOOD };
                context.UnitOfMeasurements.Add(unit2);
                context.SaveChanges();

                var unit3 = new UnitOfMeasurement { Name = IGE };
                context.UnitOfMeasurements.Add(unit3);
                context.SaveChanges();
            }
            else
            {
                return;
            }
        }
    }
}
