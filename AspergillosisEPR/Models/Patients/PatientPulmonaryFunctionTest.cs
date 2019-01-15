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
        public DateTime? DateTaken { get; set; }
        public decimal ResultValue { get; set; }
        public decimal PredictedValue { get; set; }
        public PulmonaryFunctionTest PulmonaryFunctionTest { get; set; }

        public Patient Patient { get; set; }
        public DateTime? CreatedDate { get; set; }
        public double? NormalValue { get; set; }
        public int? Age { get; set; }
        public int? Height { get; set; }
    }
}
