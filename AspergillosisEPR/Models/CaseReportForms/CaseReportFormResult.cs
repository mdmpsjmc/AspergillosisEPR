using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.CaseReportForms
{
    public class CaseReportFormResult
    {
        public int ID { get; set; }
        public int CaseReportFormResultCategoryId { get; set; }
        public string Name { get; set; }
        public IEnumerable<CaseReportFormSection> CaseReportFormResultSections { get; set; }

        public CaseReportFormCategory CaseReportFormCategory { get; set; }
    }
}
