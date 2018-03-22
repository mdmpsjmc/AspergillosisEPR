using AspergillosisEPR.Data;
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

        public static CaseReportFormPatientResultOptionChoice CreateOptionChoiceForResult(AspergillosisContext context, 
                                                                                          int? optionChoiceId)
        {
            var optionChoice = context.CaseReportFormOptionChoices
                                      .Where(oc => oc.ID == optionChoiceId)
                                      .FirstOrDefault();
            if (optionChoice == null) return null;
            var caseReportResultOptionChoice = new CaseReportFormPatientResultOptionChoice();
            caseReportResultOptionChoice.CaseReportFormOptionChoiceId = optionChoice.ID;
            return caseReportResultOptionChoice;
        }
    }

  
}
