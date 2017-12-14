using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.PatientViewModels
{
    public class PatientDataTableViewModel
    {
        public int ID {get; set;}
        public string PrimaryDiagnosis { get; set; }
        public string RM2Number { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }

    }
}
