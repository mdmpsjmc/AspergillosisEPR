using AspergillosisEPR.Lib.Exporters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientMeasurement : Exportable
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public decimal? Weight { get; set; } 
        public decimal? Height { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateTaken { get; set; }
        public string SourceInfo { get; set; }

        override public List<string> ExcludedProperties()
        {
            return new List<string>()
            {
                "PatientId", "Patient"
            };
        }

        public string BMI()
        {
          if (Height != null && Weight != null && Height != 0 && Weight != 0)
        {
          var bmi = Weight / ((Height / 100) * (Height / 100));
          return Math.Round(bmi.Value, 2).ToString();
        }
        else return "";
          
          
        }
    }
}