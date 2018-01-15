using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class DiagnosisCategory
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        public string CategoryName { get; set; }
        [JsonIgnore]
        public ICollection<PatientDiagnosis> PatientDiagnoses { get; set; }
        public string KlassName => typeof(DiagnosisCategory).Name;

    }
}