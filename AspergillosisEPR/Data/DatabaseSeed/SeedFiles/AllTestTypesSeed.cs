using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed.SeedFiles
{
    public class AllTestTypesSeed
    {
        public static void Seed(AspergillosisContext context)
        {
            var test = context.TestTypes.Where(tt => tt.Code == "CREA").FirstOrDefault();
            if (test != null) return;
            if (test == null)
            {
                foreach (var testType in TestType.Codes())
                {
                    var code = testType.Key;
                    var name = testType.Value;
                    var existingTest = context.TestTypes.Where(n => n.Name == name).FirstOrDefault();
                    if (existingTest == null) existingTest = new TestType();
                    existingTest.Name = name;
                    string unitName = TestType.Units()[code];
                    var unit = context.UnitOfMeasurements.Where(u => u.Name == unitName).FirstOrDefault();
                    if (unit == null)
                    {
                        unit = new UnitOfMeasurement()
                        {
                            Name = unitName
                        };
                        context.UnitOfMeasurements.Add(unit);
                        context.SaveChanges();
                    }
                    existingTest.Code = code;
                    existingTest.UnitOfMeasurementId = unit.ID;
                    context.TestTypes.Update(existingTest);
                }
                context.SaveChanges();
            }

        }
    }
}
