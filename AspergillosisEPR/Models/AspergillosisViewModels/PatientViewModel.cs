using AspergillosisEPR.Models.Patients;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspergillosisEPR.Models.AspergillosisViewModels
{
    public class PatientViewModel
    {
        public IEnumerable<PatientDiagnosis> Diagnoses { get; set; }
        public IEnumerable<PatientDrug> Drugs { get; set; }
        [Required]
        public Patient Patient { get; set; }
    }
}
