using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AspergillosisEPR.Models
{
    public class PatientDiagnosis
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int PatientId { get; set; }
        [Required]
        public int DiagnosisTypeId { get; set; }
        [Required]
        public int DiagnosisCategoryId { get; set; }
        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        public Patient Patient { get; set; }
        public DiagnosisType DiagnosisType { get; set; }
        public DiagnosisCategory DiagnosisCategory { get; set; }
    }
}