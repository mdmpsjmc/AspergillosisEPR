using AspergillosisEPR.Data;
using AspergillosisEPR.Lib.CaseReportForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Models.CaseReportForms;
using AspergillosisEPR.Models.Patients;

namespace AspergillosisEPR.Models.PatientViewModels
{
    public class PatientDetailsViewModel
    {
        public ICollection<PatientDiagnosis> PrimaryDiagnoses { get; set; }
        public Patient Patient { get; set; }
        public ICollection<PatientDiagnosis> UnderlyingDiseases { get; set; }
        public ICollection<PatientDiagnosis> OtherDiagnoses { get; set; }
        public ICollection<PatientDiagnosis> PastDiagnoses { get; set; }
        public ICollection<PatientDrug> PatientDrugs { get; set; }
        public ICollection<PatientMedicalTrial> MedicalTrials { get; private set; }
        public ICollection<PatientSTGQuestionnaire> STGQuestionnaires { get; set; }
        public ICollection<PatientImmunoglobulin> PatientImmunoglobulines { get; set; }
        public ICollection<PatientRadiologyFinding> PatientRadiologyFindings { get; set; }
        public ICollection<PatientMeasurement> PatientMeasurements { get; set; }
        public List<IGrouping<string, CaseReportFormResult>> CaseReportForms { get; private set; }
        public List<PatientIgChart> IgCharts { get; set; }
        public ICollection<PatientDrugLevel> DrugLevels { get; set; }

        public bool ShowDiagnoses { get; set; }
        public bool ShowDrugs { get; set; }
        public bool ShowSGRQ { get; set; }
        public bool ShowIg { get; set; }
        public bool ShowButtons { get; set; }
        public bool ShowRadiology { get; set; }
        public bool ShowWeight { get; set; }
        public bool ShowDetails { get; set; }
        public bool ShowCaseReportForms { get; set; }
        public bool ShowTrials { get; set; }

        public string SgrqImageChartFile { get; set; }

        public PatientDetailsViewModel()
        {
            ShowDetails = true;
            ShowDiagnoses = true;
            ShowDrugs = true;
            ShowSGRQ = true;
            ShowIg = true;
            ShowRadiology = true;
            ShowButtons = true;
            ShowWeight = true;
            ShowCaseReportForms = true;
        }

        public static PatientDetailsViewModel BuildPatientViewModel(AspergillosisContext context, 
                                                                    Patient patient,
                                                                    CaseReportFormManager caseReportFormManager)
        {
            var primaryDiagnosis = context.DiagnosisCategories.Where(dc => dc.CategoryName == "Primary").FirstOrDefault();
            var otherDiagnosis = context.DiagnosisCategories.Where(dc => dc.CategoryName == "Other").FirstOrDefault();
            var underlyingDiagnosis = context.DiagnosisCategories.Where(dc => dc.CategoryName == "Underlying diagnosis").FirstOrDefault();
            var pastDiagnosis = context.DiagnosisCategories.Where(dc => dc.CategoryName == "Past Diagnosis").FirstOrDefault();

            var patientDetailsViewModel = new PatientDetailsViewModel();

            patientDetailsViewModel.Patient = patient;

            if (primaryDiagnosis != null)
            {
                patientDetailsViewModel.PrimaryDiagnoses = patient.PatientDiagnoses.
                                                                    Where(pd => pd.DiagnosisCategoryId == primaryDiagnosis.ID).
                                                                    ToList();
            }
          
            if (otherDiagnosis != null)
            {
                patientDetailsViewModel.OtherDiagnoses = patient.PatientDiagnoses.
                                                                 Where(pd => pd.DiagnosisCategoryId == otherDiagnosis.ID).
                                                                 ToList();
            }
            if (underlyingDiagnosis != null)
            {
                patientDetailsViewModel.UnderlyingDiseases = patient.PatientDiagnoses.
                                                                    Where(pd => pd.DiagnosisCategoryId == underlyingDiagnosis.ID).
                                                                    ToList();
            }

            if (pastDiagnosis != null)
            {
                patientDetailsViewModel.PastDiagnoses = patient.PatientDiagnoses.
                                                                    Where(pd => pd.DiagnosisCategoryId == pastDiagnosis.ID).
                                                                    ToList();
            }
            patientDetailsViewModel.PatientDrugs = patient.PatientDrugs;
            LoadReleatedMedicalTrials(context, patient);
            patientDetailsViewModel.MedicalTrials = patient.MedicalTrials;
            patientDetailsViewModel.STGQuestionnaires = patient.STGQuestionnaires;
            patientDetailsViewModel.PatientImmunoglobulines = patient.PatientImmunoglobulines;
            patientDetailsViewModel.PatientRadiologyFindings = patient.PatientRadiologyFindings;
            patientDetailsViewModel.PatientMeasurements = patient.PatientMeasurements.OrderByDescending(q => q.DateTaken).ToList();
            patientDetailsViewModel.DrugLevels = patient.DrugLevels.OrderByDescending(q => q.DateTaken).ToList();

            if (caseReportFormManager != null)
            {
                patientDetailsViewModel.CaseReportForms = caseReportFormManager.GetGroupedCaseReportFormsForPatient(patient.ID);
            }
            return patientDetailsViewModel;
        }

        public bool HasPrimaryDiagnoses()
        {
            return PrimaryDiagnoses != null && PrimaryDiagnoses.Any();
        }

        public bool HasOtherDiagnoses()
        {
            return OtherDiagnoses != null && OtherDiagnoses.Any();
        }

        public bool HasUnderlyingDeases()
        {
            return UnderlyingDiseases != null && UnderlyingDiseases.Any();
        }

        public bool HasPastDeseases()
        {
            return PastDiagnoses != null && PastDiagnoses.Any();
        }

        private static void LoadReleatedMedicalTrials(AspergillosisContext context, Patient patient)
        {
            context.Entry(patient).Collection(c => c.MedicalTrials).Load();
            foreach (var trial in patient.MedicalTrials)
            {
                context.Entry(trial).Reference(t => t.MedicalTrial).Load();
                context.Entry(trial).Reference(t => t.PatientMedicalTrialStatus).Load();
                var medicalTrial = trial.MedicalTrial;
                context.Entry(medicalTrial).Reference(t => t.TrialStatus).Load();
            }
        }

    }
}
