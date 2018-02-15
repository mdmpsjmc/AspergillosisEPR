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
        public List<dynamic> PatientExaminations { get; set; }
        public bool ShowButtons { get; set; }

        public PatientVisitDetailsViewModel() {
            ShowButtons = true;
        }

    }
}
