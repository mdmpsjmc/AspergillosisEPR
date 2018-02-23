using AspergillosisEPR.Lib;
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
        public int VisitId { get; set; }
        public bool ShowOtherVisits { get; set; }

        public PatientVisitDetailsViewModel() {
            ShowButtons = true;
            ShowOtherVisits = true;
        }

        public static PatientVisitDetailsViewModel BuildPatientVisitDetailsVM(PatientVisitManager patientVisitManager, PatientVisit patientVisit)
        {
            var patientDetailsVM = new PatientVisitDetailsViewModel();
            patientVisit = patientVisitManager.GetPatientVisitById(patientVisit.ID);

            var patientExaminations = patientVisitManager.GetPatientExaminationsForVisitWithRelatedData(patientVisit.ID);
            var otherVisits = patientVisitManager.GetVisitsWithRelatedDataExcluding(patientVisit);

            patientDetailsVM.Patient = patientVisit.Patient;
            patientDetailsVM.VisitDate = patientVisit.VisitDate;
            patientDetailsVM.PatientExaminations = patientExaminations;
            patientDetailsVM.OtherVisits = otherVisits;
            patientDetailsVM.VisitId = patientVisit.ID;
            return patientDetailsVM;
        }
    }
}
