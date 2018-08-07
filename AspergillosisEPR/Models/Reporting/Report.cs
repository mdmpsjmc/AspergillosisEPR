using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Reporting
{
    public class Report
    {
        public int ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ReportTypeId { get; set; }
        public ReportType ReportType { get; set; }

        public ICollection<PatientReportItem> PatientReportItems { get; set; }
    }
}
