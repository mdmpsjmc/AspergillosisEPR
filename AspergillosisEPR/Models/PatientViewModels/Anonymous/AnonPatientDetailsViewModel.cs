using AspergillosisEPR.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.PatientViewModels.Anonymous
{
    public class AnonPatientDetailsViewModel : PatientDetailsViewModel
    {
        public int Identifier { get; set;  }
        public string Initials { get; set; }

        public static AnonPatientDetailsViewModel BuildAnonPatientViewModel(AspergillosisContext context, Patient patient)
        {
            var primaryDiagnosis = context.DiagnosisCategories.Where(dc => dc.CategoryName == "Primary").FirstOrDefault();
            var secondaryDiagnosis = context.DiagnosisCategories.Where(dc => dc.CategoryName == "Secondary").FirstOrDefault();
            var otherDiagnosis = context.DiagnosisCategories.Where(dc => dc.CategoryName == "Other").FirstOrDefault();
            var underlyingDiagnosis = context.DiagnosisCategories.Where(dc => dc.CategoryName == "Underlying diagnosis").FirstOrDefault();
            var pastDiagnosis = context.DiagnosisCategories.Where(dc => dc.CategoryName == "Past Diagnosis").FirstOrDefault();

            var patientDetailsViewModel = new AnonPatientDetailsViewModel();

            patientDetailsViewModel.Identifier = patient.ID;
            patientDetailsViewModel.Initials = patient.Initials();
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
            patientDetailsViewModel.PatientMeasurements = patient.PatientMeasurements.OrderByDescending(q => q.DateTaken).ToList();
            return patientDetailsViewModel;
        }
    }
}
