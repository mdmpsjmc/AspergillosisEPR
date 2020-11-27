using AspergillosisEPR.Models.Radiology;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientRadiologyNote
    { 
        public int ID { get; set; }
        public int PatientId { get; set; }
        public int RadiologyTypeId { get; set; }
        [Column(TypeName = "text")]
        public string Note { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateTaken { get; set; }
        public decimal SourceSystemGUID { get; set; }
        public DateTime? CreatedDate { get; set; }

        public RadiologyType RadiologyType { get; set; }
    }
}