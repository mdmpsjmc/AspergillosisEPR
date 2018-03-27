using System.ComponentModel.DataAnnotations;


namespace AspergillosisEPR.Models.Patients
{
    public class PatientStatus
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
