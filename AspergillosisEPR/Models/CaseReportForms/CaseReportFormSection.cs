using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.CaseReportForms
{
    public class CaseReportFormSection
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Section Name")]
        public string Name { get; set; }
        public ICollection<CaseReportFormField> CaseReportFormResultFields { get; set; }
    }
}
