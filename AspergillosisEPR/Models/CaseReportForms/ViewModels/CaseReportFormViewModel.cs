using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.CaseReportForms.ViewModels
{
    public class CaseReportFormViewModel
    {
        [Required]
        [Display(Name = "Form Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Form Category")]
        public int CaseReportFormCategoryId { get; set; }

        public List<CaseReportFormFormSection> Sections { get; set; }
        public List<CaseReportFormField> Fields { get; set; }

        public List<int> SectionsIds { get; set; }

        public string CategoryName { get; set; }
        public List<string> SectionsNames { get; set; }
        public List<string> FieldsNames { get; set; }
        public string ItemId { get; set; }
    }
}
