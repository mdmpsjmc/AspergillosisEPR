
namespace AspergillosisEPR.Models
{
    public class Drug
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string KlassName => typeof(Drug).Name;

    }
}
