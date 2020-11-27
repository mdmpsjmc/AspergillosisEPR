using AspergillosisEPR.Lib.Exporters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientDrugLevel : Exportable
    {

        public int ID { get; set; }
        public int PatientId { get; set; }

        [Required]
        [Display(Name = "Drug")]
        public int DrugId { get; set; }

        [Required]
        [Display(Name = "Unit")]
        public int UnitOfMeasurementId { get; set; }

        [Display(Name = "Date Taken")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateTaken { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Received")]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}")]
        public DateTime DateReceived { get; set; }

        [Required]
        [Display(Name = "Result")]
        public decimal ResultValue { get; set; }
        public string ComparisionCharacter { get; set; }
        public string LabNumber { get; set; }

        public Drug Drug { get; set; }
        public Patient Patient { get; set; }
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
        public decimal SourceSystemGUID { get; set; }
        public DateTime? CreatedDate { get; set; }

        override public List<string> ExcludedProperties()
        {
            return new List<string>()
            {
                "PatientId", "Patient", "Drug", "UnitOfMeasurement"
            };
        }

        public string Result()
        {
            if (!string.IsNullOrEmpty(ComparisionCharacter))
            {
                return ComparisionCharacter + ResultValue.ToString() + " " + UnitOfMeasurement.Name;
            } else
            {
                return ResultValue.ToString() + " " + UnitOfMeasurement.Name;
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
