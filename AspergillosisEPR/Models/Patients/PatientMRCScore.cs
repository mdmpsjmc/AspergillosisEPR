using System;
using System.ComponentModel.DataAnnotations;
using DataType = System.ComponentModel.DataAnnotations.DataType;


namespace AspergillosisEPR.Models.Patients
{
    public class PatientMRCScore
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateTaken { get; set; }
        public string Score { get; set; }
    }
}
