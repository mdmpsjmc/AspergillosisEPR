using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.CaseReportForms.ViewModels
{
    public class CaseReportFormFieldTypeViewModel
    {
        [Required]
        [Display(Name = "Field Type Name")]
        public string Name { get; set; }
        public string FormAction { get; set; }
    }
}
