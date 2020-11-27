using System;
using System.ComponentModel.DataAnnotations;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientTestResult
    {
        public int ID { get; set; }
        public int TestTypeId { get; set; }
        public int PatientId { get; set; }
        public int UnitOfMeasurementId { get; set; }
        public decimal Value { get; set; }
        public string Range { get; set; }
        public string SampleId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateTaken { get; set; }

        public TestType TestType { get; set; }
        public Patient Patient { get; set; }
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
        public decimal SourceSystemGUID { get; internal set; }
        public DateTime? CreatedDate { get; set; }

        public string ResultWithUnit()
        {
            return Value + " " + UnitOfMeasurement.Name;
        }
    }
}
