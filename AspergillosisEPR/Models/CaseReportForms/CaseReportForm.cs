using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.CaseReportForms
{
    public class CaseReportForm
    {
        public int ID { get; set; }
        public int CaseReportFormCategoryId { get; set; }
        public string Name { get; set; }
        public IEnumerable<CaseReportFormField> Fields { get; set; }
        public IEnumerable<CaseReportFormFormSection> Sections { get; set; }
        public CaseReportFormCategory CaseReportFormCategory { get; set; }

       
    }
}
