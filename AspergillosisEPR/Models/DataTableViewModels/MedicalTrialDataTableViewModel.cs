using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.DataTableViewModels
{
    public class MedicalTrialDataTableViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PrincipalInvestigator { get; set; }
        public string Number { get; set; }
        public string RandDNumber { get; set; }
        public string RECNumber { get; set; }
        public string TrialType { get; set; }
        public string TrialStatus { get; set; }
    }
}
