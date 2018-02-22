using AspergillosisEPR.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
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

        public static PatientExamination BuildPatientExamination(string klass, string propertyName, string propertyValueId, 
                                                                  PatientVisit patientVisit)
        {
            Type examinationType = Type.GetType("AspergillosisEPR.Models." + klass);
            var examination = (PatientExamination)Activator.CreateInstance(examinationType);
            examination.PatientVisit = patientVisit;
            PropertyInfo property = examination.GetType().GetProperty(propertyName);
            property.SetValue(examination, Int32.Parse(propertyValueId));
            examination.PatientVisitId = patientVisit.ID;
            return examination;
        }

        public static List<dynamic> EntityTypes(AspergillosisContext context, string klassName)
        {
            var types = new Dictionary<string, List<dynamic>>()
            {
                {"SGRQExamination", context.PatientSTGQuestionnaires.ToList<dynamic>() },
                {"ImmunologyExamination", context.PatientImmunoglobulins.ToList<dynamic>()},
                {"MeasurementExamination", context.PatientMeasurements.ToList<dynamic>()},
                {"RadiologyExamination", context.PatientRadiologyFindings.ToList<dynamic>()},
            };
            return types[klassName];
        }
    }
}
