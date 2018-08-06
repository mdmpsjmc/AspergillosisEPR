
using AspergillosisEPR.Lib.Search;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AspergillosisEPR.Models
{
    public class Drug : AllergicIntoleranceItem
    {
        [Required(ErrorMessage = "Drug Name is required")]
        public new string Name { get; set; }
        public string KlassName => typeof(Drug).Name;      
    }
}