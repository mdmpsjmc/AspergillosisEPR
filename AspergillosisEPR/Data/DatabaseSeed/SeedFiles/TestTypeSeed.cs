using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed.SeedFiles
{
    public static class TestTypeSeed
    {
        public static void Seed(AspergillosisContext context)
        {
            if (context.TestTypes.Any()) return;

           var tests = new TestType[]
           {
                new TestType()
                {
                    Name="Albumin"

                },
                new TestType()
                {
                    Name="C-Reactive Protein (CRP)"
                }, 
                new TestType()
                {
                    Name = "Haemoglobin"
                },
                new TestType()
                {
                    Name = "WBC"
                }, 
                new TestType()
                {
                    Name = "Lymphocytes"
                }
           };
            foreach(var testType in tests)
            {
                context.TestTypes.Add(testType);
            }
            context.SaveChanges();
        }
    }
}
