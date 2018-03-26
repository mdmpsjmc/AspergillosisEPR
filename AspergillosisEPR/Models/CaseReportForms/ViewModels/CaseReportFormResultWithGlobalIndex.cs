using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.CaseReportForms.ViewModels
{
    public class CaseReportFormResultWithGlobalIndex
    {
        public int GlobalIndex { get; set; }
        public CaseReportFormResult Result {get; set;}
        public Patient Patient { get; set; }
        public bool ShowButtons { get; set; }

        public CaseReportFormResultWithGlobalIndex()
        {
            ShowButtons = true;
        }
     }
}
