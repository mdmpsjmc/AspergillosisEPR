using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.MedicalTrials
{
    public class MedicalTrialPrincipalInvestigator
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public int PersonTitleId { get; set; }

        public PersonTitle PersonTitle { get; set; }

        public string Name()
        {
            return PersonTitle.Name + " " + FirstName + " " + LastName;
        }
    }
}
