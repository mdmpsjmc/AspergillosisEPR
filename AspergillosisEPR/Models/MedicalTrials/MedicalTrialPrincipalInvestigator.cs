using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.MedicalTrials
{
    public class MedicalTrialPrincipalInvestigator
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PersonTitleId { get; set; }

        public PersonTitle PersonTitle { get; set; }
    }
}
