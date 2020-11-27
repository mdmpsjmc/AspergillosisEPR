using AspergillosisEPR.Lib.Exporters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AspergillosisEPR.Models.CaseReportForms
{
    public class CaseReportFormResult : Exportable
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public int CaseReportFormId { get; set; }
        public int CaseReportFormCategoryId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateTaken { get; set; }

        public CaseReportForm Form {get; set;}
        public Patient Patient { get; set; }
        public CaseReportFormCategory Category { get; set; }
        public ICollection<CaseReportFormPatientResult> Results { get; set; }


        override public List<string> ExcludedProperties()
        {
            return new List<string>()
            {
                "PatientId", "CaseReportFormId", "CaseReportFormCategoryId"
            };
        }
    }
}
