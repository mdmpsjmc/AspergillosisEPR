using System.ComponentModel.DataAnnotations;

namespace AspergillosisEPR.Models
{
    public class SideEffect
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Side Effect Name is required")]
        public string Name { get; set; }
        public string KlassName => typeof(SideEffect).Name;

    }
}
