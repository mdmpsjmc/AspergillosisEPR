using System.ComponentModel.DataAnnotations;


namespace AspergillosisEPR.Models
{
    public class PatientStatus
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
