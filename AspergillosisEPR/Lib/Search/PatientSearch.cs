using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Search
{
    public class PatientSearch
    {
        public static Dictionary<string, string> CriteriaClasses()
        {
            return new Dictionary<string, string>()
            {
                { "Patient", "Patient" },
                { "Diagnosis", "PatientDiagnosis" },
                { "Drug", "PatientDrug" },
                { "Radiology", "PatientRadiologyFinding" },
                { "Medical Trial", "PatientMedicalTrial" }
            };
        }

        public static Dictionary<string, string> CriteriaMatches()
        {
            return new Dictionary<string, string>()
            {
                { "Exact", "Exact.Date.String" },
                { "Starts With", "StartsWith.String" },
                { "Ends With", "EndsWith.String" },
                { "Contains", "Contains.String" },
                { "Greater Than", "GreaterThan.Date"},
                { "Smaller Than", "SmallerThan.Date" }
            };
        }
    }
}