using AspergillosisEPR.Data;
using AspergillosisEPR.Lib;
using AspergillosisEPR.Lib.Importers.ManARTS;
using AspergillosisEPR.Lib.Search;
using AspergillosisEPR.Models.CaseReportForms;
using AspergillosisEPR.Models.Patients;
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
        public string NhsNumber { get; set; }
        public string GenericNote { get; set; }
        public string PostCode { get; set; }
        public double DistanceFromWythenshawe { get; set; }

        public ICollection<PatientDiagnosis> PatientDiagnoses { get; set; }
        public ICollection<PatientDrug> PatientDrugs { get; set; }
        public ICollection<PatientSTGQuestionnaire> STGQuestionnaires { get; set; }
        public ICollection<PatientImmunoglobulin> PatientImmunoglobulines { get; set; }
        public ICollection<PatientRadiologyFinding> PatientRadiologyFindings { get; set; }
        public ICollection<PatientMeasurement> PatientMeasurements { get; set; }
        public ICollection<CaseReportFormResult> CaseReportFormResults { get; set; }
        public ICollection<PatientMedicalTrial> MedicalTrials { get; set; }
        public ICollection<PatientDrugLevel> DrugLevels { get; set; }
        public ICollection<PatientSurgery> PatientSurgeries { get; set; }
        public PatientSmokingDrinkingStatus PatientSmokingDrinkingStatus { get; set; }
        public ICollection<PatientAllergicIntoleranceItem> PatientAllergicIntoleranceItems { get; set; }
        public ICollection<PatientPulmonaryFunctionTest> PatientPulmonaryFunctionTests { get; set; }
        public PatientStatus PatientStatus { get; set; }
        public PatientNACDates PatientNACDates { get; set; }
        public ICollection<PatientHaematology> PatientHaematologies { get; set;}
           
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

        public string Initials()
        {
            return FirstName.Substring(0, 1) + LastName.Substring(0, 1);
        }

        public Position PatientPosition(AspergillosisContext context)
        {
            if (PostCode == null) return new Position();
            var outcode = PostCode.Split(" ")[0];           
            var postcode = context.UKOutwardCodes.FirstOrDefault(pc => pc.Code.Equals(outcode));
            if (postcode == null) return new Position();
            var position = new Position();
            position.Latitude = postcode.Latitude;
            position.Longitude = postcode.Longitude;
            return position;
        }

        public void SetDistanceFromWythenshawe(AspergillosisContext context)
        {
             
            var wythenshawePosition = UKOutwardCode.WythenshawePosition(context);
            var patientPosition = PatientPosition(context);
            if (patientPosition.Latitude == 0 || patientPosition.Longitude == 0) return;

            DistanceFromWythenshawe = new Haversine().Distance(wythenshawePosition, patientPosition, DistanceType.Miles);             
        }
    }
}