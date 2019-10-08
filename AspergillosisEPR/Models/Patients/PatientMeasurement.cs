using AspergillosisEPR.Lib.Exporters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models.Patients
{
    public class PatientMeasurement : Exportable
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public decimal? Weight { get; set; } 
        public decimal? Height { get; set; }
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