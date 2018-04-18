using AspergillosisEPR.Lib.Exporters;
using AspergillosisEPR.Models.MedicalTrials;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientMedicalTrial : Exportable
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public int MedicalTrialId { get; set; }
        public int PatientMedicalTrialStatusId { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime IdentifiedDate { get; set; }
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime? ConsentedDate { get; set; }
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime? RecruitedDate { get; set; }
        public bool Consented { get; set; }

        public Patient Patient { get; set; }
        public MedicalTrial MedicalTrial { get; set; }
        
        public MedicalTrialPatientStatus PatientMedicalTrialStatus { get; set; }

        override public List<string> ExcludedProperties()
        {
            return new List<string>()
            {
                "PatientId", "Patient", "PatientMedicalTrialStatus", "MedicalTrial"
            };
        }
    }
}
