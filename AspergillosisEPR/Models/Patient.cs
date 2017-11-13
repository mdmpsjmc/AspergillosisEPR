using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace AspergillosisEPR.Models
{
    public class Patient
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyy}", ApplyFormatInEditMode = true)]
        public DateTime DOB { get; set; }
        public string RM2Number { get; set; }

        public ICollection<PatientDiagnosis> PatientDiagnoses { get; set; }
        public ICollection<PatientDrug> PatientDrugs { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return LastName + ", " + FirstName; }
        }

    }
}