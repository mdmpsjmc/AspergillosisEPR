using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.PatientViewModels.Anonymous
{
    public class AnonymousPatientDataTableViewModel
    {
        public int ID { get; set; }
        public string PrimaryDiagnosis { get; set; }
        public string Initials { get; set; }       
    }
}
