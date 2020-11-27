using System;
using System.ComponentModel.DataAnnotations;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientPulmonaryFunctionTest
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public int PulmonaryFunctionTestId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateTaken { get; set; }
        public decimal ResultValue { get; set; }
        public decimal PredictedValue { get; set; }
        public PulmonaryFunctionTest PulmonaryFunctionTest { get; set; }
        public string SourceInfo { get; set; }
        public Patient Patient { get; set; }
        public DateTime? CreatedDate { get; set; }
        public double? NormalValue { get; set; }
        public int? Age { get; set; }
        public int? Height { get; set; }
    }
}
