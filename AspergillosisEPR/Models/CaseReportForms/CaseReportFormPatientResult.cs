using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.CaseReportForms
{
    public class CaseReportFormPatientResult : IValidatableObject
    {
        public int ID { get; set; }
        public int CaseReportFormFieldId { get; set; }
        public int CaseReportFormId { get; set; }    
        public int CaseReportFormResultId { get; set; }

        public int PatientId { get; set; }       

        public string TextAnswer { get; set; }
        public DateTime? DateAnswer { get; set; }
        public decimal? NumericAnswer { get; set; }

        [NotMapped]
        public int[] SelectedIds { get; set; }    

        [NotMapped]
        public int? SelectedId { get; set; }
        public List<CaseReportFormPatientResultOptionChoice> Options { get; set; }   
        
        public CaseReportFormField Field { get; set; }
        public Patient Patient { get; set; }
        public CaseReportForm FormResult { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            CaseReportFormPatientResult form = (CaseReportFormPatientResult) validationContext.ObjectInstance;
            if (string.IsNullOrEmpty(TextAnswer) 
                    && NumericAnswer == null 
                            && DateAnswer == null
                                 && SelectedId == null
                                     && (SelectedIds == null || SelectedIds.Length == 0))
            {
                yield return new ValidationResult(
                    $"You need to provide result/value for this field",
                    new[] { "ValidationResult" });
            }           
        }
    }
}
