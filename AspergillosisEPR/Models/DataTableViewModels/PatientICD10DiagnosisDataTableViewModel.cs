using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.DataTableViewModels
{
    public class PatientICD10DiagnosisDataTableViewModel
    {
        public int ID { get; set; }        
        public double DiagnosisDate { get; set; }
        public string DiagnosisCode { get; set; }
        public string DiagnosisDescription { get; set; }
    }
}
