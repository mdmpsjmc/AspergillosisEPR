using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.CaseReportForms
{
    public class CaseReportFormFieldOption
    {
        public int ID { get; set; }
        public int CaseReportFormFieldId {get; set;}
        public int CaseReportFormOptionChoiceId { get; set; }

        public CaseReportFormField Field { get; set; }
        public CaseReportFormOptionChoice Option { get; set; }
    }
}
