using AspergillosisEPR.Models.Radiology;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientRadiologyNote
    { 
        public int ID { get; set; }
        public int PatientId { get; set; }
        public int RadiologyTypeId { get; set; }
        [Column(TypeName = "text")]
        public string Note { get; set; }
        public DateTime DateTaken { get; set; }
        public decimal SourceSystemGUID { get; set; }
        public DateTime? CreatedDate { get; set; }

        public RadiologyType RadiologyType { get; set; }
    }
}