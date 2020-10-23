using AspergillosisEPR.Data;
using AspergillosisEPR.Lib;
using AspergillosisEPR.Lib.Geodesy;
using AspergillosisEPR.Lib.Importers.ManARTS;
using AspergillosisEPR.Lib.PostCodes;
using AspergillosisEPR.Lib.Search;
using AspergillosisEPR.Models.CaseReportForms;
using AspergillosisEPR.Models.Patients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestSharp;
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

        public string DistrictNumber { get; set; }
        public int? PatientStatusId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Death")]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime? DateOfDeath { get; set; }
        public string NhsNumber { get; set; }
        public string GenericNote { get; set; }
        [RegularExpression(@"^([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9]?[A-Za-z])))) [0-9][A-Za-z]{2})$", ErrorMessage = "Post Code is invalid")]
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
        public ICollection<PatientNACDates> PatientNACDates { get; set; } = new List<PatientNACDates>();
        public ICollection<PatientHaematology> PatientHaematologies { get; set;}
        public ICollection<PatientTestResult> PatientTestResults { get; set; }
        public ICollection<PatientMRCScore> PatientMRCScores { get; set; } = new List<PatientMRCScore>();
        public ICollection<PatientICD10Diagnosis> PatientICD10Diagnoses { get; set; } = new List<PatientICD10Diagnosis>();
        public ICollection<PatientRadiologyNote> PatientRadiologyNotes { get; set; } = new List<PatientRadiologyNote>();
        public ICollection<PatientHospitalAdmission> PatientHospitalAdmissions { get; set; } = new List<PatientHospitalAdmission>();
        public ICollection<CauseOfDeath> CausesOfDeaths { get; set; } = new List<CauseOfDeath>();

    [Display(Name = "Full Name")]
        public string FullName
        {
            get { return LastName + ", " + FirstName; }
        }
        public static readonly List<string> Genders = new List<string>() { "male", "female" };
        public static readonly List<string> CPABands = new List<string>() { "1", "2","3" };

        public int Age()
        {            
            if (IsAlive())
            {
                var ageCalculator = new DatesCalculator(DOB, DateTime.Now);
                return ageCalculator.Years();
            } else
            {
                var deathCalculator = new DatesCalculator(DOB, DateOfDeath.Value);
                return deathCalculator.Years();
            }                       
        }

        public string BucketDistance()
        {
            if (DistanceFromWythenshawe <= 25) return "0-25";
            if (DistanceFromWythenshawe > 25 && DistanceFromWythenshawe <= 75) return "25-75";
            if (DistanceFromWythenshawe > 75 && DistanceFromWythenshawe <= 150) return "75-150";
            if (DistanceFromWythenshawe > 150) return "150+";
            else
            {
                return "0";
            }
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
                { "Distance from hospital", "DistanceFromWythenshawe" },
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
            var dbPostCode = context.UKPostCodes.Where(pc => pc.Code.Equals(PostCode)).FirstOrDefault();
            if (dbPostCode == null) return new Position();
            var position = new Position();
            position.Latitude = Math.Round(Decimal.Parse(dbPostCode.Latitude), 6);
            position.Longitude = Math.Round(Decimal.Parse(dbPostCode.Longitude), 6);
            return position;
        }

        public void SetDistanceFromWythenshawe(AspergillosisContext context, ILogger logger)
        {             
            var patientPosition = PatientPosition(context);
            var distanceCalculator = new GraphhopperApi(new RestClient(GraphhopperApi.ENDPOINT), logger);
            var distance = distanceCalculator.RequestDistance(patientPosition);
            DistanceFromWythenshawe = (double) distance;
        }
    }
}