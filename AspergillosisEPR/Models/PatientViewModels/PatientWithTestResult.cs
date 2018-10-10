using AspergillosisEPR.Models.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.PatientViewModels
{
    public class PatientWithTestResult
    {
        public Patient Patient { get; set; }
        public List<PatientTestResult> PatientTestResults { get; set; } = new List<PatientTestResult>();
        public TestType TestType  { get; set;}
    }
}
