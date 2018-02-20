using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.PatientViewModels
{
    public class NewPatientVisitViewModel
    {
        public int PatientId { get; set; }
        public string VisitDate { get; set; }
        public ICollection<PatientSTGQuestionnaire> STGQuestionnaires { get; set; }
        public ICollection<PatientRadiologyFinding> PatientRadiologyFindings { get; set; }
        public ICollection<PatientImmunoglobulin> PatientImmunoglobulines { get; set; }
        public ICollection<PatientMeasurement> PatientMeasurements { get; set; }
        public Patient Patient { get; set; }

        public List<int> SelectedSGRQ { get; set; }
        public List<int> SelectedRadiololgy { get; set; }
        public List<int> SelectedIg { get; set; }
        public List<int> SelectedMeasurements { get; set; }

        public NewPatientVisitViewModel()
        {
            SelectedIg = new List<int>();
            SelectedRadiololgy = new List<int>();
            SelectedSGRQ = new List<int>();
            SelectedMeasurements = new List<int>();
        }

    }
}
