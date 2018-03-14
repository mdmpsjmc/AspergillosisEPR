using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.CaseReportForms
{
    public class CaseReportFormPatientResult
    {
        public int ID { get; set; }
        public int CaseReportFormFieldId { get; set; }
        public int CaseReportFormId { get; set; }    
        public int PatientId { get; set; }       

        public string TextAnswer { get; set; }
        public DateTime? DateAnswer { get; set; }
        public decimal? NumericAnswer { get; set; }
        public List<CaseReportFormPatientResultOptionChoice> Options { get; set; }    
        public CaseReportFormField Field { get; set; }
        public Patient Patient { get; set; }
        public CaseReportForm Form { get; set; }

    }
}
