using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.CaseReportForms
{
    public class CaseReportFormPatientResultOptionChoice
    {
        public int ID { get; set; }
        public int CaseReportFormPatientResultId { get; set; }
        public int CaseReportFormOptionChoiceId { get; set; }
    }
}
