using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.CaseReportForms
{
    public class CaseReportFormOptionChoice
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int CaseReportFormOptionGroupId { get; set; }
        
        public CaseReportFormOptionGroup CaseReportFormOptionGroup { get; set; }
    }
}
