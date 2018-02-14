using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class PatientVisit
    {
        public int ID { get; set; }
        public DateTime VisitDate { get; set; }
        public int PatientId { get; set; }

        public Patient Patient { get; set; }

        public ICollection<PatientMeasurement> PatientExaminations;
        public ICollection<SGRQExamination> SGRQExaminations;
        public ICollection<RadiologyExamination> RadiologyExaminations;
        public ICollection<ImmunologyExamination> IgExaminations;
        public ICollection<MeasurementExamination> MeasurementExaminations;
    }
}
