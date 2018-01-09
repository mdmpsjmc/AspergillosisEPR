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
                { "Diagnosis", "DiagnosisType" },
                { "Drug", "Drug" },
                { "SGRQ", "PatientSTGQuestionnaire" }
            };
        }

        public static Dictionary<string, string> CriteriaMatches()
        {
            return new Dictionary<string, string>()
            {
                { "Exact", "Exact" },
                { "Starts With", "StartsWith" },
                { "Ends With", "EndsWith" },
                { "Contains", "Contains" },
                { "Greater Than", "GrearterThan"},
                { "Smaller Than", "SmallerThan" }
            };
        }
    }
}