
using AspergillosisEPR.Lib.Search;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AspergillosisEPR.Models
{
    public class Drug 
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Drug Name is required")]
        public string Name { get; set; }
        public string KlassName => typeof(Drug).Name;      
    }
}