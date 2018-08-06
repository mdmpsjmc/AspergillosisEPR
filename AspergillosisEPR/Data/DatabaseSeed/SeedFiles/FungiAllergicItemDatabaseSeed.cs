using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed.SeedFiles
{
    public static class FungiAllergicItemDatabaseSeed
    {
        public static void SeedDefaultItems(AspergillosisContext context)
        {
            if (context.Fungis.Any())
            {
                return;
            }

            var allergyItems = new Fungi[]
            {
                new Fungi { Name = "Candida albicans"},
                new Fungi { Name = "Aspergillus fumigatus"},
                new Fungi { Name = "Aspergillus niger"},
                new Fungi { Name = "Aspergillus flavus"},
                new Fungi { Name = "Aspergillus flavescens"},
                new Fungi { Name = "Aspergillus nidulans"},
                new Fungi { Name = "Aspergillus terreus"},
                new Fungi { Name = "Altenaria"}
            };

            foreach (var item in allergyItems)
            {
                context.Fungis.Add(item);
            }
            context.SaveChanges();
        }
    }
}
