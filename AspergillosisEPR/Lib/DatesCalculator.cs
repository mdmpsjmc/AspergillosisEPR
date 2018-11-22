using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib
{
    public class DatesCalculator
    {
        private DateTime _startDate;
        private DateTime _endDate;

        public DatesCalculator(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _endDate = endDate;
        }

        public int Years()
        {
          var endDate = _endDate.Year;
          var endDateDayOfYear = _endDate.DayOfYear;
          int years = endDate - _startDate.Year;
          if (endDateDayOfYear < _startDate.DayOfYear) years = years - 1;
          return years;
        }
    }
}
