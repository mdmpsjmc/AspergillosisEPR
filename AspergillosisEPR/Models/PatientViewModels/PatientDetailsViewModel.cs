using AspergillosisEPR.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.PatientViewModels
{
    public class PatientDetailsViewModel
    {
        public ICollection<PatientDiagnosis> PrimaryDiagnoses { get; set; }
        public ICollection<PatientDiagnosis> SecondaryDiagnoses { get; set; }
        public Patient Patient { get; set; }
        public ICollection<PatientDiagnosis> UnderlyingDiseases { get; set; }
        public ICollection<PatientDiagnosis> OtherDiagnoses { get; set; }
        public ICollection<PatientDiagnosis> PastDiagnoses { get; set; }
        public ICollection<PatientDrug> PatientDrugs { get; set; }
        public ICollection<PatientSTGQuestionnaire> STGQuestionnaires { get; set; }
        public ICollection<PatientImmunoglobulin> PatientImmunoglobulines { get; set; }
        public ICollection<PatientRadiologyFinding> PatientRadiologyFindings { get; set; }

        public bool ShowDiagnoses { get; set; }
        public bool ShowDrugs { get; set; }
        public bool ShowSGRQ { get; set; }
        public bool ShowIg { get; set; }
        public bool ShowButtons { get; set; }
        public string SgrqImageChartFile { get; set; }

        public PatientDetailsViewModel()
        {
            ShowDiagnoses = true;
            ShowDrugs = true;
            ShowSGRQ = true;
            ShowIg = true;
            ShowButtons = true;
        }

        public static PatientDetailsViewModel BuildPatientViewModel(AspergillosisContext context, Patient patient)
        {
            var primaryDiagnosis = context.DiagnosisCategories.Where(dc => dc.CategoryName == "Primary").FirstOrDefault();
            var secondaryDiagnosis = context.DiagnosisCategories.Where(dc => dc.CategoryName == "Secondary").FirstOrDefault();
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
            if (secondaryDiagnosis != null)
            {
                patientDetailsViewModel.SecondaryDiagnoses = patient.PatientDiagnoses.
                                                                    Where(pd => pd.DiagnosisCategoryId == secondaryDiagnosis.ID).
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
            patientDetailsViewModel.STGQuestionnaires = patient.STGQuestionnaires;
            patientDetailsViewModel.PatientImmunoglobulines = patient.PatientImmunoglobulines;
            patientDetailsViewModel.PatientRadiologyFindings = patient.PatientRadiologyFindings;
            return patientDetailsViewModel;
        }

        public bool HasPrimaryDiagnoses()
        {
            return PrimaryDiagnoses != null && PrimaryDiagnoses.Any();
        }

        public bool HasSecondaryDiagnoses()
        {
            return SecondaryDiagnoses != null && SecondaryDiagnoses.Any();
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
    }
}
