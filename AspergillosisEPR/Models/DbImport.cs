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


        public static Hashtable HeadersDictionary()
        {
            return new Hashtable()
            {
                  { "SURNAME", "Patient.FirstName" },
                  { "FORENAME", "Patient.LastName" },
                  { "HOSPITAL No", "Patient.RM2Number" },
                  { "SEX", "Patient.Gender"},
                  { "DOB", "Patient.DOB"},
                  { "Date of death", "Patient.DateOfDeath|Patient.Status"},
                  { "CCPA", "DiagnosisType.ID|DiagnosisCategory.ID"},
                  { "ABPA", "DiagnosisType.ID|DiagnosisCategory.ID"},
                  { "SAFS", "DiagnosisType.ID|DiagnosisCategory.ID"},
                  { "Other","DiagnosisType.ID|DiagnosisCategory.ID"},
                  { "Underlying disease", "DiagnosisType.ID|DiagnosisCategory.ID" }
             };
        }
    }
}
