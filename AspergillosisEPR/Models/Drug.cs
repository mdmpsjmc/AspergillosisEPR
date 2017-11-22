
using System.ComponentModel.DataAnnotations;

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