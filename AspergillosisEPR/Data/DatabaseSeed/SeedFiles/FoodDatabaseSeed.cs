using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed.SeedFiles
{
    public class FoodDatabaseSeed
    {
        public static void  SeedDefaultFoods(AspergillosisContext context)
        {
            if (context.Foods.Any())
            {
                return;
            }

            var foods = new Food[]
            {
                new Food { Name = "Milk"},
                new Food { Name = "Pork"},
                new Food { Name = "Eggs" },
                new Food { Name = "Nut"},
                new Food { Name = "Chestnut" },
                new Food { Name = "Multiple food allergies"}              
            };

            foreach (var food in foods)
            {
                context.Foods.Add(food);
            }
            context.SaveChanges();
        }
    }
}
