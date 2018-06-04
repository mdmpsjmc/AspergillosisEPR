using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientPulmonaryFunctionTest
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public int PulmonaryFunctionTestId { get; set; }
        public DateTime DateTaken { get; set; }
    }
}
