using AspergillosisEPR.Lib.Exporters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Models
{
    public class PatientMeasurement : Exportable
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public decimal? Weight { get; set; } 
        public decimal? Height { get; set; }
        public DateTime DateTaken { get; set; }

        override public List<string> ExcludedProperties()
        {
            return new List<string>()
            {
                "PatientId", "Patient"
            };
        }
    }
}