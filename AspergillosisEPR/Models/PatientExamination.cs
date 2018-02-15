using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public abstract class PatientExamination
    {
        public int ID { get; set; }
        public int PatientVisitId { get; set; }
        public string Discriminator { get; set; }
        public PatientVisit PatientVisit { get; set; }
        public int PatientMeasurementId { get; set; }
        public int PatientSTGQuestionnaireId { get; set; }
        public int PatientRadiologyFinidingId { get; set; }
        public int PatientImmunoglobulinId { get; set; }
        public PatientMeasurement PatientMeasurement { get; set; }
        public PatientSTGQuestionnaire PatientSTGQuestionnaire { get; set; }
        [ForeignKey("PatientRadiologyFinidingId")]
        public PatientRadiologyFinding PatientRadiologyFiniding { get; set; }
        public PatientImmunoglobulin PatientImmunoglobulin { get; set; }
    }
}
