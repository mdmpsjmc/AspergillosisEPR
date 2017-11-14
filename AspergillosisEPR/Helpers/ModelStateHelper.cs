using System.Collections;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AspergillosisEPR.Helpers
{
    public static class ModelStateHelper
    {
        public static Hashtable Errors(ModelStateDictionary modelState)
        {
            var errors = new Hashtable();
            if (!modelState.IsValid)
                
                foreach (var pair in modelState)
                {
                    if (pair.Value.Errors.Count > 0)
                    {
                        errors[pair.Key] = pair.Value.Errors.Select(error => error.ErrorMessage).ToList();
                     }
                }
            return errors;
        }
    }
}
