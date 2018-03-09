using AspergillosisEPR.Models.CaseReportForms.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.CaseReportForms
{
    public class CaseReportFormField
    {
        public int ID { get; set; }
        public int CaseReportFormFieldTypeId { get; set; }
        public int CaseReportFormSectionId { get; set; }
        [Required]
        [Display(Name = "Field label")]
        public string Label { get; set; }

        public CaseReportFormFieldType CaseReportFormFieldType { get; set; }
        public CaseReportFormSection CaseReportFormSection { get; set; }
        public ICollection<CaseReportFormFieldOption> Options { get; set; } 
        [NotMapped]
        public int[] SelectedOptionsIds { get; set; }
    }
}
