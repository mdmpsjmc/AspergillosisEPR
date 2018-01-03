using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Extensions;
namespace AspergillosisEPR.Extensions.Validations
{
    public static class ValidationExtensions
    {
        public static void CheckFieldUniqueness<T>(this Controller currentController, IEnumerable<T> source, string columnName, string propertyValue)
        {
            dynamic existingItem = source.Where(m => {
                return (m.GetType().GetProperty(columnName).
                          GetValue(m, null).ToString() == propertyValue);}).
                                         FirstOrDefault();
            if (existingItem != null)
            {
                string klassName = source.ToList().FirstOrDefault().GetType().Name.FirstCharacterToLower();
                currentController.ModelState.AddModelError(klassName + "."+ columnName, "Object with this name already exists in database");
            }
        }
    }
}
