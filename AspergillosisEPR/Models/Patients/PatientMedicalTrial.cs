using AspergillosisEPR.Models.MedicalTrials;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientMedicalTrial
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public int MedicalTrialId { get; set; }
        public int PatientMedicalTrialStatusId { get; set; }
        [Required]
        public DateTime IdentifiedDate { get; set; }
        public DateTime? ConsentedDate { get; set; }
        public DateTime? RecruitedDate { get; set; }
        public bool Consented { get; set; }

        public Patient Patient { get; set; }
        public MedicalTrial MedicalTrial { get; set; }
    }
}
