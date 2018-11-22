using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientICD10Diagnosis
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public string DiagnosisCode { get; set; }
        [Column(TypeName = "text")]
        public string DiagnosisDescription { get; set; }
        public long OriginalImportId { get; set; }        
    }
}
