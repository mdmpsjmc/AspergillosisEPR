using AspergillosisEPR.Lib.Exporters;
using AspergillosisEPR.Lib.Search;
using AspergillosisEPR.Models.MedicalTrials;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientMedicalTrial : Exportable, ISearchable
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public int MedicalTrialId { get; set; }
        public int PatientMedicalTrialStatusId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime IdentifiedDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ConsentedDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
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

        public Dictionary<string, string> SearchableFields()
        {
            return new Dictionary<string, string>()
            {
                { "Medical Trial", "PatientMedicalTrial.MedicalTrialId.Select"},
                { "Medical Trial Status", "PatientMedicalTrial.PatientMedicalTrialStatusId.Select"}            
            };
        }
    }
}
