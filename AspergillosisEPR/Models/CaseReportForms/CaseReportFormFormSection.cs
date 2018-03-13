using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.CaseReportForms
{
    public class CaseReportFormFormSection
    {
        public int ID { get; set; }
        public int CaseReportFormId { get; set; }
        public int CaseReportFormSectionId { get; set; }

        public CaseReportFormSection Section { get; set; }
    }
}
