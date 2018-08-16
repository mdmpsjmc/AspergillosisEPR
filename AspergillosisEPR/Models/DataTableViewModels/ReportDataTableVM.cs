using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.DataTableViewModels
{
    public class ReportDataTableVM
    {
        public int ID { get; set; }
        public string ReportType { get; set; }
        public double StartDate { get; set; }
        public double EndDate { get; set; }
        public int Patients { get; set; }
    }
}
