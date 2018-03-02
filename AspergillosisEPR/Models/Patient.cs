using AspergillosisEPR.Lib.Search;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AspergillosisEPR.Models
{
    public class Patient : ISearchable
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime DOB { get; set; }

        [Required]
        [Display(Name = "RM2 Number")]
        [StringLength(50)]
        [Remote("HasRM2Number", "Patients", AdditionalFields = "ID, InitialRM2Number",
                ErrorMessage = "Patient RM2 Number already exists in database")]
        public string RM2Number { get; set; }

        public int? PatientStatusId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Death")]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime? DateOfDeath { get; set; }

        public ICollection<PatientDiagnosis> PatientDiagnoses { get; set; }
        public ICollection<PatientDrug> PatientDrugs { get; set; }
        public ICollection<PatientSTGQuestionnaire> STGQuestionnaires { get; set; }
        public ICollection<PatientImmunoglobulin> PatientImmunoglobulines { get; set; }
        public ICollection<PatientRadiologyFinding> PatientRadiologyFindings { get; set; }
        public ICollection<PatientMeasurement> PatientMeasurements { get; set; }

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

        public bool IsValid()
        {
            var context = new ValidationContext(this);
            var results = new List<ValidationResult>();
            return Validator.TryValidateObject(this, context, results);
        }

        private bool IsAlive()
        {
            return DateOfDeath == null; 
        }

        public Dictionary<string, string> SearchableFields()
        {
            return new Dictionary<string, string>()
            {
                { "First Name", "FirstName" },
                { "Last Name", "LastName" },
                { "RM2 Number", "RM2Number" },
                { "Date of Birth", "DOB" },
                { "Date of Death", "DateOfDeath" },
                { "Status", "PatientStatus.PatientStatusId.Select" }
            }; 
        }
    }
}