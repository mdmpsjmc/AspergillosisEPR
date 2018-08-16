using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Validations
{
    public class NonEmptyDateAttribute : ValidationAttribute
    {
        public NonEmptyDateAttribute()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dateValue = (DateTime) value;
            
            if (dateValue == null)
            {
                return new ValidationResult(GetNullErrorMessage());
            }
            if (dateValue.Year == 1)
            {
                return new ValidationResult(InvalidDate());
            }

            return ValidationResult.Success;
        }

        private string GetNullErrorMessage()
        {
            return "Date cannot be empty";
        }

        private string InvalidDate()
        {
            return "Date is Invalid";
        }
    }
}
