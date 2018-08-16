using AspergillosisEPR.Lib.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Reporting
{
    public class Report : IValidatableObject
    {
        public int ID { get; set; }
        [NonEmptyDate]
        [Required]
        public DateTime StartDate { get; set; }
        [NonEmptyDate]
        [Required]
        public DateTime EndDate { get; set; }
        public int ReportTypeId { get; set; }
        public ReportType ReportType { get; set; }

        public ICollection<PatientReportItem> PatientReportItems { get; set; }
        [NotMapped]
        public string PatientIds { get; set; }
        

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(PatientIds))
            {
                yield return new ValidationResult(
                    $"You need to add at least one patient to this report",
                    new[] { "Base" });
            }

            if (StartDate == null)
            {
                yield return new ValidationResult(
                   $"Both Dates have to be present",
                   new[] { "StartDate" });
            }

            if (EndDate == null)
            {
                yield return new ValidationResult(
                   $"Both Dates have to be present",
                   new[] { "EndDate" });
            }

            if ((StartDate != null) && (EndDate != null) && (DateTime.Compare(StartDate, EndDate) >0))
            {
                yield return new ValidationResult(
                   $"End Date has to be later than Start Date",
                   new[] { "EndDate" });
            }
        }

       
    }
}
