using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib
{
    public static class TimeDifference
    {
        public static int InWeeks(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null) return 0;
            int difference = ((startDate.Value - endDate.Value).Days) / 7;
            return difference;
        }
    }
}
