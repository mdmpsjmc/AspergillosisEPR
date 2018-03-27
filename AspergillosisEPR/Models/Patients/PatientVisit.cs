using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
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
        [NotMapped]
        public List<IGrouping<string, PatientExamination>> GroupedExaminations { get; internal set; }

    }
}
