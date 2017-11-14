using System.Collections.Generic;

namespace AspergillosisEPR.Models.AspergillosisViewModels
{
    public class PatientViewModel
    {
        public IEnumerable<PatientDiagnosis> Diagnoses { get; set; }
        public IEnumerable<PatientDrug> Drugs { get; set; }
        public Patient Patient { get; set; }
    }
}
