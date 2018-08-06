using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed.SeedFiles
{
    public class OtherAllergicItemDatabaseSeed
    {
        public static void SeedDefaultItems(AspergillosisContext context)
        {
            if (context.OtherAllergicItems.Any())
            {
                return;
            }

            var allergyItems = new OtherAllergicItem[]
            {
                new OtherAllergicItem { Name = "Cat"},
                new OtherAllergicItem { Name = "Dog"},
                new OtherAllergicItem { Name = "Pollen" },
                new OtherAllergicItem { Name = "House dust mite"},
                new OtherAllergicItem { Name = "Alcohol" },
                new OtherAllergicItem { Name = "Pet"},
                new OtherAllergicItem { Name = "Multiple Fungi"},
                new OtherAllergicItem { Name = "Latex"},
                new OtherAllergicItem { Name = "Metal"},
                new OtherAllergicItem { Name = "Grass"}
            };

            foreach (var item in allergyItems)
            {
                context.OtherAllergicItems.Add(item);
            }
            context.SaveChanges();
        }
    }
}
