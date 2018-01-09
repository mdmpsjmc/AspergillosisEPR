
using AspergillosisEPR.Lib.Search;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AspergillosisEPR.Models
{
    public class Drug : ISearchable
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Drug Name is required")]
        public string Name { get; set; }
        public string KlassName => typeof(Drug).Name;

        public Dictionary<string, string> SearchableFields()
        {
            return new Dictionary<string, string>()
            {
                { "Drug Name", "Drug.Name" },
                { "Start Date", "Drug.StartDate" },
                { "End Date", "Drug.EndDate" }
            };
        }
    }
}