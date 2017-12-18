using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.PatientViewModels
{
    public class PatientDetailsViewModel
    {
        public ICollection<PatientDiagnosis> PrimaryDiagnoses { get; set; }
        public ICollection<PatientDiagnosis> SecondaryDiagnoses { get; set; }
        public Patient Patient { get; set; }
        public ICollection<PatientDiagnosis> UnderlyingDiseases { get; set; }
        public ICollection<PatientDiagnosis> OtherDiagnoses { get; set; }
        public ICollection<PatientDrug> PatientDrugs { get; set; }
        public ICollection<PatientSTGQuestionnaire> STGQuestionnaires { get; set; }

        public bool HasPrimaryDiagnoses()
        {
            return PrimaryDiagnoses != null && PrimaryDiagnoses.Any();
        }

        public bool HasSecondaryDiagnoses()
        {
            return SecondaryDiagnoses != null && SecondaryDiagnoses.Any();
        }

        public bool HasOtherDiagnoses()
        {
            return OtherDiagnoses != null && OtherDiagnoses.Any();
        }

        public bool HasUnderlyingDeases()
        {
            return UnderlyingDiseases != null && UnderlyingDiseases.Any();
        }
    }
}
