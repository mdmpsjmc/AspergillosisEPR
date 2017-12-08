using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspergillosisEPR.Models
{
    public class DiagnosisType
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string ShortName { get; set; }

        public ICollection<PatientDiagnosis> PatientDiagnoses { get; set; }

        public string KlassName => typeof(DiagnosisType).Name; 
    }
}