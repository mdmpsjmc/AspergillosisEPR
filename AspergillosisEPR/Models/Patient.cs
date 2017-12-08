using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace AspergillosisEPR.Models
{
    public class Patient
    {

        public int ID { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }       
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime DOB { get; set; }

        [Required]
        [Display(Name = "RM2 Number")]
        [StringLength(50)]
        [Remote("HasRMNumber", "Patients", AdditionalFields = "Id",
                ErrorMessage = "Patient RM2 Number already exists in database")]
        public string RM2Number { get; set; }

        public int? PatientStatusId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Death")]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime? DateOfDeath { get; set; }

        public ICollection<PatientDiagnosis> PatientDiagnoses { get; set; }
        public ICollection<PatientDrug> PatientDrugs { get; set; }
        public PatientStatus PatientStatus { get; set; }
        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return LastName + ", " + FirstName; }
        }
        public static readonly List<string> Genders = new List<string>() { "male", "female" };

        public int Age()
        {
            int age = 0;
            int endDate = 0;
            int endDateDayOfYear = 0;
            if (IsAlive())
            {
                endDate = DateTime.Now.Year;
                endDateDayOfYear = DateTime.Now.DayOfYear;
            } else
            {
                endDate = DateOfDeath.Value.Year;
                endDateDayOfYear = DateOfDeath.Value.DayOfYear;
            }
            age = endDate  - DOB.Year;
            if (endDateDayOfYear < DOB.DayOfYear)
                age = age - 1;

            return age;
        }

        public bool IsDeceased()
        {
            return !IsAlive();
        }

        private bool IsAlive()
        {
            return DateOfDeath == null; 
        }

    }
}