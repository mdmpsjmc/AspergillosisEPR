using AspergillosisEPR.Data;
using AspergillosisEPR.Helpers;
using AspergillosisEPR.Lib;
using AspergillosisEPR.Models.Patients;
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

        public static async Task<NewPatientVisitViewModel> BuildPatientVisitVM(AspergillosisContext context,
                                                                        int patientId, object visitDate = null)
        {
            var patientManager = new PatientManager(context);
            var patient = await patientManager.FindPatientWithRelationsByIdAsync(patientId);
            if (patient == null)
            {
                return null;
            }
            var patientMeasurements = context.PatientMeasurements
                                              .Where(pm => pm.PatientId == patient.ID);

            var patientVM = new NewPatientVisitViewModel();
            if (visitDate != null)
            {
                DateTime patientVisitDate = (DateTime)visitDate;
                patientVM.VisitDate = DateHelper.DateTimeToUnixTimestamp(patientVisitDate).ToString();
            }
            patientVM.PatientId = patient.ID;
            patientVM.STGQuestionnaires = patient.STGQuestionnaires;
            patientVM.PatientRadiologyFindings = patient.PatientRadiologyFindings;
            patientVM.PatientImmunoglobulines = patient.PatientImmunoglobulines;
            if (patientMeasurements != null)
            {
                patientVM.PatientMeasurements = patientMeasurements.ToList();
            }
            return patientVM;
        }

    }
}
