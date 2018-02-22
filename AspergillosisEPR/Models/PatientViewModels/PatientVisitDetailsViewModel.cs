using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.PatientViewModels
{
    public class PatientVisitDetailsViewModel
    {
        public Patient Patient { get; set; }
        public DateTime VisitDate { get; set; }
        public List<IGrouping<string, PatientExamination>> PatientExaminations { get; set; }
        public bool ShowButtons { get; set; }
        public List<PatientVisit> OtherVisits { get; set; }

        public PatientVisitDetailsViewModel() {
            ShowButtons = true;
        }
    }
}
