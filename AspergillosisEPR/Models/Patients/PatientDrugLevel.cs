using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientDrugLevel
    {
        public int ID { get; set; }
        public int PatientId { get; set; }

        [Required]
        [Display(Name = "Drug")]
        public int DrugId { get; set; }

        [Required]
        [Display(Name = "Unit")]
        public int UnitOfMeasurementId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Death")]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime DateTaken { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Death")]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime DateReceived { get; set; }

        [Required]
        [Display(Name = "Result")]
        public decimal ResultValue { get; set; }
        public string ComparisionCharacter { get; set; }
        public string LabNumber { get; set; }

        public Drug Drug { get; set; }
        public Patient Patient { get; set; }
        public UnitOfMeasurement Unit { get; set; }
        

        public string Result()
        {
            if (!string.IsNullOrEmpty(ComparisionCharacter))
            {
                return ComparisionCharacter + ResultValue.ToString() + " " + Unit.Name;
            } else
            {
                return ResultValue.ToString() + " " + Unit.Name;
            }
        }

        public static List<string> ComparisionCharacters()
        {
            return new List<string>()
            {
                "<", ">"
            };
        }
    }
}
