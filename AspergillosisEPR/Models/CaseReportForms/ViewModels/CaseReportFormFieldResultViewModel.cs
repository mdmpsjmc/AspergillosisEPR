using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.CaseReportForms.ViewModels
{
    public class CaseReportFormFieldResultViewModel
    {
        public CaseReportFormPatientResult Result { get; set; }
        public CaseReportFormField Field { get; set; }
        public string Index { get; set; }
    }
}
