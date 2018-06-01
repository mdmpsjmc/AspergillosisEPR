using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Importers.ManARTS
{
    public  static class DiagnosisHeaders
    {
        public static Hashtable Dictionary()
        {
           return new Hashtable(){
                  { "Forename", "Patient.FirstName" },
                  { "Surename", "Patient.LastName"},
                  { "DOB", "Patient.DOB" },
                  { "RM2", "Patient.RM2Number" },
                  { "DEATH_TIME", "Patient.DateOfDeath" },
                  { "DeathIndicator", "Patient.PatientStatusId"},
                  { "Gender", "Patient.Gender" },
                  { "NhsNumber", "Patient.NhsNumber"},
                  { "HasCPA", "PatientDiagnosis.DiagnosisTypeId" },
                  { "CPAType", "PatientDiagnosis.DiagnosisTypeId"},
                  { "CPANotes", "Patient.DiagnosisTypeId"},
                  { "CPAYear", "PatientDiagnosis.DiagnosisTypeId"},
                  { "CCPAPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasAspergilloma", "PatientDiagnosis.DiagnosisTypeId"},
                  { "AspergillomaYear", "PatientDiagnosis.DiagnosisTypeId"},
                  { "AspergillomaPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasABPA", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasAsthma", "PatientDiagnosis.DiagnosisTypeId"},
                  { "AsthmaNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "AsthmaYear", "PatientDiagnosis.DiagnosisTypeId"},
                  { "AsthmaPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasEmphysema", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasCOPD", "PatientDiagnosis.DiagnosisTypeId"},
                  { "COPDNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "COPDYear",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "COPDPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasPneumonia", "PatientDiagnosis.DiagnosisTypeId" },
                  { "HasFungalLungDisease", "PatientDiagnosis.DiagnosisTypeId" },
                  { "FungalLungDiseaseNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "FungalLungDiseaseYear",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "FungalLungDiseaseYearPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasChronicCough", "PatientDiagnosis.DiagnosisTypeId" },
                  { "ChronicCoughNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "ChronicCoughYear",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "ChronicCoughPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasTB", "PatientDiagnosis.DiagnosisTypeId" },
                  { "IsTBPrevious", "PatientDiagnosis.DiagnosisTypeId" },
                  { "PreviousTBNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "PreviousTBYear",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "PreviousTBYearPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasBronchiectasis", "PatientDiagnosis.DiagnosisTypeId" },
                  { "BronchiectasisNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "BronchiectasisYear",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "BronchiectasisPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasSarcoidosis", "PatientDiagnosis.DiagnosisTypeId" },
                  { "SarcoidosisNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "SarcoidosisYear",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "SarcoidosisPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasHypertension", "PatientDiagnosis.DiagnosisTypeId" },
                  { "HypertensionPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasCancer", "PatientDiagnosis.DiagnosisTypeId" },
                  { "CancerNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "CancerYear",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "CancerPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasLungSurgery", "PatientDiagnosis.DiagnosisTypeId" },
                  { "LungSurgeryNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "LungSurgeryCancerYear",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "LungSurgeryPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasDiabetes", "PatientDiagnosis.DiagnosisTypeId" },
                  { "DiabetesNotes", "PatientDiagnosis.DiagnosisTypeId"},
                  { "DiabetesYear",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "DiabetesPrimarySecondary", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasHeartDisease", "PatientDiagnosis.DiagnosisTypeId"},
                  { "HasOsteoporosis",  "PatientDiagnosis.DiagnosisTypeId" },
                  { "OtherDiagnosisAndNotes", "PatientDiagnosis.DiagnosisTypeId"},
            };
        }
    }
}
