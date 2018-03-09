using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.CaseReportForms
{
    public class CaseReportFormFieldType
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Field Type Name")]
        public string Name { get; set; }
    }
}
