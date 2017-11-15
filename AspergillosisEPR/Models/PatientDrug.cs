using System;
using System.ComponentModel.DataAnnotations;

namespace AspergillosisEPR.Models
{
    public class PatientDrug
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public int DrugId { get; set; }

        public Patient Patient { get; set; }
        public Drug Drug { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
    }
}
