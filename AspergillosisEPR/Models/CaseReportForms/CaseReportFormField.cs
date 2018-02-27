using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.CaseReportForms
{
    public class CaseReportFormField
    {
        public int ID { get; set; }
        public int CaseReportFormFieldTypeId { get; set; }
        public string Label { get; set; }

        public CaseReportFormFieldType CaseReportFormFieldType { get; set; }
        [NotMapped]
        public List<int> SelectedOptionsIds { get; set; }
    }
}
