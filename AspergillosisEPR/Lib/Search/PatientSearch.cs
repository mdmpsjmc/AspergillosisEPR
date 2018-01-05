using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Search
{
    public class PatientSearch
    {
        public static IEnumerable<string> CriteriaClasses()
        {
            return new List<string>()
            {
                "Patient", "Diagnosis Type", "Drug", "SGRQ"
            }.AsEnumerable();
        }

        public static IEnumerable<string> CriteriaMatches()
        {
            return new List<string>()
            {
                  "Exact",
                  "Starts With",
                  "Ends With",
                  "Contains", 
                  "Grearter Than",
                  "Smaller Than"
            }.AsEnumerable();
        }
    }
}