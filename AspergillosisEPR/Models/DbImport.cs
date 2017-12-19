using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace AspergillosisEPR.Models
{
    public class DbImport
    {
        public int ID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime ImportedDate { get; set; }
        public string  ImportedFileName { get; set; }
        public int PatientsCount { get; set; }
        public int DbImportTypeId { get; set;  }

        public static Hashtable HeadersDictionary()
        {
            return new Hashtable()
            {
                  { "SURNAME", "Patient.LastName" },
                  { "FORENAME", "Patient.FirstName" },
                  { "FIRST NAME", "Patient.FirstName"},
                  { "HOSPITAL No", "Patient.RM2Number" },
                  { "SEX", "Patient.Gender"},
                  { "Sex", "Patient.Gender"},
                  { "DOB", "Patient.DOB"},
                  { "Date of death", "Patient.DateOfDeath"},
                  { "HOSPITAL NUMBER", "Patient.RM2Number"},
                  { "CCPA", "PatientDiagnosis"},
                  { "ABPA", "PatientDiagnosis"},
                  { "SAFS", "PatientDiagnosis"},
                  { "OTHER","PatientDiagnosis"},
                  { "Underlying disease", "PatientDiagnosis" }
             };
        }

    }
}
