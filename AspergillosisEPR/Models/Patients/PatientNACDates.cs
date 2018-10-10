using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientNACDates
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public DateTime? ProbableStartOfDisease { get; set; }
        public DateTime? DefiniteStartOfDisease { get; set; }
        public DateTime? DateOfDiagnosis { get; set; }
        public DateTime? LastObservationPoint { get; set; }
        public DateTime FirstSeenAtNAC { get; set; }
        public int? CPABand { get; set; }

        public Dictionary<string, string> SearchableFields()
        {
            return new Dictionary<string, string>()
            {//klass.Field.Select (if select present than its a dropdown)
                { "First Seen Date", "PatientNACDates.FirstSeenAtNAC.Date" },
                { "Last Observation Point Date", "PatientNACDates.LastObservationPoint.Date" }
            };
        }

    }
}
