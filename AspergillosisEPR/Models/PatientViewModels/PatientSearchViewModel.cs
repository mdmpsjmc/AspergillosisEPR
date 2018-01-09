using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.PatientViewModels
{
    public class PatientSearchViewModel
    {
        public string SearchCriteria { get; set; }
        public string SearchClass { get; set; }
        public string Field { get; set; }
        public string SearchValue { get; set; }
        public string Index { get; set; }
        public string AndOr { get; set; }

        public PatientSearchViewModel() {
            AndOr = "AND";
            Index = "0";
        }
    }
}
