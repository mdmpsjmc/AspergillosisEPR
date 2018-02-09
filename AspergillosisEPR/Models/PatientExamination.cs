using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public abstract class PatientExamination
    {
        public int ID { get; set; }
        public int PatientVisitId { get; set; }
        //public string Discriminator { get; set; }
        public PatientVisit PatientVisit { get; set; }
        public PatientSTGQuestionnaire PatientSTGQuestionnaire { get; set; }
        public PatientRadiologyFinding PatientRadiologyFinding { get; set; }
        public PatientImmunoglobulin PatientImmunoglobulin { get; set; }

    }
}
