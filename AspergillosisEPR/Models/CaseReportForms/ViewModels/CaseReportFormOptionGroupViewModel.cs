using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.CaseReportForms.ViewModels
{
    public class CaseReportFormOptionGroupViewModel
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Option Group Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "You need to add at least one option")]
        public string[] Options { get; set; }
        public string FormAction { get; set; }
    }
}
