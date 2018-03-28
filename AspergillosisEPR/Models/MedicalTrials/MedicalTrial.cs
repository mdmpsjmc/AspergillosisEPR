using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.MedicalTrials
{
    public class MedicalTrial
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Trial Name")]
        public string Name { get; set; }
        [Column(TypeName = "ntext")]
        public string Description { get; set; }
        public string Number { get; set; }
        public string RandDNumber { get; set; }
        public string RECNumber { get; set; }
        public int? MedicalTrialPrincipalInvestigatorId { get; set; }
        public int? MedicalTrialTypeId { get; set; }
        public int? MedicalTrialStatusId { get; set; }

        public MedicalTrialPrincipalInvestigator PrincipalInvestigator { get; set; }
        public MedicalTrialType TrialType { get; set; }
        public MedicalTrialStatus TrialStatus { get; set; }
    }
}
